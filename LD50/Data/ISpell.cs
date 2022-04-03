﻿using System;
using LD50.Gameplay;
using LD50.Renderer;

namespace LD50.Data
{
    public interface ISpell
    {
        public int FrameIndex { get; }
        public string Name { get; }
        public float CastDuration { get; }
        public int ManaCost { get; }
        public int HealingAmountWhenComplete { get; }
        public IBuff BuffAppliedWhenComplete { get; }
        public Cooldown Cooldown { get; }
        bool IsInstant => CastDuration == 0f;
        void Execute(PartyMember targetPartyMember, Party party);
    }
}
