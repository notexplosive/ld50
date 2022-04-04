using System;
using System.Collections.Generic;
using System.Linq;
using LD50.Data;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class Encounter
    {
        private readonly Monster[] monsters;
        private readonly NoiseBasedRNG random;
        public BattleLogger Logger { get; }
        public int NumberOfMonsters => this.monsters.Length;

        public Encounter(int level, BattleLogger logger, params Monster[] monsters)
        {
            Level = level;
            Logger = logger;
            this.monsters = monsters;
            this.random = new NoiseBasedRNG(5656);
        }

        public int Level { get; }

        public void StartCoroutines(Scene scene, Party party, Chat chat)
        {
            foreach (var monster in this.monsters)
            {
                scene.StartCoroutine(monster.AttackCoroutine(party, this));
            }

            foreach (var partyMember in party.AllLivingMembers())
            {
                scene.StartCoroutine(partyMember.AttackCoroutine(this, chat));
            }
        }

        public Monster GetRandomLivingMonster()
        {
            var livingMonsters = GetAllLivingMonsters().ToArray();

            if (livingMonsters.Length > 0)
            {
                this.random.Shuffle(livingMonsters);
            }

            return livingMonsters[0];
        }

        public bool IsFightOver()
        {
            return !GetAllLivingMonsters().Any();
        }

        public IEnumerable<Monster> GetAllLivingMonsters()
        {
            foreach (var monster in this.monsters)
            {
                if (!monster.IsDead)
                {
                    yield return monster;
                }
            }
        }

        public void PrintStatus(MonsterMaker maker)
        {
            Logger.LogStatus($"(Level {Level+1})");

            var monsterName = this.monsters[0].Name;
            if (NumberOfMonsters > 1)
            {
                Logger.LogNormal($"A {maker.RandomPlurality()} of {NumberOfMonsters} {monsterName}s attacks!");
            }
            else
            {
                Logger.LogNormal($"{monsterName} attacks!");
            }
        }
    }
}
