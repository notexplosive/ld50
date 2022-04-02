namespace LD50.Gameplay
{
    public class Topic : ITopic
    {
        private readonly string name;

        public Topic(string name)
        {
            this.name = name;
        }
        
        public string Slug()
        {
            return $"#{this.name}";
        }
    }
}
