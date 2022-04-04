using LD50.Gameplay;
using Xunit;

namespace LD50Tests
{
    public class MonsterMakerSanityTests
    {
        [Fact]
        public void monster_maker_monster_maker_make_me_a_monster()
        {
            var monsterMaker = new MonsterMaker(1234);
            var level1 = monsterMaker.CreateEncounter(1, new BattleLogger(null));
            var level2 = monsterMaker.CreateEncounter(2, new BattleLogger(null));
            var level3 = monsterMaker.CreateEncounter(3, new BattleLogger(null));
            var level4 = monsterMaker.CreateEncounter(4, new BattleLogger(null));
            var level5 = monsterMaker.CreateEncounter(5, new BattleLogger(null));
            var level6 = monsterMaker.CreateEncounter(6, new BattleLogger(null));
        }
    }
}
