namespace LD50.Data
{
    public class Cooldown
    {
        private readonly float total;
        private float current;

        public Cooldown(float total)
        {
            this.total = total;
            this.current = 0;
        }

        public bool IsInstant => this.total == 0;

        public void Update(float dt)
        {
            if (this.current > 0)
            {
                this.current -= dt;
            }
            else
            {
                this.current = 0;
            }
        }

        public void Start()
        {
            this.current = this.total;
        }

        public bool IsReady()
        {
            return this.current <= 0;
        }

        public float RemainingTime()
        {
            return this.current;
        }

        public float Percent()
        {
            if (IsReady())
            {
                return 0f;
            }
            
            return this.current / this.total;
        }
    }
}
