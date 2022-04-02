using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
    public class PostContent
    {
        public PostContent(Emotion emotion, ITopic topic)
        {
            var fontMetrics = new SpriteFontMetrics(MachinaClient.DefaultStyle.uiElementFont);

            Text = new FormattedText(
                new FormattedTextFragment($"I'm {emotion.Name} about ", fontMetrics, Color.White),
                new FormattedTextFragment($"{topic.Slug()}", fontMetrics, Color.LightBlue));
        }

        public FormattedText Text { get; }
    }
}
