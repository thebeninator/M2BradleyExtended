using System.Collections;
using System.Linq;
using GHPC.State;
using GHPC.Vehicle;
using M2BradleyExtended;
using MelonLoader;
using UnityEngine;
using ModUtil;

[assembly: MelonInfo(typeof(Mod), "M2 Bradley Extended", "0.9.1B", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M2BradleyExtended
{
    public class Mod : MelonMod
    {
        private ModuleManager module_manager;
        public static Vehicle[] vics;
        public static MelonPreferences_Category cfg;

        internal IEnumerator OnGameReady(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            module_manager.LoadAllDynamicAssets();

            yield break;
        }

        public override void OnInitializeMelon()
        {
            cfg = MelonPreferences.CreateCategory("M2Extended");
            M2Ext.Config(cfg);

            module_manager = new ModuleManager("M2Ext");

            module_manager.Add("Assets", new Assets());
            module_manager.Add("M2Ext", new M2Ext());
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            module_manager.UnloadAllDynamicAssets();


            if (sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene" || sceneName == "t64_menu")
            {
                module_manager.LoadAllStaticAssets();
                AssetUtil.ReleaseVanillaAssets();
            }

            if (Util.menu_screens.Contains(sceneName)) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(OnGameReady), GameStatePriority.Medium);
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(M2Ext.Convert), GameStatePriority.Medium);
        }
    }
}
