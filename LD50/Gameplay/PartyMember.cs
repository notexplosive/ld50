using System.Collections.Generic;
using LD50.Data;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public enum PartyPortrait
    {
        Tank,
        Mage,
        Rogue,
        Druid,
        Healer
    }
    
    public delegate void PartyMemberEvent(PartyMember member);
    
    public class PartyMember
    {
        public PartyMemberStatus Status { get; private set; }
        public PartyRole Role { get; }
        public PartyPortrait Portrait { get; }
        public int PendingDamage { get; private set; }
        public int PendingHeals { get; private set; }
        public int PendingSpentMana { get; private set; }

        public event PartyMemberEvent Died;

        public PartyMember(BaseStats baseStats, string name = "P.T. Member", PartyRole role = default, PartyPortrait portrait = PartyPortrait.Tank)
        {
            Status = new PartyMemberStatus(baseStats, new Buffs(), 0, baseStats.MaxMana);
            Role = role;
            Portrait = portrait;
            Name = name;
        }

        public float HealthPercent => (float)Status.Health / Status.BaseStats.MaxHealth;
        public float ManaPercent => (float)Status.Mana / Status.BaseStats.MaxMana;
        public string Name { get; }

        public void Update(float dt)
        {
            var nextStatus = Status.GetNext(dt, this.PendingHeals, this.PendingDamage, this.PendingSpentMana);
            var previousStatus = Status;

            Status = nextStatus;

            this.PendingDamage = 0;
            this.PendingHeals = 0;
            this.PendingSpentMana = 0;
            
            if (!previousStatus.IsDead && nextStatus.IsDead)
            {
                Status.Buffs.Clear();
                Died?.Invoke(this);
            }
        }

        public void TakeDamage(int damage)
        {
            this.PendingDamage += damage;
        }

        public void TakeHeal(int heal)
        {
            if (Status.IsDead)
            {
                return;
            }
            
            this.PendingHeals += heal;
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
            this.PendingSpentMana += manaCost;
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
                this.PendingHeals += Status.BaseStats.MaxHealth / 10;
            }
        }
    }
}
