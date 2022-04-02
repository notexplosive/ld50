using LD50.Gameplay;
using Machina.Data;
using Machina.Data.TextRendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LD50.Renderer
{
    public static class PostRenderer
    {
        public static void DrawTextContent(SpriteBatch spriteBatch, Rectangle textBounds, Post post, Depth depth)
        {
            var boundedText = new BoundedText(textBounds.Size, Alignment.Center, Overflow.Ignore, post.Content.Text);
            
            foreach (var item in boundedText.GetRenderedText())
            {
                item.Draw(spriteBatch, textBounds.Location, 0f, depth);


                if (item.Text.StartsWith('#'))
                {
                    var link = new Hyperlink(item);
                    spriteBatch.DrawRectangle(link.GetClickableRectangle(textBounds.Location), Color.White, 1f, depth);
                }
            }
        }

        public static Point EstimateSize(Post post, Point textBounds)
        {
            var boundedText = new BoundedText(textBounds, Alignment.Center, Overflow.Ignore, post.Content.Text);
            return boundedText.UsedSize;
        }
    }
}
