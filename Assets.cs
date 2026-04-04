using GHPC;
using GHPC.Camera;
using GHPC.Vehicle;
using MelonLoader.Utils;
using System.IO;
using System.Linq;
using UnityEngine;
using GHPC.Weaponry;
using ModUtil;
using GHPC.Equipment;

namespace M2BradleyExtended
{
    internal sealed class Assets : Module
    {
        public static Mesh m2_bradley_smr_cleaned;
        public static Mesh m2_bradley_smr_cleaned_enhanced;
        public static Mesh m2_bradley_smr_cleaned_enhanced_ibas;
        public static Mesh m2_bradley_smr_cleaned_ibas;
        public static Mesh m2_bradley_smr_cleaned_sheridan;
        public static Mesh m2_bradley_hull_side_modified;
        public static Mesh m2_bradley_hull_side_applique_modified;
        public static GameObject m2a2_armour_kit;
        public static GameObject m2_sheridan_kit;

        public static GameObject ibas_hud;
        public static TMPro.TMP_FontAsset ibas_font;

        public static ArmorCodexScriptable US_generic_hhs;

        public static AmmoCodexScriptable m791_round_codex;
        //public static AmmoClipCodexScriptable m791_70_clip_codex;
        //public static AmmoClipCodexScriptable m791_50_clip_codex;
        //public static AmmoClipCodexScriptable m791_230_clip_codex;

        public static AmmoCodexScriptable itow_round_codex;

        public static AmmoCodexScriptable atow_round_codex;

        public static AmmoCodexScriptable br412d_round_codex;

        public static Material flir_blit_mat_green;

        public static GameObject muzzle_flash_125;

        public static AssetBundle m2_extended_assets;
        public static AssetBundle sheridan_kit_assets;

        public override void LoadStaticAssets()
        {
            //m791_70_clip_codex = Resources.FindObjectsOfTypeAll<AmmoClipCodexScriptable>().Where(o => o.name == "clip_M791_70rd_load").First();
            //m791_50_clip_codex = Resources.FindObjectsOfTypeAll<AmmoClipCodexScriptable>().Where(o => o.name == "clip_M791_50rd_box").First();
            //m791_230_clip_codex = Resources.FindObjectsOfTypeAll<AmmoClipCodexScriptable>().Where(o => o.name == "clip_M791_230rd_load").First();
            //atow_round_codex = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_TOW").First();

            //br412d_round_codex = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_BR-412D").First();

            //GameObject t64 = Resources.FindObjectsOfTypeAll<Vehicle>().Where(o => o.name == "T64A 1981").First().gameObject;
            //GameObject _125mm_muzzle_flash = t64.transform.Find("---MAIN GUN SCRIPTS---/125mm Gun 2A46M/GameObject/105mm Muzzle Flash").gameObject;
            //_125mm_muzzle_flash.SetActive(false);
            //muzzle_flash_125 = GameObject.Instantiate(t64.transform.Find("---MAIN GUN SCRIPTS---/125mm Gun 2A46M/GameObject/105mm Muzzle Flash").gameObject);
            //_125mm_muzzle_flash.SetActive(true);

            m2_extended_assets = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/M2Extended", "m2assets"));
            sheridan_kit_assets = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/M2Extended", "sheridan_kit"));

            m2_bradley_smr_cleaned = m2_extended_assets.LoadAsset<Mesh>("m2_smr_clean.asset");
            m2_bradley_smr_cleaned_enhanced = m2_extended_assets.LoadAsset<Mesh>("m2_smr_clean_enhanced.asset");

            m2_bradley_smr_cleaned_enhanced_ibas = m2_extended_assets.LoadAsset<Mesh>("m2_smr_clean_enhanced_ibas.asset");
            m2_bradley_smr_cleaned_ibas = m2_extended_assets.LoadAsset<Mesh>("m2_smr_clean_ibas.asset");

            m2_bradley_hull_side_modified = m2_extended_assets.LoadAsset<Mesh>("Hull_Sides_Alu_7039_mod.asset");
            m2_bradley_hull_side_applique_modified = m2_extended_assets.LoadAsset<Mesh>("Hull_Sides_Hard_Steel_0_25_.007_mod.asset");

            m2a2_armour_kit = m2_extended_assets.LoadAsset<GameObject>("m2a2 kit.prefab");
            Transform m2a2_armour = m2a2_armour_kit.transform.Find("armour");
            Util.CreateUniformArmour(m2a2_armour.Find("turret/turret").gameObject, "addon turret plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("turret/mantlet").gameObject, "addon mantlet plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("turret/turret/bustle").gameObject, "turret bustle rack", 5f, 5);

            Util.CreateUniformArmour(m2a2_armour.Find("hull/ufp").gameObject, "addon upper glacis plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/lfp").gameObject, "addon lower glacis plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/turret ring").gameObject, "addon turret collar", 25.4f, 25.4f, US_generic_hhs);

            Util.CreateUniformArmour(m2a2_armour.Find("hull/troop plate").gameObject, "addon hull side plate", 10f, 10f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/upper side").gameObject, "addon side plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/middle side").gameObject, "addon side plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/lower side").gameObject, "addon side plate", 25.4f, 25.4f, US_generic_hhs);
            Util.CreateUniformArmour(m2a2_armour.Find("hull/mounting brackets").gameObject, "mounting bracket", 6f, 6f, US_generic_hhs);

            Util.SetupFLIRShaders(m2a2_armour_kit);

            m2_bradley_smr_cleaned_sheridan = sheridan_kit_assets.LoadAsset<Mesh>("m2_smr_clean_sheridan.asset");
            m2_sheridan_kit = sheridan_kit_assets.LoadAsset<GameObject>("sheridan kit.prefab");

            ibas_hud = m2_extended_assets.LoadAsset<GameObject>("ibas hud.prefab");

            US_generic_hhs = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
            US_generic_hhs.name = "us gen hhs";
            ArmorType hhs = new ArmorType();
            hhs.BHN = 389f;
            hhs.CanRicochet = true;
            hhs.CanShatterLongRods = true;
            hhs.Name = "High hardness steel";
            hhs.NormalizesHits = true;
            hhs.RhaeMultiplierCe = 1f;
            hhs.RhaeMultiplierKe = 1f;
            hhs.SpallAngleMultiplier = 1f;
            hhs.SpallPowerMultiplier = 1f;
            hhs.ThicknessSource = ArmorType.RhaSource.BHN;
            US_generic_hhs.ArmorType = hhs;
        }

        public override void LoadDynamicAssets()
        {
            if (AssetUtil.VehicleInMission("M2 Bradley") || AssetUtil.VehicleInMission("M2 Bradley(AP heavy belt temp) Variant")) 
            {
                if (m791_round_codex == null)
                {
                    AssetUtil.LoadVanillaVehicle("M2BRADLEY"); // force load the codices immediately
                    m791_round_codex = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_25mm_M791_APDS").First();
                    itow_round_codex = Resources.FindObjectsOfTypeAll<AmmoCodexScriptable>().Where(o => o.name == "ammo_I-TOW").First();
                    Ammo.Init();
                    IBAS.Init();
                }

                Vehicle m60a3 = AssetUtil.LoadVanillaVehicle("M60A3TTS");
                flir_blit_mat_green = m60a3.transform.Find("Turret Scripts/Sights/FLIR").GetComponent<CameraSlot>().FLIRBlitMaterialOverride;

                ibas_font = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>().Where(o => o.name == "VCR_OSD_MONO_1 green").First();

                foreach (TMPro.TextMeshProUGUI text in ibas_hud.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
                {
                    text.font = ibas_font;
                }
            }
        }
    }
}
