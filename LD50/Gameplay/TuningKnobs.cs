using System;

namespace LD50.Gameplay
{
    public static class TuningKnobs
    {
        public static int GetIdealHealthAtLevel(int level)
        {
            // essentially a function of how long the battle lasts

            var total = 500 + level * 25;
            
            if (GetDifficultyForLevel(level) == Difficulty.Boss)
            {
                total += 200;
            }

            return total;
        }
        
        public static float GetIdealDelayAtLevel(int level)
        {
            var ideal = 3f - (level + 3) / 5f;
            return Math.Max(ideal, 0.25f);
        }
        
        public static Difficulty GetDifficultyForLevel(int level)
        {
            if (level < 1)
            {
                return Difficulty.Easy;
            }

            if (level < 4)
            {
                return Difficulty.Medium;
            }

            if (level < 6)
            {
                return Difficulty.Hard;
            }

            return Difficulty.Boss;
        }
        
        public static int GetIdealDamageAtLevel(int level)
        {
            return 24 + level / 2;
        }
        
        public static float GetMonsterAttackDelay(int level)
        {
            return TuningKnobs.GetIdealDelayAtLevel(level);
        }
    }
}
