using System.Collections.Generic;
using System.Text;
using Machina.Data;
using Machina.Engine;

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
        private readonly string[] plurality =
        {
            "gang", "squad", "gaggle", "gathering", "posse", "school", "pod"
        };

        private readonly Dictionary<Difficulty, string[]> possibleAdjectives = new Dictionary<Difficulty, string[]>
        {
            {Difficulty.Easy, new[] {"Red", "Blue", "Green", "Yellow", "Sickly", "Wimpy", "Small", "Soft", "Stone"}},
            {
                Difficulty.Medium,
                new[]
                {
                    "Dark", "Large", "Fire", "Frost", "Hairy", "Spiky", "Angry", "Metal", "Shiny", "Chrome"
                }
            },
            {
                Difficulty.Hard,
                new[]
                {
                    "Mecha", "Huge", "Enchanted", "Giant", "Vermilion", "Celadon", "Amaranth", "Sapphire", "Ruby",
                    "Deadly", "Titanium", "Glowing", "Undead"
                }
            },
            {
                Difficulty.Boss,
                new[]
                {
                    "Ultimate Final", "Super Mega Ultra", "Diamond", "Perfect", "First",
                    "Forgotten", "Last", "Ancient", "Five Dimensional", "Multi-threaded"
                }
            }
        };

        private readonly string[] possibleNouns =
        {
            "Goblin", "Bandit", "Kobold", "Golem", "Snake", "Boar", "Wolf", "Trout", "Buff Cat", "Panther", "Lion",
            "Motorcycle", "Elemental", "Slime", "Squid", "Frog", "Toad", "Walking Mushroom", "Tree Person", "Spider",
            "Dragon", "Centaur", "Crab", "Chicken", "Bee"
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

            if (TuningKnobs.GetDifficultyForLevel(level) == Difficulty.Boss)
            {
                numberOfMonsters = 1;
            }

            var monsterName = GetMonsterName(level);

            for (var i = 0; i < numberOfMonsters; i++)
            {
                var monsterDamage = GetMonsterDamagePerHit(level, numberOfMonsters);
                monsters.Add(new Monster(GetMonsterSoundEffect(), GetMonsterHealth(level, numberOfMonsters), monsterDamage,
                    TuningKnobs.GetMonsterAttackDelay(level), monsterName));
            }

            return new Encounter(level, logger, monsters.ToArray());
        }

        private SoundEffects GetMonsterSoundEffect()
        {
            return new SoundEffects(GetHurtSoundEffect(), GetDealDamageSoundEffect(), GetDieSoundEffect());
        }

        private string GetDieSoundEffect()
        {
            return "monster-die";
        }

        private string GetDealDamageSoundEffect()
        {
            var array = new[] { "bass", "beatbox2", "blem", "clink", "blunthit" };
            return this.random.GetRandomElement(array);
        }

        private string GetHurtSoundEffect()
        {
            return "hurt_" + MachinaClient.RandomDirty.Next(1, 5);
        }

        public string RandomPlurality()
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
            if (TuningKnobs.GetDifficultyForLevel(level) == Difficulty.Boss)
            {
                return $"{GetBossName()}, The {GetAdjective(level)} {GetNoun()}";
            }

            return $"{GetAdjective(level)} {GetNoun()}";
        }

        private string GetBossName()
        {
            var consonants = new[] {'x', 'c', 'd', 'k', 'n', 'b', 'v', 'r', 'p', 'g'};
            var vowels = new[] {'a', 'e', 'o', 'u', '\''};

            this.random.Next(consonants.Length);

            string Consonant()
            {
                return consonants[this.random.Next(consonants.Length)].ToString();
            }

            string Vowel()
            {
                return vowels[this.random.Next(vowels.Length)].ToString();
            }

            var name = new StringBuilder();
            name.Append(Consonant().ToUpper());
            name.Append(Consonant());
            name.Append(Vowel());
            name.Append(Consonant());
            name.Append(Consonant());
            name.Append(Vowel());
            name.Append(Consonant());
            name.Append(Consonant());
            name.Append(Consonant());
            name.Append(Vowel());
            name.Append(Consonant());

            return name.ToString();
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
