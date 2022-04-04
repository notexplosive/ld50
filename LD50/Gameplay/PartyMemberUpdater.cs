using Machina.Components;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class PartyMemberUpdater : BaseComponent
    {
        private readonly PartyMember partyMember;
        private readonly Party party;

        public PartyMemberUpdater(Actor actor, PartyMember partyMember, Party party) : base(actor)
        {
            this.partyMember = partyMember;
            this.party = party;
        }

        public override void Update(float dt)
        {
            this.partyMember.Update(dt);

            if (!this.party.InCombat)
            {
                if (!this.partyMember.Status.IsFullyHealed)
                {
                    this.partyMember.TakeHeal(1);
                }

                if (!this.partyMember.Status.IsFullMana)
                {
                    this.partyMember.RegenerateMana(1);
                }
            }
        }
    }
}
