using LD50.Gameplay;

namespace LD50.Data
{
    public class WholePartySpell : ISpell 
    {
        public WholePartySpell(string name, float castDuration, int manaCost, int healingAmountWhenComplete, Buff buffAppliedWhenComplete)
        {
            Name = name;
            ManaCost = manaCost;
            HealingAmountWhenComplete = healingAmountWhenComplete;
            BuffAppliedWhenComplete = buffAppliedWhenComplete;
            CastDuration = castDuration;
        }

        public string Name { get; }
        public float CastDuration { get; }
        public int ManaCost { get; }
        public int HealingAmountWhenComplete { get; }
        public Buff BuffAppliedWhenComplete { get; }
        public void Execute(PartyMember targetPartyMember, Party party)
        {
            foreach (var member in party.AllMembers())
            {
                member.TakeHeal(HealingAmountWhenComplete);
                member.GainBuff(BuffAppliedWhenComplete);
            }
        }
    }
}
