﻿using BepInEx.Bootstrap;
using HarmonyLib;

namespace Ultra_Nightmare.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {
        [HarmonyPatch("DisplayDaysLeft")]
        [HarmonyPrefix]
        static bool ShowDaysLeft()
        {
            Plugin.LogToConsole("All players dead? -> " + StartOfRoundPatch.wiped);
            if (StartOfRoundPatch.wiped)
            {
                StartOfRoundPatch.wiped = false;
                return false;
            }
            return true;
        }

        [HarmonyPatch("ApplyPenalty")]
        [HarmonyPrefix]
        static bool ApplyPenalty()
        {
            if (Plugin.deathQuotaInstalled) return true;
            return false;
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void DisableHUDObject()
        {
            HUDManager.Instance.statsUIElements.penaltyTotal.transform.parent.parent.gameObject.SetActive(Chainloader.PluginInfos.ContainsKey("lacrivoca.DeathQuota"));
        }
    }
}
