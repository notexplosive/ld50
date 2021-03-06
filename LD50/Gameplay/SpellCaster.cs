using System.Collections.Generic;
using LD50.Data;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework.Input;

namespace LD50.Gameplay
{
    public class SpellCaster : BaseComponent
    {
        public static readonly float BufferWindow = 0.5f;
        private readonly TweenChain castingTween;
        private readonly SpellLogger logger;
        private readonly Party party;
        private readonly List<PartyMemberTuple> partyTuples;
        private readonly TweenAccessors<float> percentTweenable;
        private readonly PartyMember player;
        private readonly ISpell[] spells;

        public SpellCaster(Actor actor, Party party, ISpell[] spells, PartyMember player, Cooldown globalCooldown,
            SpellLogger logger = null) : base(actor)
        {
            this.logger = logger;
            this.player = player;
            this.spells = spells;
            this.party = party;
            this.partyTuples = new List<PartyMemberTuple>();
            this.castingTween = new TweenChain();
            this.percentTweenable = new TweenAccessors<float>(0);
            GlobalCooldown = globalCooldown;
        }

        public Cooldown GlobalCooldown { get; }

        public float Percent => this.percentTweenable.CurrentValue;

        public PendingSpell BufferedSpell { get; private set; }

        public PendingSpell InProgressSpell { get; private set; }

        public override void Update(float dt)
        {
            foreach (var spell in this.spells)
            {
                spell.Cooldown.Update(dt);
            }

            GlobalCooldown.Update(dt);

            if (BufferedSpell.Spell != null)
            {
                if (TryToCastSpell(BufferedSpell, true))
                {
                    ClearBufferedSpell();
                }
            }
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (state == ButtonState.Pressed)
            {
                if (key == Keys.Escape)
                {
                    CancelInProgressSpell();
                    ClearBufferedSpell();
                }

                if (key == Keys.D1)
                {
                    TryToCastSpell(0);
                }

                if (key == Keys.D2)
                {
                    TryToCastSpell(1);
                }

                if (key == Keys.D3)
                {
                    TryToCastSpell(2);
                }

                if (key == Keys.D4)
                {
                    TryToCastSpell(3);
                }

                if (key == Keys.D5)
                {
                    TryToCastSpell(4);
                }

                if (key == Keys.D6)
                {
                    TryToCastSpell(5);
                }
            }
        }

        public bool TryToCastSpell(PendingSpell pendingSpell, bool isFromBuffer = false)
        {
            if (!isFromBuffer)
            {
                ClearBufferedSpell();
            }

            if (this.player.Status.IsDead)
            {
                ClearBufferedSpell();
                this.logger.Log("You are dead.");

                return false;
            }

            var spell = pendingSpell.Spell;
            var hoveredPartyMember = pendingSpell.TargetPartyMember;

            if (hoveredPartyMember == null && spell is SingleTargetSpell)
            {
                this.logger.Log("You need to be hovering a target.");
                ClearBufferedSpell();
                return false;
            }

            if (!this.castingTween.IsDone())
            {
                var timeLeft = InProgressSpell.Spell.CastDuration -
                               this.percentTweenable.CurrentValue * InProgressSpell.Spell.CastDuration;

                if (timeLeft < SpellCaster.BufferWindow)
                {
                    BufferSpell(pendingSpell);
                }
                else
                {
                    this.logger.Log("You're already casting something else.");
                }

                return false;
            }

            if (!GlobalCooldown.IsReady())
            {
                if (GlobalCooldown.RemainingTime() < SpellCaster.BufferWindow)
                {
                    BufferSpell(pendingSpell);
                }
                else
                {
                    if (isFromBuffer)
                    {
                        ClearBufferedSpell();
                    }
                    
                    this.logger.Log("Not ready yet.");
                }

                return false;
            }

            if (!spell.Cooldown.IsReady())
            {
                if (spell.Cooldown.RemainingTime() < SpellCaster.BufferWindow)
                {
                    BufferSpell(pendingSpell);
                }
                else
                {
                    if (isFromBuffer)
                    {
                        ClearBufferedSpell();
                    }
                    
                    this.logger.Log("Not ready yet.");
                }

                return false;
            }

            if (spell.ManaCost > this.player.Status.Mana)
            {
                if (isFromBuffer)
                {
                    ClearBufferedSpell();
                }

                this.logger.Log("Not enough mana.");
                return false;
            }

            InProgressSpell = pendingSpell;
            GlobalCooldown.Start(); // gcd triggers when you START casting a spell

            void FinishCast()
            {
                InProgressSpell.Spell.Execute(InProgressSpell.TargetPartyMember, this.party);
                InProgressSpell.Spell.Cooldown.Start(); // regular cooldown starts when you finish the spell
                this.player.ConsumeMana(InProgressSpell.Spell.ManaCost);

                MachinaClient.SoundEffectPlayer.PlaySound(InProgressSpell.Spell.SoundEffect);
                
                CancelInProgressSpell();
            }

            if (!InProgressSpell.Spell.IsInstant)
            {
                this.castingTween.Clear();
                this.percentTweenable.setter(0);
                this.castingTween.AppendFloatTween(1f, InProgressSpell.Spell.CastDuration, EaseFuncs.Linear,
                    this.percentTweenable);
                this.castingTween.AppendCallback(FinishCast);
            }
            else
            {
                FinishCast();
            }

            return true;
        }

        public bool TryToCastSpell(int i)
        {
            ClearBufferedSpell();
            var hoveredPartyMember = GetHoveredPartyMember();

            if (i >= this.spells.Length)
            {
                return false;
            }

            var spell = this.spells[i];
            var pendingSpell = new PendingSpell(hoveredPartyMember, spell);

            var success = TryToCastSpell(pendingSpell);

            pendingSpell.Spell.OnAttempt();

            return success;
        }

        private void BufferSpell(PendingSpell pendingSpell)
        {
            var tryingToCastSameSpellTwice = pendingSpell.Spell == InProgressSpell.Spell;
            var pendingSpellHasCooldown = !pendingSpell.Spell.Cooldown.IsInstant;

            if (tryingToCastSameSpellTwice && !pendingSpellHasCooldown || !tryingToCastSameSpellTwice)
            {
                BufferedSpell = pendingSpell;
            }
        }

        public void AddPartyMemberInterface(Actor partyMemberActor, Hoverable hoverable, PartyMember partyMember)
        {
            this.partyTuples.Add(new PartyMemberTuple(partyMemberActor, hoverable, partyMember));

            partyMember.Died += WhenPartyMemberDies;
        }

        private void WhenPartyMemberDies(PartyMember member)
        {
            if (InProgressSpell.TargetPartyMember == member)
            {
                CancelInProgressSpell();
            }

            if (BufferedSpell.TargetPartyMember == member)
            {
                ClearBufferedSpell();
            }

            if (member == this.player)
            {
                CancelInProgressSpell();
                ClearBufferedSpell();
            }
        }

        private void ClearBufferedSpell()
        {
            BufferedSpell = new PendingSpell();
        }

        private void CancelInProgressSpell()
        {
            this.percentTweenable.setter(0f);
            this.castingTween.Clear();
            InProgressSpell = new PendingSpell();
        }

        public PartyMember GetHoveredPartyMember()
        {
            foreach (var tuple in this.partyTuples)
            {
                if (tuple.hoverable.IsHovered && !tuple.partyMember.Status.IsDead)
                {
                    return tuple.partyMember;
                }
            }

            return null;
        }

        public void UpdateTween(float dt)
        {
            this.castingTween.Update(dt);
        }

        private readonly struct PartyMemberTuple
        {
            public readonly Actor partyMemberActor;
            public readonly Hoverable hoverable;
            public readonly PartyMember partyMember;

            public PartyMemberTuple(Actor partyMemberActor, Hoverable hoverable, PartyMember partyMember)
            {
                this.partyMemberActor = partyMemberActor;
                this.hoverable = hoverable;
                this.partyMember = partyMember;
            }
        }
    }
}
