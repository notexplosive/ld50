using System.Collections;

namespace LD50.Gameplay
{
    public class Party
    {
        private readonly PartyMember[] partyMembers;

        public Party(params PartyMember[] partyMembers)
        {
            this.partyMembers = partyMembers;
        }

        public PartyMember GetMember(int partyMemberIndex)
        {
            return this.partyMembers[partyMemberIndex];
        }

        public PartyMember[] AllMembers()
        {
            return this.partyMembers;
        }
    }
}
