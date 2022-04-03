namespace LD50.Gameplay
{
    public class BattleLogger
    {
        private readonly Chat chat;

        public BattleLogger(Chat chat)
        {
            this.chat = chat;
        }

        public void Log(string message)
        {
            this.chat.LogCombatEvent(message);
        }
    }
}
