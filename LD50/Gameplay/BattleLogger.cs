﻿using Microsoft.Xna.Framework;

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
            if (this.chat == null)
            {
                return;
            }
            
            this.chat.LogCombatEvent(message);
        }

        public void LogVictory()
        {
            this.chat.AppendColoredString("Victory!", Color.Yellow);
        }
    }
}
