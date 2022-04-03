using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
    public class SpellLogger
    {
        private readonly Chat chat;

        public SpellLogger(Chat chat)
        {
            this.chat = chat;
        }

        public void Log(string message)
        {
            this.chat.AppendColoredString(message, Color.LightBlue);
        }
    }
}
