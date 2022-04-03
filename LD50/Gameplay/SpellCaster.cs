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
        private readonly List<PartyMemberTuple> partyTuples;
        private readonly TweenChain castingTween;
        private readonly TweenAccessors<float> percentTweenable;
        private readonly ISpell[] spells;
        private readonly Party party;
        private readonly PartyMember player;
        public Cooldown GlobalCooldown { get; }

        public SpellCaster(Actor actor, Party party, ISpell[] spells, PartyMember player, Cooldown globalCooldown) : base(actor)
        {
            this.player = player;
            this.spells = spells;
            this.party = party;
            this.partyTuples = new List<PartyMemberTuple>();
            this.castingTween = new TweenChain();
            this.percentTweenable = new TweenAccessors<float>(0);
            GlobalCooldown = globalCooldown;
        }

        public float Percent => this.percentTweenable.CurrentValue;

        public override void Update(float dt)
        {
            foreach (var spell in this.spells)
            {
                spell.Cooldown.Update(dt);
            }
            
            GlobalCooldown.Update(dt);
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (state == ButtonState.Pressed)
            {
                if (key == Keys.Escape)
                {
                    CancelInProgressSpell();
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

        public bool TryToCastSpell(int i)
        {
            var hoveredPartyMember = GetHoveredPartyMember();

            if (this.player.Status.IsDead)
            {
                MachinaClient.Print("You are dead");
                return false;
            }
            
            if (i >= this.spells.Length)
            {
                MachinaClient.Print("no spell at that index", i);
                return false;
            }
            
            var spell = this.spells[i];

            if (!this.castingTween.IsDone())
            {
                MachinaClient.Print("Casting in progress");
                // todo: buffer next spell if there's less than a half second cast time remaining
                return false;
            }

            if (!GlobalCooldown.IsReady())
            {
                // todo: buffer next spell if there's less than half second remaining
                MachinaClient.Print("global cooldown");
                return false;
            }
            
            if (!spell.Cooldown.IsReady())
            {
                MachinaClient.Print("that spell is on cooldown");
                // todo: buffer next spell if there's less than a half second remaining
                return false;
            }

            if (hoveredPartyMember == null && spell is SingleTargetSpell)
            {
                MachinaClient.Print("No target");    
                return false;
            }

            if (spell.ManaCost > this.player.Status.Mana)
            {
                MachinaClient.Print("Not enough mana");    
                return false;
            }

            MachinaClient.Print("Casting spell", spell.Name);

            InProgressSpell = new PendingSpell(hoveredPartyMember, spell);
            GlobalCooldown.Start(); // gcd triggers when you START casting a spell
            
            void FinishCast()
            {
                InProgressSpell.Spell.Execute(InProgressSpell.TargetPartyMember, this.party);
                InProgressSpell.Spell.Cooldown.Start(); // regular cooldown starts when you finish the spell
                this.player.ConsumeMana(InProgressSpell.Spell.ManaCost);
                CancelInProgressSpell();
            }
            
            if (!InProgressSpell.Spell.IsInstant)
            {
                this.castingTween.Clear();
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

        public PendingSpell InProgressSpell { get; private set; }

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

            if (member == this.player)
            {
                CancelInProgressSpell();
            }
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

        public void UpdateTween(float dt)
        {
            this.castingTween.Update(dt);
        }
    }
}
