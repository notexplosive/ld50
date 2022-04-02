using System;
using Machina.Components;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Input;

namespace LD50.Gameplay
{
    public class ScrollButton : BaseComponent
    {
        public event Action ScrolledToNextPage;
        
        public ScrollButton(Actor actor) : base(actor)
        {
        }

        public override void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
        {
            if (key == Keys.Space && modifiers.None && state == ButtonState.Pressed)
            {
                ScrolledToNextPage?.Invoke();
            }
        }
    }
}
