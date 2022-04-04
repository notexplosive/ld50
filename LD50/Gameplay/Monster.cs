using System;
using System.Collections.Generic;
using System.Linq;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class Monster
    {
        public static NoiseBasedRNG random = new NoiseBasedRNG(45678);
        private readonly float initialDelay;
        public SoundEffects SoundEffects { get; }
        public int Health { get; private set; }
        public int DamagePerHit { get; }
        public float AttackDelay { get; }
        public bool IsDead => Health <= 0;
        public string Name { get; }

        public Monster(SoundEffects soundEffects, int health, int damagePerHit, float attackDelay, string name = "Monster")
        {
            SoundEffects = soundEffects;
            Health = health;
            DamagePerHit = damagePerHit;
            AttackDelay = attackDelay;
            Name = name;

            this.initialDelay = Monster.random.NextFloat() * attackDelay;
        }

        public void TakeDamage(int damage, Encounter encounter, PartyMember attacker)
        {
            Health -= damage;
            if (IsDead)
            {
                encounter.Logger.LogHappy($"{attacker.Name} killed the {Name}.");
                MachinaClient.SoundEffectPlayer.PlaySound(SoundEffects.Die);
            }
        }

        public IEnumerator<ICoroutineAction> AttackCoroutine(Party party, Encounter encounter)
        {
            var target = party.GetMostThreateningPartyMember();
            yield return new WaitSeconds(this.initialDelay);

            while (!IsDead)
            {
                if (target == null || target.Status.IsDead)
                {
                    target = party.GetMostThreateningPartyMember();
                }
                
                if (target == null)
                {
                    encounter.Logger.LogNormal($"{Name} dances over the party's dead bodies");
                    encounter.Logger.LogNormal($"You made it to level {encounter.Level}");
                    encounter.Logger.LogNormal($"Press Space to return to the main menu");
                    yield return new WaitUntil(() => false);
                }
                else
                {
                    var possibleSecondaryTargets = party.AllLivingDamageMembers().ToArray();
                    if (possibleSecondaryTargets.Length > 0 && Monster.random.NextFloat() < 0.10f)
                    {
                        possibleSecondaryTargets[Monster.random.Next(possibleSecondaryTargets.Length)].TakeDamage(DamagePerHit);
                    }
                    else
                    {
                        MachinaClient.SoundEffectPlayer.PlaySound(SoundEffects.DealDamage);
                        target.TakeDamage(DamagePerHit);
                    }

                    yield return new WaitSeconds(AttackDelay);
                }
            }
        }
    }
}
