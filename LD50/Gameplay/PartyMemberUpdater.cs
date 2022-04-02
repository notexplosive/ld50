using Machina.Components;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class PartyMemberUpdater : BaseComponent
    {
        private readonly PartyMember partyMember;

        public PartyMemberUpdater(Actor actor, PartyMember partyMember) : base(actor)
        {
            this.partyMember = partyMember;
        }

        public override void Update(float dt)
        {
            this.partyMember.Update(dt);
        }
    }
}
