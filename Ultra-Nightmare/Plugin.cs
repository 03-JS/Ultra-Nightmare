using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultra_Nightmare.Patches;

namespace Ultra_Nightmare
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.Patch)]
    internal class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "JS03.Ultra-Nightmare";
        private const string modName = "Ultra-Nightmare";
        private const string modVersion = "1.0.1";

        // Config values
        public static ConfigEntry<bool> automatedMsg;
        // public static ConfigEntry<bool> permaDeath;
        // public static ConfigEntry<float> automatedMsgDelay;

        private readonly Harmony harmony = new Harmony(modGUID);
        private static Plugin Instance;
        internal static ManualLogSource mls;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Ultra-Nightmare is up and running");

            //permaDeath = Config.Bind(
            //    "Ultra-Nightmare", // Config section
            //    "Permadeath", // Key of this config
            //    false, // Default value
            //    "Makes it so you will only revive after your crew gets fired.\nEnable this if you're clinically insane and want to suffer even more" // Description
            //);

            automatedMsg = Config.Bind(
                "Ultra-Nightmare", // Config section
                "Automated chat message", // Key of this config
                true, // Default value
                "Enables the automated message sent by VEGA" // Description
            );

            //automatedMsgDelay = Config.Bind(
            //    "Ultra-Nightmare", // Config section
            //    "Automated chat message delay", // Key of this config
            //    8f, // Default value
            //    "Applies a delay to the automated message" // Description
            //);

            PatchStuff();
        }

        internal void PatchStuff()
        {
            harmony.PatchAll(typeof(Plugin));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(TimeOfDayPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
        }

        public static void LogToConsole(string message, string logType = "")
        {
            switch (logType.ToLower())
            {
                case "warn":
                    mls.LogWarning(message);
                    break;
                case "error":
                    mls.LogError(message);
                    break;
                default:
                    mls.LogInfo(message);
                    break;
            }
        }
    }
}
