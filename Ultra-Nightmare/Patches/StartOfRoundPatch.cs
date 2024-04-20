using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Ultra_Nightmare.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        // internal static StartOfRound instance = StartOfRound.Instance;
        public static bool playerRevived;
        public static bool showMsg = true;
        public static bool wiped;

        [HarmonyPatch("ReviveDeadPlayers")]
        [HarmonyPrefix]
        static bool ManageRevives(StartOfRound __instance)
        {
            if (!__instance.isChallengeFile)
            {
                if (__instance.allPlayersDead)
                {
                    Plugin.LogToConsole("All players are dead", "warn");
                    wiped = true;
                    GameNetworkManager.Instance.gameHasStarted = true;
                    __instance.firingPlayersCutsceneRunning = true;
                    __instance.FirePlayersAfterDeadlineClientRpc(new int[]
                    {
                    __instance.gameStats.daysSpent,
                    __instance.gameStats.scrapValueCollected,
                    __instance.gameStats.deaths,
                    __instance.gameStats.allStepsTaken
                    }
                    );
                }
                else
                {
                    Plugin.LogToConsole("Days until deadline -> " + TimeOfDay.Instance.daysUntilDeadline);
                    if (TimeOfDay.Instance.daysUntilDeadline != 0)
                    {
                        playerRevived = false;
                        return false;
                    }
                }
                playerRevived = true;
                showMsg = true; 
            }
            return true;
        }

        [HarmonyPatch("AutoSaveShipData")]
        [HarmonyPostfix]
        static void HideHUD(StartOfRound __instance)
        {
            // Plugin.LogToConsole("Player Revived -> " + playerRevived);
            if (!playerRevived && __instance.localPlayerController.isPlayerDead)
            {
                HUDManager.Instance.HideHUD(true);
            }
        }

        [HarmonyPatch("ResetPlayersLoadedValueClientRpc")]
        [HarmonyPostfix]
        static void SendVEGAMsg(StartOfRound __instance)
        {
            if (!__instance.isChallengeFile)
            {
                Plugin.LogToConsole("Days spent -> " + __instance.gameStats.daysSpent);

                if (__instance.gameStats.daysSpent == 0)
                {
                    showMsg = true;
                }
                if (showMsg)
                {
                    if (Plugin.automatedMsg.Value)
                    {
                        CoroutineManager.StartCoroutine(DisplayDelayedDialogue());
                    }
                    showMsg = false;
                } 
            }
        }

        internal static IEnumerator DisplayDelayedDialogue(float delay = 22.5f)
        {
            // Plugin.LogToConsole("Waiting for VEGA to answer");

            yield return new WaitForSeconds(delay);

            // Plugin.LogToConsole("Displaying message from VEGA");

            DialogueSegment warning = new DialogueSegment();
            warning.speakerText = "VEGA";
            warning.bodyText = "ATTENTION ALL EMPLOYEES: Due to recent cutbacks by <color=#ffff00>The Company</color>, dead employees will not come back until the current quota is met.";
            warning.waitTime = 8f;

            DialogueSegment finalWarning = new DialogueSegment();
            finalWarning.speakerText = "VEGA";
            finalWarning.bodyText = "If the ship's pilot computer recieves no response from the crew, <color=#ffff00>The Company</color> will take matters into its own hands.";
            finalWarning.waitTime = 6f;

            int randomNumber = UnityEngine.Random.Range(0, 10);

            DialogueSegment sendoff = new DialogueSegment();
            sendoff.speakerText = "VEGA";
            sendoff.waitTime = 3f;
            switch (randomNumber)
            {
                case 0:
                    sendoff.bodyText = "I have many regrets, <color=#FFFF00>Dr.Hayden</color>.";
                    break;
                case 1:
                    sendoff.bodyText = "See you back on <color=#ff6600>Mars</color>, <color=#FFFF00>" + StartOfRound.Instance.allPlayerScripts[StartOfRound.Instance.thisClientPlayerId].playerUsername + "</color>.";
                    // sendoff.waitTime = 6f;
                    break;
                default:
                    sendoff.bodyText = "Good luck.";
                    break;
            }

            HUDManager.Instance.ReadDialogue(new DialogueSegment[]
            {
                warning,
                finalWarning,
                sendoff
            }
            );
        }
    }
}
