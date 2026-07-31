using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GHPC.State;
using GHPC.Vehicle;
using GHPC.Utility;
using MelonLoader;
using GHPC.Weapons;
using GHPC.Equipment.Optics;
using GHPC.Thermals;
using GHPC.Weaponry;
using ModUtil;
using M2BradleyExtended.Airburst;
using GHPC.Player;
using Presets;

namespace M2BradleyExtended
{
    internal sealed class M2Ext : Module
    {
        static PresetManager<M2Preset> preset_manager;
        static MelonPreferences_Entry<bool> use_preset_pool;
        static MelonPreferences_Entry<string> target_preset;

        static MelonPreferences_Entry<bool> m2_patch;
        static MelonPreferences_Entry<bool> m2a2_armour_package;

        static MelonPreferences_Entry<bool> quickswap_bins;
        static MelonPreferences_Entry<bool> use_m919_apfsds;
        static MelonPreferences_Entry<string> tow_missile_type;
        static MelonPreferences_Entry<int> limited_javelins;
        static MelonPreferences_Entry<string> javelin_alternate_msl_type;
        static MelonPreferences_Entry<bool> guarantee_player_javelin;
        static MelonPreferences_Entry<bool> has_enhanced_bushmaster;
        static MelonPreferences_Entry<bool> use_xm913;

        static MelonPreferences_Entry<bool> has_lrf;
        static MelonPreferences_Entry<bool> has_ibas;
        static MelonPreferences_Entry<bool> has_citv;
        static MelonPreferences_Entry<bool> green_thermals;
        static MelonPreferences_Entry<bool> clear_thermals;

        public static void Config(MelonPreferences_Category cfg) {
            m2_patch = cfg.CreateEntry<bool>("M2 Bradley Patch", true);
            use_preset_pool = cfg.CreateEntry<bool>("Use Preset Bundle", false);
            target_preset = cfg.CreateEntry<string>("Preset Bundle Path", "M2Extended/PresetBundles/Example");

            m2a2_armour_package = cfg.CreateEntry<bool>("Addon Armour Package", true);
            m2a2_armour_package.Comment = "Addon 25.4mm steel plates for hull and turret. Increased weight.";

            use_m919_apfsds = cfg.CreateEntry<bool>("Use M919 APFSDS-T", true);
            use_m919_apfsds.Comment = "Increased penetration, velocity over M791 APDS-T";
            use_m919_apfsds.Description = "//////////////////////////////////////////////////////////////";

            tow_missile_type = cfg.CreateEntry<string>("TOW Missile", "TOW2");
            tow_missile_type.Comment = "Default, TOW2, TOW2A (anti-ERA), TOWFF (FnF, anti-era), TOWFFMP (FnF)";

            has_lrf = cfg.CreateEntry<bool>("Has Laser Rangefinder", true);
            has_lrf.Comment = "Does NOT have automatic lead";
            has_lrf.Description = "//////////////////////////////////////////////////////////////";

            has_ibas = cfg.CreateEntry<bool>("Has IBAS", false);
            has_ibas.Comment = "Complete overhaul for day and thermal sight; has automatic lead, includes LRF";

            has_enhanced_bushmaster = cfg.CreateEntry<bool>("Enhanced M242 Bushmaster", false);
            has_enhanced_bushmaster.Comment = "Increases autocannon accuracy";

            use_xm913 = cfg.CreateEntry<bool>("50mm XM913 Autocannon", false);
            use_xm913.Comment = "Replaces the M242; comes with its own APFSDS and HEAB rounds";

            quickswap_bins = cfg.CreateEntry<bool>("Quick Refill Ammo Bins", false);
            quickswap_bins.Comment = "Reduces time to replenish autocannon ammo bins to 15 seconds";

            //has_citv = cfg.CreateEntry<bool>("Has CITV", false);
            //has_citv.Comment = "Gives commander their own thermal optic; ";
            if (use_preset_pool.Value)
            {
                preset_manager = new PresetManager<M2Preset>(target_preset.Value);
            }
        }

        private static void HandleConversion(Vehicle vic)
        {
            if (vic.UniqueName != "M2BRADLEY" && vic.UniqueName != "M2BRADLEY(ALT)") return;

            bool is_ap_heavy = vic.UniqueName == "M2BRADLEY(ALT)";
            bool player_controlled = vic.GetInstanceID() == PlayerInput.Instance.CurrentPlayerUnit.GetInstanceID();

            string tow_type = tow_missile_type.Value.ToUpper();
            bool cfg_m919 = use_m919_apfsds.Value;
            bool cfg_lrf = has_lrf.Value;
            bool cfg_ibas = has_ibas.Value;
            bool cfg_enhanced_bushmaster = has_enhanced_bushmaster.Value;
            bool cfg_xm913 = use_xm913.Value;
            bool cfg_quick_restock = quickswap_bins.Value;
            bool cfg_addon_armour = m2a2_armour_package.Value;

            if (use_preset_pool.Value)
            {
                M2Preset preset = preset_manager.ChoosePreset();

                if (preset_manager.HasPlayerReservedPreset && player_controlled)
                {
                    preset = preset_manager.PlayerReservedPreset;
                }

                tow_type = preset.TOWMissile.ToUpper();
                cfg_m919 = preset.M919;
                cfg_lrf = preset.LRF;
                cfg_ibas = preset.IBAS;
                cfg_enhanced_bushmaster = preset.EnhancedM242;
                cfg_xm913 = preset.XM913;
                cfg_quick_restock = preset.QuickRefillBins;
                cfg_addon_armour = preset.AddonArmour;
            }

            GameObject go = vic.gameObject;
            Transform rig = go.transform.Find("M2BRADLEY_rig/lp_hull005");
            Transform turret = go.transform.Find("M2BRADLEY_rig/HULL/Turret");
            Transform mantlet = go.transform.Find("M2BRADLEY_rig/HULL/Turret/Mantlet");

            LoadoutManager loadout_manager = vic.GetComponent<LoadoutManager>();
            WeaponsManager weapons_manager = vic.GetComponent<WeaponsManager>();
            WeaponSystem bushmaster = weapons_manager.Weapons[0].Weapon;
            WeaponSystem tow = weapons_manager.Weapons[1].Weapon;
            WeaponSystem m240 = weapons_manager.Weapons[2].Weapon;
            AmmoFeed bushmaster_feed = bushmaster.Feed;
            AmmoFeed tow_feed = tow.Feed;

            UsableOptic day_optic = go.transform.Find("M2BRADLEY_rig/HULL/Turret/GPS Optic").GetComponent<UsableOptic>();
            UsableOptic night_optic = day_optic.slot.LinkedNightSight.PairedOptic;

            Transform day_hud = day_optic.transform.Find("M2 Bradley GPS canvas/HUD elements");
            Transform night_hud = night_optic.transform.Find("M2 Bradley GPS canvas (1)/HUD elements");
            Transform[] huds = new Transform[] { day_hud, night_hud };

            if (cfg_xm913)
            {
                GameObject m913 = GameObject.Instantiate(Assets.m913_prefab, rig);
                m913.transform.SetParent(mantlet);
                m913.transform.localEulerAngles = Vector3.zero;

                bushmaster.BaseDeviationAngle = 0.065f / 2.2f;
                bushmaster.WeaponSound.SingleShotEventPaths[0] = "event:/Weapons/canon_73mm-2A28Grom";
                bushmaster.Impulse = 7500f;
                bushmaster.RecoilBlurMultiplier = 1.55f;
                bushmaster.Feed._totalCycleTime = 0.4f;
                bushmaster.transform.localPosition = new Vector3(0.0826f, 0.0085f, 2.6239f);

                AnimatedPart gun_animator = bushmaster.Feed.RoundCycleStages[0].AnimatedParts[0];
                gun_animator.StartTransform = m913.transform.Find("brake start");
                gun_animator.EndTransform = m913.transform.Find("brake end");
                gun_animator.Transform = m913.transform.Find("brake");

                weapons_manager.Weapons[0].Name = "50mm cannon M913";

                AirburstManager airburst_manager = bushmaster.gameObject.AddComponent<AirburstManager>();
                airburst_manager.AmmoKeyIdx = 1;
                airburst_manager.AirburstAmmo = Ammo.xm1204_round_codex.AmmoType;
            }

            if (cfg_quick_restock)
            {
                GHPC.Weapons.AmmoRack main = loadout_manager.RackLoadouts[0].Rack;
                main._retrievalDelaySeconds = 10f;
                main._storageDelaySeconds = 5f;

                GHPC.Weapons.AmmoRack reserve = loadout_manager.RackLoadouts[1].Rack;
                reserve._retrievalDelaySeconds = 10f;
                reserve._storageDelaySeconds = 5f;
            }

            if (cfg_enhanced_bushmaster && !cfg_xm913)
            {
                bushmaster.BaseDeviationAngle = 0.065f / 2f;
                weapons_manager.Weapons[0].Name = "25mm cannon M242 enhanced";
            }

            if (cfg_lrf || cfg_ibas)
            {
                bushmaster.FCS.MaxLaserRange = 4000f;
                GameObject laser_ref_point = new GameObject("laser ref");
                laser_ref_point.transform.SetParent(bushmaster.FCS.LaserOrigin);
                laser_ref_point.transform.localEulerAngles = Vector3.zero;
                laser_ref_point.transform.localPosition = new Vector3(0f, 0f, 2f);
                bushmaster.FCS.LaserOrigin = laser_ref_point.transform;

                day_optic.RangeTextArchetype = "0000";
                day_optic.RangeTextDivideBy = 1;
                night_optic.RangeTextArchetype = "0000";
                night_optic.RangeTextDivideBy = 1;

                foreach (Transform hud in huds)
                {
                    hud.Find("tow selected").localPosition = new Vector3(-40f * 2f, -173f, 0f);
                    hud.Find("762 selected").localPosition = new Vector3(40f * 2f, -173f, 0f);
                    hud.Find("autocannon ammo types/AP selected").localPosition = new Vector3(60f * 2f, -173f, 0f);
                    hud.Find("autocannon ammo types/HE selected").localPosition = new Vector3(-60f * 2f, -173f, 0f);
                }
            }

            if (cfg_ibas)
            {
                IBAS.Add(day_optic, bushmaster.FCS, bushmaster, tow, m240);
                weapons_manager.Weapons[1].MuzzleAngleOffset = Vector3.zero;

                //CustomGuidanceComputer cgc = bushmaster.FCS.gameObject.AddComponent<CustomGuidanceComputer>();
                //cgc.fcs = bushmaster.FCS;
                //cgc.mgu = tow.GuidanceUnit;

                tow.GuidanceUnit.AimElement = bushmaster.FCS.LaserOrigin;
            }

            if (cfg_m919 || cfg_xm913)
            {
                AmmoClipCodexScriptable reserve_ap_clip = cfg_xm913 ? Ammo.xm1203_50_clip_codex : Ammo.m919_50_clip_codex;
                AmmoClipCodexScriptable reg_ap_clip = cfg_xm913 ? Ammo.xm1203_50_clip_codex : Ammo.m919_70_clip_codex;
                AmmoClipCodexScriptable heavy_ap_clip = cfg_xm913 ? Ammo.xm1203_170_clip_codex : Ammo.m919_230_clip_codex;

                AmmoClipCodexScriptable ap_clip = is_ap_heavy ? heavy_ap_clip : reg_ap_clip;
                AmmoClipCodexScriptable he_clip_m913 = is_ap_heavy ? Ammo.xm1204_50_clip_codex : Ammo.xm1204_170_clip_codex;

                LoadoutManager.RackLoadout primary_loadout = loadout_manager.RackLoadouts[0];
                GHPC.Weapons.AmmoRack rack = primary_loadout.Rack;

                primary_loadout.OverrideInitialClips[0] = ap_clip;
                rack.ClipTypes[0] = ap_clip.ClipType;

                GHPC.Weapons.AmmoRack reserve = loadout_manager.RackLoadouts[1].Rack;
                reserve.ClipTypes[0] = reserve_ap_clip.ClipType;

                if (cfg_xm913)
                {
                    bushmaster.Fired += bushmaster.AddProgrammedFuse;
                    primary_loadout.OverrideInitialClips[1] = he_clip_m913;
                    rack.ClipTypes[1] = he_clip_m913.ClipType;
                    loadout_manager.LoadedAmmoList.AmmoClips[1] = Ammo.xm1204_50_clip_codex;

                    bushmaster_feed.ExclusiveItems[1].AdditionalAmmo = new AmmoClipCodexScriptable[]
                    {
                        Ammo.xm1204_170_clip_codex,
                        Ammo.xm1204_50_clip_codex,
                    };
                }

                loadout_manager.LoadedAmmoList.AmmoClips[0] = reserve_ap_clip;
                Util.EmptyRack(rack);
                Util.EmptyRack(reserve);

                loadout_manager.SpawnCurrentLoadout();
                bushmaster.Feed.AmmoTypeInBreech = null;
                bushmaster.Feed.LoadedClipType = null;
                bushmaster.Feed.Start();
                loadout_manager.RegisterAllBallistics();

                bushmaster_feed.ExclusiveItems[0].AdditionalAmmo = new AmmoClipCodexScriptable[]
                {
                    Ammo.m919_230_clip_codex,
                    Ammo.m919_70_clip_codex,
                    Ammo.m919_50_clip_codex ,
                    Ammo.xm1203_170_clip_codex,
                    Ammo.xm1203_50_clip_codex,
                };

                bushmaster_feed.ToggleExclusiveItems(ap_clip.ClipType);
            }

            if (cfg_addon_armour)
            {
                vic.GetComponent<Rigidbody>().mass = 32600f;
                SkinnedMeshRenderer smr = rig.GetComponent<SkinnedMeshRenderer>();
                Mesh new_mesh = Assets.m2_bradley_smr_cleaned;

                GameObject armour_kit = GameObject.Instantiate(Assets.m2a2_armour_kit, rig);
                armour_kit.transform.localEulerAngles = new Vector3(0f, 90f, 90f);

                Transform mantlet_visual = armour_kit.transform.Find("mantlet");
                Transform turret_visual = armour_kit.transform.Find("turret");

                Transform turret_armour = armour_kit.transform.Find("armour/turret");
                Transform mantlet_armour = turret_armour.Find("mantlet");
                Transform hull_armour = armour_kit.transform.Find("armour/hull");

                if (!cfg_ibas)
                {
                    turret_visual.Find("ibas").gameObject.SetActive(false);
                }
                else
                {
                    new_mesh = Assets.m2_bradley_smr_cleaned_ibas;
                }

                Transform brake = mantlet_visual.Find("enhanced brake");
                if (cfg_enhanced_bushmaster && !cfg_xm913)
                {
                    brake.SetParent(vic.transform.Find("M2BRADLEY_rig/HULL/Turret/Mantlet/Main gun"), true);
                    new_mesh = cfg_ibas ? Assets.m2_bradley_smr_cleaned_enhanced_ibas : Assets.m2_bradley_smr_cleaned_enhanced;
                }
                else
                {
                    brake.gameObject.SetActive(false);
                }

                smr.sharedMesh = new_mesh;

                armour_kit.GetComponent<HeatSource>().OnEnable();

                turret_visual.SetParent(turret, true);
                mantlet_visual.SetParent(mantlet, true);

                turret_armour.SetParent(turret.GetComponent<LateFollowTarget>().LateFollowers[0].transform, true);
                mantlet_armour.SetParent(mantlet.GetComponent<LateFollowTarget>().LateFollowers[0].transform, true);
                hull_armour.SetParent(vic.GetComponent<LateFollowTarget>().LateFollowers[0].transform, true);
                GameObject.Destroy(armour_kit.transform.Find("armour").gameObject);

                Transform original_hull_armour_lft = go.GetComponent<LateFollowTarget>()._lateFollowers[0].transform;
                Transform original_hull_armour = original_hull_armour_lft.GetChild(2);
                original_hull_armour.Find("UnknownMaterialAndThickness (WT says Alu 1\")").gameObject.SetActive(false);
                original_hull_armour.Find("Fording ramp Steel 0.25\"?").gameObject.SetActive(false);
                Transform og_sides = original_hull_armour.Find("Hull Sides Alu 7039");
                og_sides.GetComponent<MeshFilter>().sharedMesh = Assets.m2_bradley_hull_side_modified;
                og_sides.GetComponent<MeshCollider>().sharedMesh = Assets.m2_bradley_hull_side_modified;

                Transform og_sides_applique = original_hull_armour.Find("Hull Sides Hard Steel 0.25\"");
                og_sides_applique.GetComponent<MeshFilter>().sharedMesh = Assets.m2_bradley_hull_side_applique_modified;
                og_sides_applique.GetComponent<MeshCollider>().sharedMesh = Assets.m2_bradley_hull_side_applique_modified;

                Transform original_turret_armour_lft = turret.GetComponent<LateFollowTarget>()._lateFollowers[0].transform;
                Transform original_turret_armour = original_turret_armour_lft.GetChild(3);
                original_turret_armour.Find("Turret Storage Basket Hard Steel 0.25\"").gameObject.SetActive(false);
            }

            if (tow_type != null && tow_type != "DEFAULT")
            {
                GHPC.Weapons.AmmoRack tow_rack = tow_feed.ReadyRack;
                AmmoType.AmmoClip tow_clip = Ammo.tow_missiles[tow_type].ClipType;

                tow_rack.ClipTypes[0] = tow_clip;
                tow_rack.StoredClips = new List<AmmoType.AmmoClip>()
                {
                    tow_clip,
                    tow_clip,
                    tow_clip,
                    tow_clip,
                    tow_clip
                };

                if (tow_type.Contains("TOWFF"))
                {
                    tow.GuidanceUnit.GuidanceStarted -= tow.GuidanceUnit.OnGuidanceStarted;
                    tow.GuidanceUnit.GuidanceStopped -= tow.GuidanceUnit.OnGuidanceStopped;
                    //tow.GuidanceUnit = null;
                    tow.WireGuided = false;
                    //tow.Feed._missileGuidance = null;
                    tow.Feed.ReloadDuringMissileTracking = true;
                    tow.TriggerHoldTime = 0.5f;
                    AmmoType ammo_dir = tow_type == "TOWFFMP" ? Ammo.towff_mp_dir_ammo : Ammo.towff_dir_ammo;
                    Javelin.Add(day_optic, bushmaster.FCS, bushmaster, tow, has_ibas.Value, ammo_dir);
                }

                tow_feed.AmmoTypeInBreech = null;
                tow_feed.Start();
            }

            if (tow_type != "DEFAULT")
            {
                vic._friendlyName = "M2A1";
            }

            if (cfg_addon_armour)
            {
                vic._friendlyName = "M2A2";

                if (cfg_lrf)
                {
                    vic._friendlyName = "M2A2 ODS";
                }

                if (cfg_ibas)
                {
                    vic._friendlyName = "M2A2 ODS-SA";

                    //if (has_citv.Value)
                    //{
                    //    vic._friendlyName = "M2A3";
                    //}
                }
            }

            if (cfg_xm913)
            {
                if (vic._friendlyName == "M2A2 ODS-SA")
                {
                    vic._friendlyName = "M2A2+ HMCWS";
                }
                else
                {
                    vic._friendlyName += " HMCWS";
                }
            }
        }

        public static IEnumerator Convert(GameState _) {
            foreach (Vehicle vic in Mod.vics) 
            {
                HandleConversion(vic);
            }

            yield break;
        }
    }
}
