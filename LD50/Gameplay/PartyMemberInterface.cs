using LD50.Renderer;
using Machina.Components;
using Machina.Engine;

namespace LD50.Gameplay
{
    public static class PartyMemberInterface
    {
        public static void CreateFromActor(Actor actor, PartyMember partyMember, SpellCaster spellCaster)
        {
            var hoverable = new Hoverable(actor);
            new PartyMemberRenderer(actor, partyMember, spellCaster);
            new PartyMemberUpdater(actor, partyMember);

            spellCaster.AddPartyMemberInterface(actor, hoverable, partyMember);
        }
    }
}
