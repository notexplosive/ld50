using System;
using System.Collections.Generic;
using Machina.Data;

namespace LD50.Gameplay
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Boss
    }

    public class MonsterMaker
    {
        private readonly Dictionary<Difficulty, string[]> possibleAdjectives = new Dictionary<Difficulty, string[]>
        {
            {Difficulty.Easy, new[] {"Red", "Blue", "Green", "Yellow", "Sickly", "Wimpy", "Small", "Soft", "Stone"}},
            {
                Difficulty.Medium,
                new[]
                {
                    "Dark", "Purple", "Orange", "Pink", "Big", "Fire", "Frost", "Hairy", "Spiky", "Angry", "Metal",
                    "Shiny"
                }
            },
            {
                Difficulty.Hard,
                new[]
                {
                    "Mecha", "Huge", "Enchanted", "Giant", "Vermilion", "Celadon", "Amaranth", "Sapphire", "Ruby",
                    "Deadly", "Titanium", "Glowing"
                }
            },
            {
                Difficulty.Boss,
                new[]
                {
                    "Ultimate Final", "Gigantic", "Super Mega Ultra", "Diamond", "The Perfect", "The First",
                    "The Forgotten", "The Last"
                }
            }
        };

        private readonly string[] plurality =
        {
            "gang", "squad", "gaggle", "gathering", "posse", "school", "pod"
        };
        
        private readonly string[] possibleNouns =
        {
            "Goblin", "Bandit", "Kobold", "Golem", "Snake", "Boar", "Wolf", "Trout", "Fox", "Panther", "Lion",
            "Motorcycle", "Elemental", "Slime", "Squid", "Frog", "Toad", "Walking Mushroom", "Tree Person", "Spider",
            "Dragon", "Centaur"
        };

        private readonly NoiseBasedRNG random;

        public MonsterMaker(uint randomSeed)
        {
            this.random = new NoiseBasedRNG(randomSeed);
        }

        public Encounter CreateEncounter(int level, BattleLogger logger)
        {
            var monsters = new List<Monster>();

            var numberOfMonsters = GetNumberOfMonsters();
            var monsterName = GetMonsterName(level);

            for (var i = 0; i < numberOfMonsters; i++)
            {
                var monsterDamage = GetMonsterDamagePerHit(level, numberOfMonsters);
                monsters.Add(new Monster(GetMonsterHealth(level, numberOfMonsters), monsterDamage,
                    TuningKnobs.GetMonsterAttackDelay(level), monsterName));
            }

            logger.LogStatus($"(Level {level})");
            
            if (numberOfMonsters > 1)
            {
                logger.LogNormal($"A {RandomPlurality()} of {numberOfMonsters} {monsterName}s attacks!");
            }
            else
            {
                logger.LogNormal($"A {monsterName} attacks!");
            }

            return new Encounter(logger, monsters.ToArray());
        }

        private string RandomPlurality()
        {
            return this.plurality[this.random.Next(this.plurality.Length)];
        }

        private int GetMonsterHealth(int level, int numberOfMonsters)
        {
            return TuningKnobs.GetIdealHealthAtLevel(level) / numberOfMonsters;
        }

        private int GetMonsterDamagePerHit(int level, int numberOfMonsters)
        {
            return TuningKnobs.GetIdealDamageAtLevel(level) / numberOfMonsters;
        }

        private string GetMonsterName(int level)
        {
            return $"{GetAdjective(level)} {GetNoun()}";
        }

        private string GetNoun()
        {
            return this.possibleNouns[this.random.Next(this.possibleNouns.Length)];
        }

        private string GetAdjective(int level)
        {
            var array = this.possibleAdjectives[TuningKnobs.GetDifficultyForLevel(level)];
            var randomIndex = this.random.Next(array.Length);
            return array[randomIndex];
        }

        private int GetNumberOfMonsters()
        {
            return this.random.Next(4) + 1;
        }
    }
}
