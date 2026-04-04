using System.Collections;
using System.Linq;
using GHPC.State;
using GHPC.Vehicle;
using M2BradleyExtended;
using MelonLoader;
using UnityEngine;
using ModUtil;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(Mod), "M2 Bradley Extended", "0.9.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M2BradleyExtended
{
    public class Mod : MelonMod
    {
        public static Vehicle[] vics;
        public static MelonPreferences_Category cfg;
        public static Dictionary<string, Module> modules = new Dictionary<string, Module>();

        internal IEnumerator OnGameReady(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            foreach (string id in modules.Keys)
            {
                Module module = modules[id];
                bool loaded = module.TryLoadDynamicAssets();

                if (loaded)
                {
                    MelonLogger.Msg("M2Ext dynamic assets loaded from module: " + id);
                }
            }

            yield break;
        }

        public override void OnInitializeMelon()
        {
            cfg = MelonPreferences.CreateCategory("M2Extended");
            M2Ext.Config(cfg);

            modules.Add("Assets", new Assets());
            modules.Add("M2Ext", new M2Ext());
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            foreach (string id in modules.Keys)
            {
                Module module = modules[id];
                bool dynamic_unloaded = module.TryUnloadDynamicAssets();

                if (dynamic_unloaded)
                {
                    MelonLogger.Msg("M2Ext dynamic assets unloaded from module: " + id);
                }
            }

            if (sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene" || sceneName == "t64_menu")
            {
                foreach (string id in modules.Keys)
                {
                    Module module = modules[id];
                    bool static_loaded = module.TryLoadStaticAssets();

                    if (static_loaded)
                    {
                        MelonLogger.Msg("M2Ext static assets loaded from module: " + id);
                    }
                }

                AssetUtil.ReleaseVanillaAssets();
            }

            if (Util.menu_screens.Contains(sceneName)) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(OnGameReady), GameStatePriority.Medium);
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(M2Ext.Convert), GameStatePriority.Medium);
        }
    }
}
