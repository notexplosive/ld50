﻿using LD50.Data;

namespace LD50.Gameplay
{
    public delegate void PartyMemberEvent(PartyMember member);
    
    public class PartyMember
    {
        public PartyMemberStatus Status { get; private set; }
        private int pendingDamage;
        private int pendingHeals;
        private int pendingSpentMana;

        public event PartyMemberEvent Died;

        public PartyMember(BaseStats baseStats)
        {
            Status = new PartyMemberStatus(baseStats, new Buffs(), 0, baseStats.MaxMana);
        }

        public float HealthPercent => (float)Status.Health / Status.BaseStats.MaxHealth;
        public float ManaPercent => (float)Status.Mana / Status.BaseStats.MaxMana;

        public void Update(float dt)
        {
            var nextStatus = Status.GetNext(dt, this.pendingHeals, this.pendingDamage, this.pendingSpentMana);

            if (!Status.IsDead && nextStatus.IsDead)
            {
                Status.Buffs.Clear();
                Died?.Invoke(this);
            }
            
            Status = nextStatus;
            this.pendingDamage = 0;
            this.pendingHeals = 0;
            this.pendingSpentMana = 0;
        }

        public void TakeDamage(int damage)
        {
            this.pendingDamage += damage;
        }

        public void TakeHeal(int heal)
        {
            this.pendingHeals += heal;
        }

        public void GainBuff(IBuff buff)
        {
            Status.Buffs.AddBuff(buff);
        }

        public Buffs GetBuffs()
        {
            return Status.GetBuffs();
        }

        public void ConsumeMana(int manaCost)
        {
            this.pendingSpentMana += manaCost;
        }
    }
}
