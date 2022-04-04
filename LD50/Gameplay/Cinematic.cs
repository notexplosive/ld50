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
            yield return new WaitSeconds(1f);
            Say("Ouch!");
            MachinaClient.SoundEffectPlayer.PlaySound("ouch");
            
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
    }
}
