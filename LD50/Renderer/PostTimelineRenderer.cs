using System.Collections.Generic;
using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Renderer
{
    public class PostTimelineRenderer : BaseComponent
    {
        private readonly List<Post> posts;
        private readonly int screenWidth;

        public PostTimelineRenderer(Actor actor, int screenWidth) : base(actor)
        {
            this.posts = new List<Post>();

            var jake = new User("Jake", "Stevenson");
            var alice = new User("Alice", "Mumford");

            var rice = new Topic("rice");
            var cheese = new Topic("cheese");

            this.screenWidth = screenWidth;
            
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Sad, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Happy, cheese)));
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, cheese)));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var postLocation = Point.Zero;
            foreach (var post in this.posts)
            {
                PostRenderer.DrawPost(spriteBatch, post, postLocation.Y, this.screenWidth, transform.Depth);
                postLocation.Y += PostRenderer.EstimateSize(this.screenWidth).Y;
            }
        }
    }
}
