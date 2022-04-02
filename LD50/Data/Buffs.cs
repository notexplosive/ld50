namespace LD50.Data
{
    public readonly struct Buffs
    {
        public Buffs GetNext(float dt)
        {
            return new Buffs();
        }

        public int GetHealingThisTick(float dt)
        {
            return 0;
        }

        public int GetDamageThisTick(float dt)
        {
            return 0;
        }
    }
}
