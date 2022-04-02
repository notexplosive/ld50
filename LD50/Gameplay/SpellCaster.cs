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
        private readonly Cooldown globalCooldown;

        public SpellCaster(Actor actor, Party party, ISpell[] spells, Cooldown globalCooldown) : base(actor)
        {
            this.spells = spells;
            this.party = party;
            this.partyTuples = new List<PartyMemberTuple>();
            this.castingTween = new TweenChain();
            this.percentTweenable = new TweenAccessors<float>(0);
            this.globalCooldown = globalCooldown;
        }

        public float Percent => this.percentTweenable.CurrentValue;

        public override void Update(float dt)
        {
            foreach (var spell in this.spells)
            {
                spell.Cooldown.Update(dt);
            }
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (state == ButtonState.Pressed)
            {
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

            if (i >= this.spells.Length)
            {
                MachinaClient.Print("no spell at that index", i);
            }
            
            var spell = this.spells[i];

            if (!this.castingTween.IsDone())
            {
                MachinaClient.Print("Casting in progress");
                // todo: buffer next spell if there's less than a half second cast time remaining
                return false;
            }

            if (!this.globalCooldown.IsReady())
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

            MachinaClient.Print("Casting spell", spell.Name);

            InProgressSpell = new PendingSpell(hoveredPartyMember, spell);

            void Cast()
            {
                InProgressSpell.Spell.Execute(InProgressSpell.TargetPartyMember, this.party);
                InProgressSpell.Spell.Cooldown.Start();
                this.globalCooldown.Start();
            }
            
            if (!InProgressSpell.Spell.IsInstant)
            {
                this.castingTween.Clear();
                this.castingTween.AppendFloatTween(1f, InProgressSpell.Spell.CastDuration, EaseFuncs.Linear,
                    this.percentTweenable);
                this.castingTween.AppendCallback(
                    () =>
                    {
                        this.percentTweenable.setter(0f);
                        Cast();
                    });
            }
            else
            {
                Cast();
            }

            return true;
        }

        public PendingSpell InProgressSpell { get; private set; }

        public void AddPartyMemberInterface(Actor partyMemberActor, Hoverable hoverable, PartyMember partyMember)
        {
            this.partyTuples.Add(new PartyMemberTuple(partyMemberActor, hoverable, partyMember));
        }

        public PartyMember GetHoveredPartyMember()
        {
            foreach (var tuple in this.partyTuples)
            {
                if (tuple.hoverable.IsHovered)
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
