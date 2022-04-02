using System;

namespace LD50.Data
{
    public readonly struct PartyMemberStatus
    {
        private readonly int damageTaken;
        public BaseStats BaseStats { get; }
        public int Health => BaseStats.MaxHealth - this.damageTaken;

        public PartyMemberStatus(BaseStats baseStats, Buffs appliedBuffs, int damageTaken = 0)
        {
            BaseStats = baseStats;
            this.damageTaken = damageTaken;
            Buffs = appliedBuffs;
        }

        public Buffs Buffs { get; }

        public PartyMemberStatus GetNext(float dt, int healsThisFrame, int damageThisFrame)
        {
            healsThisFrame += Buffs.GetHealingThisTick(dt);
            damageThisFrame += Buffs.GetDamageThisTick(dt);
            damageThisFrame = Buffs.CalculateAbsorbedDamage(damageThisFrame);

            var newDamageTaken = this.damageTaken - healsThisFrame + damageThisFrame;
            newDamageTaken = Math.Clamp(newDamageTaken, 0, BaseStats.MaxHealth);

            return new PartyMemberStatus(BaseStats, Buffs.GetUpdated(dt), newDamageTaken);
        }

        public Buffs GetBuffs()
        {
            var result = new Buffs();

            foreach (var buff in Buffs.AllBuffs())
            {
                result.AddBuff(buff);
            }

            return result;
        }
    }
}
