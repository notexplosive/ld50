using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
    public class BattleLogger
    {
        private readonly Chat chat;

        public BattleLogger(Chat chat)
        {
            this.chat = chat;
        }

        public void LogNormal(string message)
        {
            if (this.chat == null)
            {
                return;
            }
            
            this.chat.LogCombatEvent(message);
        }
        
        public void LogHappy(string message)
        {
            if (this.chat == null)
            {
                return;
            }
            
            this.chat.AppendColoredString(message, Color.Yellow);
        }

        public void LogVictory()
        {
            if (this.chat == null)
            {
                return;
            }
            this.chat.AppendColoredString("Victory!", Color.Yellow);
        }

        public void LogStatus(string message)
        {
            if (this.chat == null)
            {
                return;
            }
            
            this.chat.AppendColoredString(message, Color.Cyan);
        }
    }
}
