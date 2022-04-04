using System;

namespace LD50.Gameplay
{
    public static class TuningKnobs
    {
        public static int GetIdealHealthAtLevel(int level)
        {
            // essentially a function of how long the battle lasts
            return 1000 + level * 100;
        }
        
        public static float GetIdealDelayAtLevel(int level)
        {
            return Math.Max(3f - level / 10f, 0.25f);
        }
        
        public static Difficulty GetDifficultyForLevel(int level)
        {
            if (level < 5)
            {
                return Difficulty.Easy;
            }

            if (level < 10)
            {
                return Difficulty.Medium;
            }

            return Difficulty.Hard;
        }
        
        public static int GetIdealDamageAtLevel(int level)
        {
            return 10 + level * 2;
        }
        
        public static float GetMonsterAttackDelay(int level, int numberOfMonsters)
        {
            return TuningKnobs.GetIdealDelayAtLevel(level) * numberOfMonsters;
        }
    }
}
