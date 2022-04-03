using System.Collections.Generic;
using Machina.Components;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class BattleSystem : BaseComponent
    {
        private readonly Party party;

        public BattleSystem(Actor actor, Party party) : base(actor)
        {
            this.party = party;
            actor.scene.StartCoroutine(BasicBattle());
        }

        private IEnumerator<ICoroutineAction> BasicBattle()
        {
            while (true)
            {
                yield return new WaitSeconds(1);
                this.party.GetMostThreateningPartyMember().TakeDamage(5);
            }
        }
    }
}
