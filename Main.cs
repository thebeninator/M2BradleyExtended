using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Audio;
using GHPC.Camera;
using GHPC.Player;
using GHPC.State;
using GHPC.Vehicle;
using M2BradleyExtended;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Mod), "M2 Bradley Extended", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M2BradleyExtended
{
    public class Mod : MelonMod
    {
        public static Vehicle[] vics;
        public static MelonPreferences_Category cfg;

        internal IEnumerator GetVics(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);
            yield break;
        }

        public override void OnInitializeMelon()
        {
            cfg = MelonPreferences.CreateCategory("M2Extended");
            M2Ext.Config(cfg);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!Assets.done && (sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene" || sceneName == "t64_menu"))
            {
                Assets.Load();
                Ammo.Init();
                IBAS.Init();
            }

            if (Util.menu_screens.Contains(sceneName)) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(GetVics), GameStatePriority.Medium);
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(M2Ext.Convert), GameStatePriority.Medium);
        }
    }
}
