using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;

namespace AlterTheFog
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.tinysquid.alterthefog";
        public const string PLUGIN_NAME = "alterthefog";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Harmony harmony;

        private static ConfigEntry<float> aggressiveness;

        private static ConfigEntry<float> initialLevel;
        private static ConfigEntry<float> initialGrowth;
        private static ConfigEntry<float> initialColonize;

        private static ConfigEntry<float> maxDensity;

        private static ConfigEntry<float> growthSpeedFactor;
        private static ConfigEntry<float> powerThreatFactor;
        private static ConfigEntry<float> battleThreatFactor;
        private static ConfigEntry<float> battleExpFactor;

        public void Awake()
        {
            LoadConfig();

            harmony = new Harmony(PLUGIN_GUID);

            ApplyPatch();
        }

        public void OnDestroy()
        {
            RevertPatch();
        }

        public void LoadConfig()
        {
            const string SECTION_KEY = "Difficulty";
            
            aggressiveness = Config.Bind(SECTION_KEY, "aggressiveness", 1f, "");

            initialLevel = Config.Bind(SECTION_KEY, "initialLevel", 1f, "");
            initialGrowth = Config.Bind(SECTION_KEY, "initialGrowth", 1f, "");
            initialColonize = Config.Bind(SECTION_KEY, "initialColonize", 1f, "");

            maxDensity = Config.Bind(SECTION_KEY, "maxDensity", 1f, "");
            
            growthSpeedFactor= Config.Bind(SECTION_KEY, "growthSpeedFactor", 1f, "");
            powerThreatFactor = Config.Bind(SECTION_KEY, "powerThreatFactor", 1f, "");
            battleThreatFactor = Config.Bind(SECTION_KEY, "battleThreatFactor", 1f, "");
            battleExpFactor = Config.Bind(SECTION_KEY, "battleExpFactor", 1f, "");

            Logger.LogInfo("Loaded configuration values.");
        }

        public void ApplyPatch()
        {
            try
            {
                harmony.PatchAll(typeof(PatchDarkFogSettings));
                Logger.LogInfo("Patch applied!");
            }
            catch (Exception e)
            {
                Logger.LogError("Patch failed with exception: " + e.ToString());
            }
        }

        public void RevertPatch()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(CombatSettings), nameof(CombatSettings.SetDefault))]
        // For new game starts it will use our configured plugin values and ignore the in-game setting
        class PatchDarkFogDefault
        {
            public static bool Prefix(ref CombatSettings __instance)
            {
                __instance.aggressiveness = aggressiveness.Value;
                __instance.initialLevel = initialLevel.Value;
                __instance.initialGrowth = initialGrowth.Value;
                __instance.initialColonize = initialColonize.Value;
                __instance.maxDensity = maxDensity.Value;
                __instance.growthSpeedFactor = growthSpeedFactor.Value;
                __instance.powerThreatFactor = powerThreatFactor.Value;
                __instance.battleThreatFactor = battleThreatFactor.Value;
                __instance.battleExpFactor = battleExpFactor.Value;

                return false;
            }
        }

        // For existing saves we have to override the export method so the next save write will reflect our plugin values on reload
        [HarmonyPatch(typeof(CombatSettings), nameof(CombatSettings.Export))]
        class PatchDarkFogSettings
        {
            public static bool Prefix(BinaryWriter w)
            {
                w.Write(0);
                w.Write(aggressiveness.Value);

                w.Write(initialLevel.Value);
                w.Write(initialGrowth.Value);
                w.Write(initialColonize.Value);

                w.Write(maxDensity.Value);

                w.Write(growthSpeedFactor.Value);
                w.Write(powerThreatFactor.Value);
                w.Write(battleThreatFactor.Value);
                w.Write(battleExpFactor.Value);

                return false;
            }
        }
    }
}
