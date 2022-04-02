using System.Runtime.CompilerServices;
using LD50.Renderer;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class SpellInterface
    {
        public static void CreateFromActor(Actor actor, SpellCaster spellCaster)
        {
            new SpellRenderer(actor, spellCaster);
        }
    }
}
