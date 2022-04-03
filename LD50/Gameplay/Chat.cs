using System;
using System.Collections.Generic;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Gameplay
{
    public class Chat
    {
        public event Action ContentChanged;
        private List<FormattedText> messages = new List<FormattedText>();

        public SpriteFont GetFont()
        {
            return MachinaClient.Assets.GetSpriteFont("UIFont");
        }

        public void AppendColoredString(string message, Color color)
        {
            FormattedText formattedText = FormattedText.FromString(message, GetFont(), color);
            AppendFormattedString(formattedText);
        }

        public void PartyMemberSay(PartyMember partyMember, string message)
        {
            var partyMemberName = partyMember.Name;
            var metrics = new SpriteFontMetrics(GetFont());
            FormattedText formattedText = new FormattedText(new FormattedTextFragment($"[{partyMemberName}]: ", metrics, Color.LightBlue), new FormattedTextFragment(message, metrics, Color.White));
            
            AppendFormattedString(formattedText);
        }

        public void LogCombatEvent(string message)
        {
            AppendColoredString(message, Color.Orange);
        }

        public void AppendFormattedString(FormattedText formattedText)
        {
            this.messages.Add(formattedText);
            ContentChanged?.Invoke();
        }

        public List<FormattedText> AllMessages()
        {
            return this.messages;
        }
    }
}
