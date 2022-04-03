using System.Collections.Generic;
using Machina.Components;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public class BattleSystem : BaseComponent
    {
        public Encounter CurrentEncounter { get; private set; }
        private readonly Party party;

        public BattleSystem(Actor actor, Party party) : base(actor)
        {
            this.party = party;
            CurrentEncounter = new Encounter();
        }

        public void StartNewEncounter(Encounter encounter)
        {
            CurrentEncounter = encounter;
            
            CurrentEncounter.StartCoroutines(actor.scene, party);
        }

        public IEnumerator<ICoroutineAction> CombatLoopCoroutine()
        {
            while (true)
            {
                var encounter = new Encounter(
                    new Monster(100, 10, 2f),
                    new Monster(100, 10, 2f));
                StartNewEncounter(encounter);
                yield return new WaitUntil(CurrentEncounter.IsFightOver);
                yield return new WaitSeconds(5);
            }
        }
    }
}
