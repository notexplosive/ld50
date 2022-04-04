namespace LD50.Data
{
    public readonly struct EmptyBuff : IBuff
    {
        public int FrameIndex => -1;

        public static IBuff Create()
        {
            return new EmptyBuff();
        }

        public float RemainingDuration => 0;
        
        public IBuff GetNext(float dt)
        {
            return this;
        }

        public int GetHealAmount(float dt)
        {
            return 0;
        }
    }
}
