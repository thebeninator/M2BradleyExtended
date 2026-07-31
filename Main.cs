using System.Collections;
using System.Linq;
using GHPC.State;
using GHPC.Vehicle;
using M2BradleyExtended;
using MelonLoader;
using UnityEngine;
using ModUtil;
using Presets;

[assembly: MelonInfo(typeof(Mod), "M2 Bradley Extended", "0.9.5B", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M2BradleyExtended
{
    public class Mod : MelonMod
    {
        private ModuleManager module_manager;
        public static Vehicle[] vics;
        public static MelonPreferences_Category cfg;
        private int valid_scene_count = 0;

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

            PresetManager.LoadAllPresets();

            //M2PresetTemplate template = new M2PresetTemplate();
            //string toml_string = TomletMain.TomlStringFrom(template);

            //File.WriteAllText(Path.Combine(MelonEnvironment.ModsDirectory + "/M2Extended/Presets", "template.cfg"), toml_string);
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

            valid_scene_count++;

            if (valid_scene_count == 2)
            {
                StateController.RunOrDefer(GameState.PlayerReady, new GameStateEventHandler(OnGameReady), GameStatePriority.Medium);
                StateController.RunOrDefer(GameState.PlayerReady, new GameStateEventHandler(M2Ext.Convert), GameStatePriority.Medium);
                StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Ammo.SetupEraOptimizations), GameStatePriority.Lowest);
                valid_scene_count = 0;
            }
        }
    }
}
