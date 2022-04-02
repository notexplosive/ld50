using System.Collections.Generic;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;

namespace LD50.Gameplay
{
    public class Post
    {
        public Post(User author, PostContent postContent)
        {
            Content = postContent;
            Author = author;
        }

        public User Author { get; }
        public PostContent Content { get; }
    }
}
