using System.Collections.Generic;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class Monster
    {
        public static NoiseBasedRNG random = new NoiseBasedRNG(45678);
        
        private readonly float initialDelay;
        public int Health { get; private set; }
        public int DamagePerHit { get; }
        public float AttackDelay { get; }
        public bool IsDead => Health <= 0;

        public Monster(int health, int damagePerHit, float attackDelay)
        {
            Health = health;
            DamagePerHit = damagePerHit;
            AttackDelay = attackDelay;

            this.initialDelay = Monster.random.NextFloat() * attackDelay;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public IEnumerator<ICoroutineAction> AttackCoroutine(Party party)
        {
            var target = party.GetMostThreateningPartyMember();
            yield return new WaitSeconds(this.initialDelay);

            while (!IsDead)
            {
                if (target.Status.IsDead)
                {
                    target = party.GetMostThreateningPartyMember();
                }
                
                if (target == null)
                {
                    MachinaClient.Print("Monster could not find anything to attack");
                    yield return new WaitUntil(() => false);
                }
                else
                {
                    target.TakeDamage(DamagePerHit);
                    yield return new WaitSeconds(AttackDelay);
                }
            }
        }
    }
}
