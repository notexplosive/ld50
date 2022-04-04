using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using Machina.Data;
using Machina.Engine;

namespace LD50.Gameplay
{
    public static class Cinematic
    {
        public static IEnumerator<ICoroutineAction> TutorialIntro(Chat chat, Party party, BattleSystem battle)
        {
            var advisor = party.GetMember(0);
            
            void Say(string message)
            {
                chat.PartyMemberSay(advisor, message);
            }
            
            yield return new WaitSeconds(1f);
            Say("Hey, looks like you're pretty new to this whole Healing gig.");
            
            yield return new WaitSeconds(5f);
            Say("No biggy, I'll show you the ropes.");
            
            yield return new WaitSeconds(5f);
            Say("You should see your spells in the bottom right of your screen.");
            
            yield return new WaitSeconds(2.5f);
            Say("Note that each spell is numbered 1 through 5.");

            yield return new WaitSeconds(5f);
            advisor.TakeDamage(15);
            chat.LogCombatEvent("The Advisor punches themself in the face.");
            yield return new WaitSeconds(0.25f);
            Say("Ouch!");
            
            yield return new WaitSeconds(5f);
            Say("OK, now that I've taken some damage, go ahead and heal me by hovering over my portrait and pressing 1.");

            yield return new WaitUntil(() => advisor.Status.IsFullyHealed);
            Say("Good enough!");
            
            yield return new WaitSeconds(5f);
            Say("Keep in mind: clicking does nothing. But you're welcome to click to your hearts content!");
            
            yield return new WaitSeconds(5f);
            chat.LogCombatEvent("The Advisor throws a grenade, damaging the dummies.");
            
            party.GetMember(1).TakeDamage(30);
            party.GetMember(2).TakeDamage(30);
            party.GetMember(3).TakeDamage(30);
            
            yield return new WaitSeconds(5f);
            Say("Oops! That did a bit more damage than I thought it would.");
            
            yield return new WaitSeconds(5f);
            Say("Go ahead and heal everyone back up, feel free to experiment with your different spells");

            yield return new WaitSeconds(2.5f);
            Say("Oh, by the way. You can press Escape to cancel out of the current spell your casting.");
            
            yield return new WaitUntil(() =>
            {
                foreach (var member in party.AllLivingMembers())
                {
                    if (!member.Status.IsFullyHealed)
                    {
                        return false;
                    }
                }

                return true;
            });

            yield return new WaitSeconds(5f);
            Say("Great, now we'll leave combat, which will revive the dummy I killed earlier.");
            
            yield return new WaitSeconds(2f);
            party.LeaveCombat();
            battle.FinishEncounter();

            yield return new WaitUntil(party.IsFullyRegenerated);

            yield return new WaitSeconds(5f);
            Say("You may have noticed the sword icons in the dummy's portraits.");
            
            yield return new WaitSeconds(5f);
            Say("That indicates that they're damage dealers. They deal most of the damage to monsters.");
            
            yield return new WaitSeconds(5f);
            Say("I'm a Tank, that means I attract the monsters' attention and soak up hits.");
            
            yield return new WaitSeconds(5f);
            Say("And you're well... a healer. I think you understand what that means at this point.");
            
            yield return new WaitSeconds(5f);
            Say("Whelp. That's about it. Good luck out there!");
        }

        public static IEnumerator<ICoroutineAction> GetCinematicForLevel(Chat chat, Party party, int level)
        {
            if (!SeenCinematicForLevel.ContainsKey(level))
            {

                void TankSay(string message)
                {
                    chat.PartyMemberSay(party.GetMember(0), message);
                }

                void MageSay(string message)
                {
                    chat.PartyMemberSay(party.GetMember(1), message);
                }

                void RogueSay(string message)
                {
                    chat.PartyMemberSay(party.GetMember(2), message);
                }

                void DruidSay(string message)
                {
                    chat.PartyMemberSay(party.GetMember(3), message);
                }

                WaitSeconds ShortDelay()
                {
                    return new WaitSeconds(3);
                }

                WaitSeconds NormalDelay()
                {
                    return new WaitSeconds(5);
                }

                WaitSeconds LongDelay()
                {
                    return new WaitSeconds(6);
                }

                if (level == 0)
                {
                    RogueSay("ugh,, finally found a group, i've been waiting the queue for ages");
                    yield return NormalDelay();
                    TankSay("Are the queue times that bad?");
                    yield return ShortDelay();
                    TankSay("How long were you waiting?");
                    yield return NormalDelay();
                    RogueSay("since JANUARY!!!");
                    yield return NormalDelay();
                    DruidSay("It's harder for damage dealers to find groups.");
                    yield return NormalDelay();
                    DruidSay("You could just switch to healer you know");
                    yield return NormalDelay();
                    RogueSay("heck no?? healing sux");
                    yield return NormalDelay();
                    RogueSay("no offense healer");
                    yield return LongDelay();
                    TankSay("I'm... gonna go ahead and pull.");
                }

                if (level == 1)
                {
                    DruidSay("brb! doorbell");
                    yield return ShortDelay();
                    TankSay("No prob, we'll wait.");
                    yield return NormalDelay();
                    RogueSay("srsly? we could just go without her");
                    yield return NormalDelay();
                    TankSay("Without her damage output, the fight might go on too long.");
                    yield return NormalDelay();
                    TankSay("Then the healer will run out of mana and I'll die");
                    yield return NormalDelay();
                    TankSay("And then we'll all die.");
                    yield return NormalDelay();
                    RogueSay("ya but");
                    yield return ShortDelay();
                    RogueSay("its not like she was doing THAT much damage");
                    yield return NormalDelay();
                    MageSay("... she did more damage than you that fight.");
                    yield return ShortDelay();
                    DruidSay("Back!");
                }

                if (level == 2)
                {
                    RogueSay("hang on");
                    yield return ShortDelay();
                    RogueSay("gotta take out the trash");
                    yield return LongDelay();
                    MageSay($"Hey {party.GetMember(3).Name}");
                    yield return ShortDelay();
                    DruidSay("Hey what?");
                    yield return ShortDelay();
                    MageSay("How come you're queued as Damage Dealer?");
                    yield return NormalDelay();
                    DruidSay("What do you mean?");
                    yield return NormalDelay();
                    MageSay("Your class can be any role");
                    yield return ShortDelay();
                    MageSay("Why choose the long queue times?");
                    yield return NormalDelay();
                    DruidSay("Eh, being healer is stressful");
                    yield return NormalDelay();
                    DruidSay("I get enough of that in my day job");
                    yield return ShortDelay();
                    MageSay("What do you do?");
                    yield return NormalDelay();
                    DruidSay("I'm a surgeon");
                    yield return ShortDelay();
                    RogueSay("back");
                }

                if (level == 4)
                {
                    TankSay("Good work team!");
                    yield return NormalDelay();
                    TankSay("It only gets tougher from here!");
                    yield return NormalDelay();
                }

                if (TuningKnobs.GetDifficultyForLevel(level) == Difficulty.Boss)
                {
                    if (TuningKnobs.GetDifficultyForLevel(level - 1) != Difficulty.Boss)
                    {
                        TankSay("OK. The next room is the boss. Focus up!");
                        yield return NormalDelay();
                    }
                    else
                    {
                        TankSay("WE DID IT!");
                        yield return NormalDelay();
                        MageSay("Sorry to break character but...");
                        yield return NormalDelay();
                        MageSay("Thanks for playing! That's the game!");
                        yield return NormalDelay();
                        MageSay("This was made for Ludum Dare 50.");
                        yield return NormalDelay();
                        MageSay("With the theme: Delay the Inevitable");
                        yield return NormalDelay();
                        MageSay("It is now safe to turn off the game.");
                        yield return NormalDelay();
                        TankSay("Or we can fight another boss ;)");
                        yield return NormalDelay();
                        RogueSay("or you can play the creator's other games at notexplosive.net ;^)");
                        yield return LongDelay();
                        TankSay("Well... I'm gonna pull the next one");
                    }

                }
            }

            SeenCinematicForLevel[level] = true;
        }

        public static readonly Dictionary<int, bool> SeenCinematicForLevel = new Dictionary<int, bool>();
    }
}
