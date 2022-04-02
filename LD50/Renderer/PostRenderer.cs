using LD50.Gameplay;
using Machina.Data;
using Machina.Data.Layout;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace LD50.Renderer
{
    public static class PostRenderer
    {
        public static void DrawTextContent(SpriteBatch spriteBatch, Rectangle textBounds, PostContent content,
            Depth depth)
        {
            var boundedText = new BoundedText(textBounds.Size, Alignment.Center, Overflow.Ignore, content.Text);

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

        public static RawLayout GetPostLayout(int screenWidth)
        {
            return LayoutNode.VerticalParent("root", LayoutSize.Pixels(screenWidth, 200),
                new LayoutStyle(new Point(10, 10)),
                LayoutNode.VerticalParent("inner", LayoutSize.StretchedBoth(), LayoutStyle.Empty,
                    LayoutNode.Leaf("title-band", LayoutSize.StretchedHorizontally(24)),
                    LayoutNode.Leaf("body", LayoutSize.StretchedBoth()),
                    LayoutNode.HorizontalParent("button-band", LayoutSize.StretchedHorizontally(50),
                        new LayoutStyle(padding: 10),
                        LayoutNode.Leaf("agree-button", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("disagree-button", LayoutSize.StretchedBoth())
                    ))
            );
        }

        public static void DrawPost(SpriteBatch spriteBatch, Post post, int yPos, int screenWidth, Depth depth)
        {
            var font = MachinaClient.DefaultStyle.uiElementFont;

            var layout = PostRenderer.GetPostLayout(screenWidth);

            var bakedLayout = layout.Bake();
            var bodyNode = bakedLayout.GetNode("body");

            var textBounds = bodyNode.Rectangle;
            textBounds.Location += new Point(0, yPos);

            var bodyBounds = bakedLayout.GetNode("inner").Rectangle;
            bodyBounds.Location += new Point(0, yPos);

            var usernameBounds = bakedLayout.GetNode("title-band").Rectangle;
            usernameBounds.Location += new Point(0, yPos);

            var agreeButton = bakedLayout.GetNode("agree-button").Rectangle;
            agreeButton.Location += new Point(0, yPos);
            
            var disagreeButton = bakedLayout.GetNode("disagree-button").Rectangle;
            disagreeButton.Location += new Point(0, yPos);
            
            spriteBatch.DrawString(font, post.Author.Username, usernameBounds.Location.ToVector2(), Color.White);
            spriteBatch.DrawRectangle(bodyBounds, Color.White, 1f, depth);
            
            spriteBatch.DrawRectangle(agreeButton, Color.White, 1f, depth);
            spriteBatch.DrawRectangle(disagreeButton, Color.White, 1f, depth);

            PostRenderer.DrawTextContent(spriteBatch, textBounds, post.Content, depth);
        }

        public static Point EstimateSize(int screenWidth)
        {
            return GetPostLayout(screenWidth).Bake().GetNode("root").Size;
        }
    }
}
