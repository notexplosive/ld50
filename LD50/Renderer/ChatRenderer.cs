using System.Collections.Generic;
using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD50.Renderer
{
    public class ChatRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Chat chat;
        private readonly List<PositionedText> currentTexts = new List<PositionedText>();
        private readonly NinepatchSheet ninepatch;

        public ChatRenderer(Actor actor, Chat chat) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.chat = chat;

            this.chat.ContentChanged += WhenContentChanged;

            this.ninepatch = MachinaClient.Assets.GetMachinaAsset<NinepatchSheet>("chat-patch");
        }

        private void WhenContentChanged()
        {
            var messages = this.chat.AllMessages();
            var usedHeight = this.boundingRect.Size.Y;
            var boundedTexts = new List<BoundedText>();

            for(int i = messages.Count - 1; i >= 0; i--)
            {
                var message = messages[i];
                var boundedText = new BoundedText(this.boundingRect.Size, Alignment.TopLeft, Overflow.Ignore, message);

                usedHeight -= boundedText.UsedSize.Y;
                if (usedHeight >= 0)
                {
                    boundedTexts.Add(boundedText);
                }
                else
                {
                    break;
                }
            }

            var currentHeight = this.boundingRect.Size.Y;
            this.currentTexts.Clear();

            foreach(var text in boundedTexts)
            {
                currentHeight -= text.UsedSize.Y;
                this.currentTexts.Add(
                    new PositionedText(text, this.boundingRect.Location + new Point(0, currentHeight)));
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            this.ninepatch.DrawFullNinepatch(spriteBatch, this.boundingRect.Rect, NinepatchSheet.GenerationDirection.Outer, transform.Depth + 100);
            
            foreach (var positionedText in this.currentTexts)
            {
                foreach (var item in positionedText.Text.GetRenderedText())
                {
                    item.Draw(spriteBatch, positionedText.Position, 0f, transform.Depth);
                }
            }
        }

        private readonly struct PositionedText
        {
            public BoundedText Text { get; }
            public Point Position { get; }

            public PositionedText(BoundedText text, Point position)
            {
                Text = text;
                Position = position;
            }
        }
    }
}
