using System;

namespace LD50.Data
{
    public readonly struct PartyMemberStatus
    {
        private readonly int damageTaken;
        private readonly float manaFraction;
        public BaseStats BaseStats { get; }
        public int Health => BaseStats.MaxHealth - this.damageTaken;
        public int Mana { get; }

        public PartyMemberStatus(BaseStats baseStats, Buffs appliedBuffs, int damageTaken = 0, int mana = 0, float manaFraction = 0f)
        {
            BaseStats = baseStats;
            this.damageTaken = damageTaken;
            Buffs = appliedBuffs;
            IsDead = damageTaken >= BaseStats.MaxHealth;
            Mana = mana;
            this.manaFraction = manaFraction;
        }

        public bool IsDead { get; }
        public Buffs Buffs { get; }

        public PartyMemberStatus GetNext(float dt, int healsThisFrame, int damageThisFrame, int spentManaThisFrame)
        {
            healsThisFrame += Buffs.GetHealingThisTick(dt);
            damageThisFrame += Buffs.GetDamageThisTick(dt);
            damageThisFrame = Buffs.CalculateAbsorbedDamage(damageThisFrame);

            var newDamageTaken = this.damageTaken - healsThisFrame + damageThisFrame;
            newDamageTaken = Math.Clamp(newDamageTaken, 0, BaseStats.MaxHealth);

            var exactNewMana = Mana + CalculateRegeneratedMana(dt) + this.manaFraction - spentManaThisFrame;
            exactNewMana = Math.Clamp(exactNewMana, 0, BaseStats.MaxMana);
            var roundedMana = (int) exactNewMana;
            var newManaFraction = exactNewMana - roundedMana;

            return new PartyMemberStatus(BaseStats, Buffs.GetUpdated(dt), newDamageTaken, roundedMana, newManaFraction);
        }

        private float CalculateRegeneratedMana(float dt)
        {
            return BaseStats.ManaRegenPerSecond * dt;
        }

        public Buffs GetBuffs()
        {
            var result = new Buffs();

            foreach (var buff in Buffs.AllNonEmptyBuffs())
            {
                result.AddBuff(buff);
            }

            return result;
        }
    }
}
