using System.Collections.Generic;
using LD50.Gameplay;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Renderer
{
    public class PostTimelineRenderer : BaseComponent
    {
        private readonly List<Post> posts;

        public PostTimelineRenderer(Actor actor) : base(actor)
        {
            this.posts = new List<Post>();

            var jake = new User("Jake", "Stevenson");
            var alice = new User("Alice", "Mumford");

            var rice = new Topic("rice");
            var cheese = new Topic("cheese");
            
            
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Sad, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Happy, cheese)));
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, cheese)));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var textBounds = new Point(100, 100);
            var postLocation = Point.Zero;
            foreach (var post in this.posts)
            {
                PostRenderer.DrawTextContent(spriteBatch, new Rectangle(postLocation, textBounds), post, transform.Depth);
                postLocation.Y += PostRenderer.EstimateSize(post, textBounds).Y;
            }
        }
    }
}
