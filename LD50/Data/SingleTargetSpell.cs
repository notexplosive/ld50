using LD50.Gameplay;

namespace LD50.Data
{
    public class SingleTargetSpell : ISpell
    {
        public string Name { get; }

        public SingleTargetSpell(string name, float castDuration, int manaCost, int healingAmountWhenComplete, Buff buffAppliedWhenComplete)
        {
            Name = name;
            ManaCost = manaCost;
            HealingAmountWhenComplete = healingAmountWhenComplete;
            BuffAppliedWhenComplete = buffAppliedWhenComplete;
            CastDuration = castDuration;
        }
        
        public float CastDuration { get; }
        public int ManaCost { get; }
        public int HealingAmountWhenComplete { get; }
        public Buff BuffAppliedWhenComplete { get; }
        
        public void Execute(PartyMember targetPartyMember, Party party)
        {
            targetPartyMember.TakeHeal(HealingAmountWhenComplete);
            targetPartyMember.GainBuff(BuffAppliedWhenComplete);
        }
    }
}
