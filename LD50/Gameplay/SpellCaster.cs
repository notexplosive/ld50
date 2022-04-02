using System.Collections.Generic;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Input;

namespace LD50.Gameplay
{
    public class SpellCaster : BaseComponent
    {
        private readonly List<PartyMemberTuple> partyTuples;

        public SpellCaster(Actor actor) : base(actor)
        {
            this.partyTuples = new List<PartyMemberTuple>();
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

        private void TryToCastSpell(int i)
        {
            var hoveredPartyMember = GetHoveredPartyMember();

            if (hoveredPartyMember == null)
            {
                MachinaClient.Print("No target");    
                return;
            }
            
            MachinaClient.Print("Casting spell",i);
        }

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
    }
}
