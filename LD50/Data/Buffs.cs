using System;
using System.Collections.Generic;
using Machina.Engine;

namespace LD50.Data
{
    public class Buffs
    {
        private readonly List<IBuff> list = new List<IBuff>();
        public object Count => this.list.Count;

        public void AddBuff(IBuff buff)
        {
            this.list.Add(buff);
        }

        public int CalculateAbsorbedDamage(int incomingDamage)
        {
            if (incomingDamage == 0)
            {
                return 0;
            }
            
            for(int i = 0; i < this.list.Count; i++)
            {
                var buff = this.list[i];
                if (buff is ShieldBuff shield)
                {
                    var damageWithAbsorbedRemove = shield.GetDamageAfterAbsorb(incomingDamage);
                    var damageMitigated = incomingDamage - damageWithAbsorbedRemove;
                    this.list[i] = shield.GetBuffAfterAbsorb(damageMitigated);
                    
                    incomingDamage = damageWithAbsorbedRemove;
                }
            }
            
            return incomingDamage;
        }
        
        public Buffs GetUpdated(float dt)
        {
            var result = new Buffs();

            foreach (var buff in this.list)
            {
                var newBuff = buff.GetNext(dt);

                var shouldCarryOver = newBuff.RemainingDuration > 0;

                if (newBuff is EmptyBuff)
                {
                    shouldCarryOver = false;
                }

                if (newBuff is ShieldBuff {DamageAbsorb: 0})
                {
                    shouldCarryOver = false;
                }
                
                if (shouldCarryOver)
                {
                    result.AddBuff(newBuff);
                }
            }

            return result;
        }

        public int GetHealingThisTick(float dt)
        {
            var totalHealing = 0;
            foreach (var buff in this.list)
            {
                totalHealing += buff.GetHealAmount(dt);
            }

            return totalHealing;
        }

        public int GetDamageThisTick(float dt)
        {
            return 0;
        }

        public IBuff[] AllNonEmptyBuffs()
        {
            var result = new List<IBuff>();
            foreach (var item in this.list)
            {
                if (!(item is EmptyBuff))
                {
                    result.Add(item);
                }
            }
            
            return result.ToArray();
        }

        public IBuff At(int index)
        {
            return this.list[index];
        }
    }
}
