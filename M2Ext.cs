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

namespace M2BradleyExtended
{
    internal sealed class M2Ext : Module
    {
        static MelonPreferences_Entry<bool> m2_patch;
        static MelonPreferences_Entry<bool> m2a2_armour_package;

        static MelonPreferences_Entry<bool> quickswap_bins;
        static MelonPreferences_Entry<bool> use_m919_apfsds;
        static MelonPreferences_Entry<string> tow_missile_type;
        static MelonPreferences_Entry<bool> has_enhanced_bushmaster;
        static MelonPreferences_Entry<bool> use_xm913;

        static MelonPreferences_Entry<bool> has_lrf;
        static MelonPreferences_Entry<bool> has_ibas;
        static MelonPreferences_Entry<bool> has_citv;
        static MelonPreferences_Entry<bool> green_thermals;
        static MelonPreferences_Entry<bool> clear_thermals;

        public static void Config(MelonPreferences_Category cfg) {
            m2_patch = cfg.CreateEntry<bool>("M2 Bradley Patch", true);

            m2a2_armour_package = cfg.CreateEntry<bool>("Addon Armour Package", true);
            m2a2_armour_package.Comment = "Addon 25.4mm steel plates for hull and turret. Increased weight.";

            use_m919_apfsds = cfg.CreateEntry<bool>("Use M919 APFSDS-T", true);
            use_m919_apfsds.Comment = "Increased penetration, velocity over M791 APDS-T";
            use_m919_apfsds.Description = "//////////////////////////////////////////////////////////////";

            tow_missile_type = cfg.CreateEntry<string>("TOW Missile", "TOW2");
            tow_missile_type.Comment = "Default, TOW2, TOW2A (anti-ERA)";

            has_lrf = cfg.CreateEntry<bool>("Has Laser Rangefinder", true);
            has_lrf.Comment = "Does NOT have automatic lead";
            has_lrf.Description = "//////////////////////////////////////////////////////////////";

            has_ibas = cfg.CreateEntry<bool>("Has IBAS", false);
            has_ibas.Comment = "Complete overhaul for day and thermal sight; has automatic lead, includes LRF";

            has_enhanced_bushmaster = cfg.CreateEntry<bool>("Enhanced M242 Bushmaster", false);
            has_enhanced_bushmaster.Comment = "Increases autocannon accuracy";

            //use_xm913 = cfg.CreateEntry<bool>("50mm XM913 Autocannon", false);
            //use_xm913.Comment = "Replaces the M242; features its own APFSDS and HEAB rounds";

            quickswap_bins = cfg.CreateEntry<bool>("Quick Refill Ammo Bins", false);
            quickswap_bins.Comment = "Reduces time to replenish autocannon ammo bins to 15 seconds";

            //has_citv = cfg.CreateEntry<bool>("Has CITV", false);
            //has_citv.Comment = "Gives commander their own thermal optic; ";
        }

        public static IEnumerator Convert(GameState _) {
            foreach (Vehicle vic in Mod.vics) {
                if (vic.GetComponent<AlreadyConverted>() != null) continue;
                if (vic.UniqueName != "M2BRADLEY" && vic.UniqueName != "M2BRADLEY(ALT)") continue;

                bool is_ap_heavy = vic.UniqueName == "M2BRADLEY(ALT)";

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

                string tow_type = tow_missile_type.Value.ToUpper();
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

                    tow_feed.AmmoTypeInBreech = null;
                    tow_feed.Start();
                }

                if (quickswap_bins.Value)
                {
                    GHPC.Weapons.AmmoRack main = loadout_manager.RackLoadouts[0].Rack;
                    main._retrievalDelaySeconds = 10f;
                    main._storageDelaySeconds = 5f;

                    GHPC.Weapons.AmmoRack reserve = loadout_manager.RackLoadouts[1].Rack;
                    reserve._retrievalDelaySeconds = 10f;
                    reserve._storageDelaySeconds = 5f;
                }

                if (has_enhanced_bushmaster.Value /*&& !use_xm913.Value*/)
                {
                    bushmaster.BaseDeviationAngle = 0.065f / 2f;
                    weapons_manager.Weapons[0].Name = "25mm cannon M242 enhanced";
                }

                if (has_ibas.Value)
                {
                    IBAS.Add(day_optic, bushmaster.FCS, bushmaster, tow, m240);
                    tow.GuidanceUnit.AimElement = day_optic.transform;

                    CustomGuidanceComputer cgc = bushmaster.FCS.gameObject.AddComponent<CustomGuidanceComputer>();
                    cgc.fcs = bushmaster.FCS;
                    cgc.mgu = tow.GuidanceUnit;
                }

                if (has_lrf.Value || has_ibas.Value)
                {
                    bushmaster.FCS.MaxLaserRange = 4000f;

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

                if (use_m919_apfsds.Value /*&& !use_xm913.Value*/)
                {
                    AmmoClipCodexScriptable ap_clip = is_ap_heavy ? Ammo.m919_230_clip_codex : Ammo.m919_70_clip_codex;

                    LoadoutManager.RackLoadout primary_loadout = loadout_manager.RackLoadouts[0];
                    primary_loadout.OverrideInitialClips[0] = ap_clip;
                    GHPC.Weapons.AmmoRack rack = primary_loadout.Rack;
                    rack.ClipTypes[0] = ap_clip.ClipType;
                    Util.EmptyRack(rack);

                    GHPC.Weapons.AmmoRack reserve = loadout_manager.RackLoadouts[1].Rack;
                    reserve.ClipTypes[0] = Ammo.m919_50_clip_codex.ClipType;
                    Util.EmptyRack(reserve);

                    loadout_manager.LoadedAmmoList.AmmoClips[0] = Ammo.m919_50_clip_codex;

                    loadout_manager.SpawnCurrentLoadout();
                    bushmaster.Feed.AmmoTypeInBreech = null;
                    bushmaster.Feed.LoadedClipType = null;
                    bushmaster.Feed.Start();
                    loadout_manager.RegisterAllBallistics();

                    bushmaster_feed.ExclusiveItems[0].AdditionalAmmo = new AmmoClipCodexScriptable[] { Ammo.m919_230_clip_codex, Ammo.m919_70_clip_codex, Ammo.m919_50_clip_codex };
                    bushmaster_feed.ToggleExclusiveItems(ap_clip.ClipType);
                }

                if (m2a2_armour_package.Value)
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

                    if (!has_ibas.Value)
                    {
                        turret_visual.Find("ibas").gameObject.SetActive(false);
                    }
                    else
                    {
                        new_mesh = Assets.m2_bradley_smr_cleaned_ibas;
                    }

                    Transform brake = mantlet_visual.Find("enhanced brake");
                    if (has_enhanced_bushmaster.Value /*! && use_xm913.Value*/)
                    {
                        brake.SetParent(vic.transform.Find("M2BRADLEY_rig/HULL/Turret/Mantlet/Main gun"), true);
                        new_mesh = has_ibas.Value ? Assets.m2_bradley_smr_cleaned_enhanced_ibas : Assets.m2_bradley_smr_cleaned_enhanced;
                    }
                    else
                    {
                        brake.gameObject.SetActive(false);
                    }

                    //if (!use_xm913.Value)
                    //{
                    mantlet_visual.Find("xm913").gameObject.SetActive(false);
                    //}

                    smr.sharedMesh = new_mesh;

                    armour_kit.GetComponent<HeatSource>().OnEnable();

                    turret_visual.SetParent(turret, true);
                    mantlet_visual.SetParent(mantlet, true);

                    turret_armour.SetParent(turret, true);
                    mantlet_armour.SetParent(mantlet, true);

                    LateFollow turret_armour_lf = turret_armour.gameObject.AddComponent<LateFollow>();
                    turret_armour_lf.FollowTarget = turret;
                    turret_armour_lf.ForceToRoot = true;
                    turret_armour_lf.enabled = true;
                    turret_armour_lf.Awake();

                    LateFollow mantlet_armour_lf = mantlet_armour.gameObject.AddComponent<LateFollow>();
                    mantlet_armour_lf.FollowTarget = turret;
                    mantlet_armour_lf.ForceToRoot = true;
                    mantlet_armour_lf.enabled = true;
                    mantlet_armour_lf.Awake();

                    LateFollow hull_armour_lf = hull_armour.gameObject.AddComponent<LateFollow>();
                    hull_armour_lf.FollowTarget = vic.transform;
                    hull_armour_lf.ForceToRoot = true;
                    hull_armour_lf.enabled = true;
                    hull_armour_lf.Awake();

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

                if (tow_type != "DEFAULT")
                {
                    vic._friendlyName = "M2A1";
                }

                if (m2a2_armour_package.Value)
                {
                    vic._friendlyName = "M2A2";

                    if (has_lrf.Value)
                    {
                        vic._friendlyName = "M2A2 ODS";
                    }

                    if (has_ibas.Value)
                    {
                        vic._friendlyName = "M2A2 ODS-SA";

                        //if (has_citv.Value)
                        //{
                        //    vic._friendlyName = "M2A3";
                        //}
                    }
                }
                else
                {
                }

                go.AddComponent<AlreadyConverted>();
            }

            yield break;
        }
    }
}
