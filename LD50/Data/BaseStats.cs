namespace LD50.Data
{
    public readonly struct BaseStats
    {
        public int MaxHealth { get; }
        public int MaxMana { get; }

        public BaseStats(int maxHealth, int maxMana)
        {
            MaxHealth = maxHealth;
            MaxMana = maxMana;
        }
    }
}
