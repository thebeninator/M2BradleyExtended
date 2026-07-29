using System.Collections.Generic;
using System.IO;
using MelonLoader.Utils;
using Tomlet.Models;
using Tomlet;
using System;
using MelonLoader;

namespace Presets
{
    internal class PresetManager {
        public static List<PresetManager> PresetManagers = new List<PresetManager>();

        public virtual void LoadPresets() { }

        public static void LoadAllPresets()
        {
            foreach (PresetManager preset_manager in PresetManagers)
            {
                preset_manager.LoadPresets();
            }
        }
    }

    internal class PresetManager<T> : PresetManager
    {
        public T PlayerReservedPreset { get; set; }
        public bool HasPlayerReservedPreset { get; set; } = false;

        private List<T> loaded_presets = new List<T>();
        private List<T> preset_choice_pool = new List<T>();
        private string preset_bundle_path;

        public PresetManager(string preset_bundle_path)
        {
            this.preset_bundle_path = preset_bundle_path;
            PresetManagers.Add(this);
        }

        public override void LoadPresets()
        {
            string full_path = Path.Combine(MelonEnvironment.ModsDirectory, preset_bundle_path);

            try 
            {
                MelonLogger.Msg("loading preset bundle from path: " + preset_bundle_path);

                string[] preset_paths = Directory.GetFiles(full_path, "*.cfg");

                foreach (string preset_path in preset_paths)
                {
                    TomlDocument toml_doc = TomlParser.ParseFile(preset_path);
                    T preset = TomletMain.To<T>(toml_doc);
                    loaded_presets.Add(preset);

                    PresetTemplate t = preset as PresetTemplate;

                    int weight = t.Weight;
                    for (int i = 0; i < weight; i++)
                    {
                        preset_choice_pool.Add(preset);
                    }

                    if (t.GuaranteeForStartingUnit)
                    {
                        HasPlayerReservedPreset = true;
                        PlayerReservedPreset = preset;
                    }
                }

                foreach (T preset in loaded_presets)
                {
                    PresetTemplate t = preset as PresetTemplate;
                    string probablity = (1f * t.Weight / preset_choice_pool.Count * 100f).ToString("F2");
                    MelonLogger.Msg($"loaded preset <{t.Id}> with probabilty <{probablity}%>");
                }

                MelonLogger.Msg("\n\n");
            }
            catch (Exception e)
            {
                MelonLogger.Error(e);
            }
        }

        public T ChoosePreset()
        {
            return preset_choice_pool[UnityEngine.Random.Range(0, preset_choice_pool.Count - 1)];
        }
    }
}
