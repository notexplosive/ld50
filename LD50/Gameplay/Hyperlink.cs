using Machina.Data.TextRendering;
using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
    public class Hyperlink
    {
        private readonly Rectangle originalRectangle;

        public Hyperlink(RenderableText item)
        {
            this.originalRectangle = new Rectangle(item.Offset, item.Drawable.Size);
        }

        public Rectangle GetClickableRectangle(Point textPosition)
        {
            return new Rectangle(textPosition + this.originalRectangle.Location, this.originalRectangle.Size);
        }
    }
}
