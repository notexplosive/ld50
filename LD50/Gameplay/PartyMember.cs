using System.Collections.Generic;
using LD50.Data;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public delegate void PartyMemberEvent(PartyMember member);
    
    public class PartyMember
    {
        public PartyMemberStatus Status { get; private set; }
        public PartyRole Role { get; }
        private int pendingDamage;
        private int pendingHeals;
        private int pendingSpentMana;

        public event PartyMemberEvent Died;

        public PartyMember(BaseStats baseStats, PartyRole role = default)
        {
            Status = new PartyMemberStatus(baseStats, new Buffs(), 0, baseStats.MaxMana);
            Role = role;
            Name = GetHashCode().ToString();
        }

        public float HealthPercent => (float)Status.Health / Status.BaseStats.MaxHealth;
        public float ManaPercent => (float)Status.Mana / Status.BaseStats.MaxMana;
        public string Name { get; }

        public void Update(float dt)
        {
            var nextStatus = Status.GetNext(dt, this.pendingHeals, this.pendingDamage, this.pendingSpentMana);
            var previousStatus = Status;

            Status = nextStatus;
            this.pendingDamage = 0;
            this.pendingHeals = 0;
            this.pendingSpentMana = 0;
            
            if (!previousStatus.IsDead && nextStatus.IsDead)
            {
                Status.Buffs.Clear();
                Died?.Invoke(this);
            }
        }

        public void TakeDamage(int damage)
        {
            this.pendingDamage += damage;
        }

        public void TakeHeal(int heal)
        {
            if (Status.IsDead)
            {
                return;
            }
            
            this.pendingHeals += heal;
        }

        public void GainBuff(IBuff buff)
        {
            if (Status.IsDead)
            {
                return;
            }
            
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

        public IEnumerator<ICoroutineAction> AttackCoroutine(Encounter encounter)
        {
            while (!encounter.IsFightOver() && !Status.IsDead)
            {
                var monster = encounter.GetRandomLivingMonster();
                monster.TakeDamage(Status.BaseStats.DamageOutput, encounter);
                yield return new WaitSeconds(1f); // todo: maybe each party member should have a different attack speed
            }
        }

        public void Revive()
        {
            if (Status.IsDead)
            {
                this.pendingHeals += Status.BaseStats.MaxHealth / 10;
            }
        }
    }
}
