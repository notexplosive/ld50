using LD50.Gameplay;

namespace LD50.Data
{
    public readonly struct PendingSpell
    {
        public PendingSpell(PartyMember targetPartyMember, ISpell spell)
        {
            TargetPartyMember = targetPartyMember;
            Spell = spell;
        }

        public PartyMember TargetPartyMember { get; }
        public ISpell Spell { get; }
    }
}
