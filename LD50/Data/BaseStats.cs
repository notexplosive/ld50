namespace LD50.Data
{
    public readonly struct BaseStats
    {
        public int MaxHealth { get; }
        public int MaxMana { get; }
        public int ManaRegenPerSecond { get; }

        public BaseStats(int maxHealth, int maxMana, int manaRegenPerSecond = 0)
        {
            MaxHealth = maxHealth;
            MaxMana = maxMana;
            ManaRegenPerSecond = manaRegenPerSecond;
        }
    }
}
