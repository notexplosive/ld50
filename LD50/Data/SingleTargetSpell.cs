using System.Collections.Generic;
using LD50.Gameplay;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace LD50.Data
{
    public class SingleTargetSpell : ISpell
    {
        public string Name { get; }

        public SingleTargetSpell(string name, float castDuration, int manaCost, int healingAmountWhenComplete, IBuff buffAppliedWhenComplete, float cooldown, int keybind = 0, int frameIndex = 0, string soundEffect = "heal_fast")
        {
            Name = name;
            ManaCost = manaCost;
            HealingAmountWhenComplete = healingAmountWhenComplete;
            BuffAppliedWhenComplete = buffAppliedWhenComplete;
            Keybind = keybind;
            FrameIndex = frameIndex;
            SoundEffect = soundEffect;
            CastDuration = castDuration;
            Cooldown = new Cooldown(cooldown);
        }

        public Cooldown Cooldown { get; }
        public float CastDuration { get; }
        public int ManaCost { get; }
        public int HealingAmountWhenComplete { get; }
        public IBuff BuffAppliedWhenComplete { get; }
        public int Keybind { get; }
        public int FrameIndex { get; }
        public string SoundEffect { get; }

        public void Execute(PartyMember targetPartyMember, Party party)
        {
            targetPartyMember.TakeHeal(HealingAmountWhenComplete);
            targetPartyMember.GainBuff(BuffAppliedWhenComplete);
        }

        public string CastInstructions()
        {
            return $"Mouse over target and press {Keybind}";
        }

        public bool AttemptedRecently { get; set; }
    }
}
