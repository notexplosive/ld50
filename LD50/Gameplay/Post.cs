using System.Collections.Generic;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
    public class Post
    {
        public FormattedText Text { get; }
        
        public Post()
        {
            var fontMetrics = new SpriteFontMetrics(MachinaClient.DefaultStyle.uiElementFont);

            Text = new FormattedText(
                new FormattedTextFragment("I'm mad about ", fontMetrics, Color.White),
                new FormattedTextFragment("#BabyEarth", fontMetrics, Color.LightBlue));
        }
    }
}
