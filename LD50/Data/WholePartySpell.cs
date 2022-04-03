using System.Collections.Generic;
using LD50.Gameplay;
using Machina.Data.TextRendering;

namespace LD50.Data
{
    public class WholePartySpell : ISpell 
    {
        public WholePartySpell(string name, float castDuration, int manaCost, int healingAmountWhenComplete, IBuff buffAppliedWhenComplete, float cooldown, int keybind = 0, int frameIndex = 1)
        {
            Name = name;
            ManaCost = manaCost;
            HealingAmountWhenComplete = healingAmountWhenComplete;
            BuffAppliedWhenComplete = buffAppliedWhenComplete;
            Keybind = keybind;
            FrameIndex = frameIndex;
            CastDuration = castDuration;
            Cooldown = new Cooldown(cooldown);
        }

        public string Name { get; }
        public float CastDuration { get; }
        public int ManaCost { get; }
        public int HealingAmountWhenComplete { get; }
        public IBuff BuffAppliedWhenComplete { get; }
        public int Keybind { get; }
        public int FrameIndex { get; }
        public Cooldown Cooldown { get; }

        public void Execute(PartyMember targetPartyMember, Party party)
        {
            foreach (var member in party.AllMembers())
            {
                member.TakeHeal(HealingAmountWhenComplete);
                member.GainBuff(BuffAppliedWhenComplete);
            }
        }
        
        public string CastInstructions()
        {
            return $"Press {Keybind}";
        }

        public bool AttemptedRecently { get; set; }
    }
}
