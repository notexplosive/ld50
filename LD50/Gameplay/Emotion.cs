namespace LD50.Gameplay
{
    public class Emotion
    {
        public string Name { get; }

        public static Emotion Mad = new Emotion("mad");
        public static Emotion Sad = new Emotion("sad");
        public static Emotion Laughing = new Emotion("laughing");
        public static Emotion Meh = new Emotion("meh");
        public static Emotion Happy = new Emotion("happy");

        private Emotion(string name)
        {
            Name = name;
        }
    }
}
