using System;
using System.Collections.Generic;
using LD50.Gameplay;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Machina.ThirdParty;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD50.Renderer
{
    public class PostTimelineRenderer : BaseComponent
    {
        private readonly List<Post> posts;
        private readonly int screenWidth;
        private readonly TweenChain scrollTween;
        private Point scrolledPosition;
        private readonly TweenAccessors<int> scrollSpeed;

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

            this.scrollSpeed = new TweenAccessors<int>(0);
            this.scrollTween = new TweenChain();
            this.scrollTween.AppendIntTween(60, 0.25f, EaseFuncs.QuadraticEaseOut, scrollSpeed);
            this.scrollTween.AppendIntTween(0, 0.25f, EaseFuncs.QuadraticEaseIn, scrollSpeed);
            
            this.scrollTween.SkipToEnd();
            
            this.scrolledPosition = Point.Zero;
        }

        public override void Update(float dt)
        {
            this.scrollTween.Update(dt);
            this.scrolledPosition -= new Point(0, this.scrollSpeed.CurrentValue);
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var postLocation = this.scrolledPosition;
            foreach (var post in this.posts)
            {
                var height = PostRenderer.EstimateSize(this.screenWidth).Y;
                if (postLocation.Y + height > 0)
                {
                    PostRenderer.DrawPost(spriteBatch, post, postLocation.Y, this.screenWidth, transform.Depth);
                }

                postLocation.Y += height;
            }
        }

        public void AddNewPosts()
        {
            var jake = new User("Hannah", "Morgan");
            var alice = new User("Mike", "Fritz");

            var rice = new Topic("lemon");
            var cheese = new Topic("cream");
            
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Sad, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Happy, cheese)));
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, cheese)));
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Sad, rice)));
            this.posts.Add(new Post(alice, new PostContent(Emotion.Happy, cheese)));
            this.posts.Add(new Post(jake, new PostContent(Emotion.Happy, cheese)));
        }

        public void AnimateScroll()
        {
            this.scrollTween.Refresh();
        }
    }
}
