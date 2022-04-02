using System.Collections.Generic;
using Machina.Engine;

namespace LD50.Data
{
    public class Buffs
    {
        private readonly List<Buff> list = new List<Buff>();

        public void AddBuff(Buff buff)
        {
            this.list.Add(buff);
        }
        
        public Buffs GetNext(float dt)
        {
            var result = new Buffs();

            foreach (var buff in list)
            {
                var newBuff = buff.GetNext(dt);
                if (newBuff.RemainingDuration > 0)
                {
                    result.AddBuff(newBuff);
                }
            }

            return result;
        }

        public int GetHealingThisTick(float dt)
        {
            var totalHealing = 0;
            foreach (var buff in list)
            {
                totalHealing += buff.GetHealAmount(dt);
            }

            return totalHealing;
        }

        public int GetDamageThisTick(float dt)
        {
            return 0;
        }
    }
}
