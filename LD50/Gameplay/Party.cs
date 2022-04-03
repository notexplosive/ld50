using System.Collections;
using System.Collections.Generic;
using Machina.Data;
using Machina.Engine;

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
        
        public IEnumerable<PartyMember> AllLivingMembers()
        {
            foreach (var member in this.partyMembers)
            {
                if (!member.Status.IsDead)
                {
                    yield return member;
                }
            }
        }

        /// <summary>
        /// Gets the tank if available, if not, picks a random Damage Dealer
        /// If no damage dealers are left, get the healer
        /// </summary>
        /// <returns></returns>
        public PartyMember GetMostThreateningPartyMember()
        {
            PartyMember healer = null;
            var damageDealers = new List<PartyMember>();
            foreach (var member in AllLivingMembers())
            {
                if (member.Role == PartyRole.Tank)
                {
                    return member;
                }
                
                if (member.Role == PartyRole.Damage)
                {
                    damageDealers.Add(member);
                }

                if (member.Role == PartyRole.Healer)
                {
                    healer = member;
                }
            }

            if (damageDealers.Count > 0)
            {
                Party.random.Shuffle(damageDealers);
                return damageDealers[0];
            }

            return healer;
        }

        public static NoiseBasedRNG random = new NoiseBasedRNG(123456);
    }
}
