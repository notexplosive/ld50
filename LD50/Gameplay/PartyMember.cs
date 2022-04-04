using System.Collections.Generic;
using System.Linq;
using LD50.Data;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
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
            var nextStatus = Status.GetNext(dt, PendingHeals, PendingDamage, PendingSpentMana);
            var previousStatus = Status;

            Status = nextStatus;

            PendingDamage = 0;
            PendingHeals = 0;
            PendingSpentMana = 0;
            
            if (!previousStatus.IsDead && nextStatus.IsDead)
            {
                Status.Buffs.Clear();
                Died?.Invoke(this);
            }
        }

        public void TakeDamage(int damage)
        {
            PendingDamage += damage;
        }

        public void TakeHeal(int heal)
        {
            if (Status.IsDead)
            {
                return;
            }
            
            PendingHeals += heal;
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
            PendingSpentMana += manaCost;
        }

        public IEnumerator<ICoroutineAction> AttackCoroutine(Encounter encounter, Chat chat)
        {
            yield return new WaitSeconds(Status.BaseStats.AttackDelay);
            
            while (!encounter.IsFightOver() && !Status.IsDead)
            {
                var monster = encounter.GetAllLivingMonsters().ToArray()[0];
                var damage = Status.BaseStats.DamageOutput;
                if (damage > 0)
                {
                    monster.TakeDamage(damage, encounter, this);
                    
                    // spend mana
                    Status = Status.GetNext(0f, 0, 0, damage * 2);
                }

                yield return new WaitSeconds(Status.BaseStats.AttackDelay);
            }
        }

        public void Revive()
        {
            if (Status.IsDead)
            {
                PendingHeals += Status.BaseStats.MaxHealth / 10;
            }
        }

        public void RegenerateMana(int amount)
        {
            Status = Status.GetNext(0, 0, 0, -amount);
        }
    }
}
