using GHPC;
using MelonLoader.Utils;
using System.IO;
using System.Linq;
using UnityEngine;

namespace M2BradleyExtended
{
    internal sealed class Assets
    {
        public static bool done = false;
        public static Mesh m2_bradley_smr_cleaned;
        public static Mesh m2_bradley_hull_side_modified;
        public static Mesh m2_bradley_hull_side_applique_modified;
        public static GameObject m2a2_armour_kit;
        public static ArmorCodexScriptable US_generic_hhs;
        public static AmmoCodexScriptable m791_round_codex;
        public static AmmoClipCodexScriptable m791_70_clip_codex;
        public static AmmoClipCodexScriptable m791_50_clip_codex;
        public static AmmoClipCodexScriptable m791_230_clip_codex;

        internal static void Load() {
            US_generic_hhs = Resources.FindObjectsOfTypeAll<ArmorCodexScriptable>().Where(o => o.name == "US generic HHS").First();
            m791_round_codex = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_25mm_M791_APDS").First();
            m791_70_clip_codex = Resources.FindObjectsOfTypeAll<AmmoClipCodexScriptable>().Where(o => o.name == "clip_M791_70rd_load").First();
            m791_50_clip_codex = Resources.FindObjectsOfTypeAll<AmmoClipCodexScriptable>().Where(o => o.name == "clip_M791_50rd_box").First();
            m791_230_clip_codex = Resources.FindObjectsOfTypeAll<AmmoClipCodexScriptable>().Where(o => o.name == "clip_M791_230rd_load").First();

            AssetBundle m2_extended_assets = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/M2Extended", "m2assets"));
            m2_bradley_smr_cleaned = m2_extended_assets.LoadAsset<Mesh>("m2_smr_clean.asset");
            m2_bradley_hull_side_modified = m2_extended_assets.LoadAsset<Mesh>("Hull_Sides_Alu_7039_mod.asset");
            m2_bradley_hull_side_applique_modified = m2_extended_assets.LoadAsset<Mesh>("Hull_Sides_Hard_Steel_0_25_.007_mod.asset");

            m2a2_armour_kit = m2_extended_assets.LoadAsset<GameObject>("m2a2 kit.prefab");

            Transform m2a2_armour = m2a2_armour_kit.transform.Find("armour");
            Util.CreateUniformArmour(m2a2_armour.Find("turret/turret").gameObject, "addon turret plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("turret/turret/bustle").gameObject, "turret bustle rack", 5f, 5, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("turret/mantlet").gameObject, "addon mantlet plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/mounting brackets").gameObject, "mounting bracket", 6f, 6f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/troop plate").gameObject, "addon plate", 10f, 10f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/ufp").gameObject, "addon upper glacis plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/lfp").gameObject, "addon lower glacis plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/upper side").gameObject, "addon upper side plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/middle side").gameObject, "addon middle side plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/lower side").gameObject, "addon lower side plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/turret ring").gameObject, "addon turret collar", 25.4f, 25.4f, US_generic_hhs);

            Util.SetupFLIRShaders(m2a2_armour_kit);
            done = true;
        }
    }
}
