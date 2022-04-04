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
        public bool IsFullMana => Mana == BaseStats.MaxMana;
        public bool IsFullyHealed => this.damageTaken == 0;

        public int GetAbsorbedDamageThisFrame(float dt, int incomingDamage)
        {
            incomingDamage += Buffs.GetDamageThisTick(dt);
            var damageBeforeAbsorb = incomingDamage;
            incomingDamage = Buffs.CalculateDamageAfterAbsorb(incomingDamage);
            return damageBeforeAbsorb - incomingDamage;
        }

        public int GetDamageTakenThisFrame(float dt, int incomingDamage)
        {
            incomingDamage = Buffs.CalculateDamageAfterAbsorb(incomingDamage + Buffs.GetDamageThisTick(dt));
            return incomingDamage;
        }

        public int GetHealingTakenThisFrame(float dt, int healing)
        {
            return healing + Buffs.GetHealingThisTick(dt);
        }
        
        public PartyMemberStatus GetNext(float dt, int healsThisFrame, int damageThisFrame, int spentManaThisFrame)
        {
            healsThisFrame += Buffs.GetHealingThisTick(dt);
            var newTotalDamageTaken = this.damageTaken - healsThisFrame + GetDamageTakenThisFrame(dt, damageThisFrame);
            newTotalDamageTaken = Math.Clamp(newTotalDamageTaken, 0, BaseStats.MaxHealth);

            var exactNewMana = Mana + CalculateRegeneratedMana(dt) + this.manaFraction - spentManaThisFrame;
            exactNewMana = Math.Clamp(exactNewMana, 0, BaseStats.MaxMana);
            var roundedMana = (int) exactNewMana;
            var newManaFraction = exactNewMana - roundedMana;
            
            return new PartyMemberStatus(BaseStats, Buffs.GetUpdated(dt), newTotalDamageTaken, roundedMana, newManaFraction);
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
