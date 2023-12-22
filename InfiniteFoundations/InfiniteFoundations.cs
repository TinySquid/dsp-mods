using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;

namespace InfiniteFoundations
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.tinysquid.infinitefoundations";
        public const string PLUGIN_NAME = "infinitefoundations";
        public const string PLUGIN_VERSION = "1.0.0";

        private static Harmony harmony;

        private ConfigEntry<bool> shouldOverrideSoil;

        private ConfigEntry<bool> shouldOverrideFoundations;
        private static ConfigEntry<int> foundationItemId;

        public void Awake()
        {
            LoadConfig();

            harmony = new Harmony(PLUGIN_GUID);
            ApplyPatches();
        }

        public void OnDestroy()
        {
            RevertPatches();
        }

        public void LoadConfig()
        {
            shouldOverrideSoil = Config.Bind("General", "shouldOverrideSoil", true, "toggle for infinite soil");
            shouldOverrideFoundations = Config.Bind("General", "shouldOverrideFoundations", true, "toggle for infinite foundations");
            foundationItemId = Config.Bind("Other", "foundationItemId", 1131, "exposed item id for foundations if the game ever changes it for any reason. See https://dsp-wiki.com/Modding:Items_IDs");

            Logger.LogInfo("Loaded configuration values.");
        }

        public void ApplyPatches()
        { 
            try
            {
                if (shouldOverrideSoil.Value)
                {
                    harmony.PatchAll(typeof(PatchSandCount));
                }

                if (shouldOverrideFoundations.Value)
                {
                    harmony.PatchAll(typeof(PatchFoundationCount));
                }

                Logger.LogInfo("Patches applied!");
            }
            catch (Exception e)
            {
                Logger.LogError("Patch failed with exception: " + e.ToString());
            }
        }

        public void RevertPatches()
        {
            harmony.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Player), "sandCount", MethodType.Getter)]
        class PatchSandCount
        {
            static void Postfix(ref long __result)
            {
                __result = 999999;
            }
        }

        [HarmonyPatch(typeof(StorageComponent), nameof(StorageComponent.GetItemCount), new Type[] { typeof(int) })]
        class PatchFoundationCount
        {
            static void Postfix(int itemId, ref int __result)
            {
                if (itemId == foundationItemId.Value)
                {
                    __result = 9999;
                }
            }
        }
    }
}
