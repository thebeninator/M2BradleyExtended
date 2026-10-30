using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MelonLoader;
using MelonLoader.Utils;
using Tomlet;
using Tomlet.Models;

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

        private Type toml_serializer_options_type;
        private Type[] toml_to_signature;
        private MethodInfo toml_to_method;

        public PresetManager(string preset_bundle_path)
        {
            this.preset_bundle_path = preset_bundle_path;
            PresetManagers.Add(this);

            string serializer_options = Assembly.CreateQualifiedName("Tomlet", "Tomlet.TomlSerializerOptions");
            toml_serializer_options_type = Type.GetType(serializer_options);
            toml_to_signature = new Type[] { typeof(TomlDocument) };

            if (toml_serializer_options_type != null)
            {
                toml_to_signature = new Type[] { typeof(TomlDocument), toml_serializer_options_type };
            }

            toml_to_method = typeof(TomletMain).GetMethod("To", toml_to_signature).MakeGenericMethod(typeof(T));
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

                    object[] arguments = new object[]{ toml_doc };
                    if (toml_serializer_options_type != null)
                    {
                        arguments = new object[] { toml_doc, null };
                    }

                    T preset = (T)toml_to_method.Invoke("", arguments);
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
