using System;
using LD50.Gameplay;
using LD50.Renderer;

namespace LD50.Data
{
    public interface ISpell
    {
        public string Name { get; }
        public float CastDuration { get; }
        public int ManaCost { get; }
        public int HealingAmountWhenComplete { get; }
        public IBuff BuffAppliedWhenComplete { get; }
        bool IsInstant => CastDuration == 0f;
        void Execute(PartyMember targetPartyMember, Party party);
    }
}
