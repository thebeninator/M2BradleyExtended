using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GHPC.State;
using GHPC.Vehicle;
using M2BradleyExtended;
using GHPC.Utility;
using MelonLoader;
using GHPC.Weapons;

namespace M2BradleyExtended
{
    internal sealed class M2Ext
    {
        static MelonPreferences_Entry<bool> m2_patch;
        static MelonPreferences_Entry<bool> m2a2_armour_package;

        static MelonPreferences_Entry<bool> use_m919_apfsds;
        static MelonPreferences_Entry<string> tow_missile_type;

        static MelonPreferences_Entry<bool> has_lrf;
        static MelonPreferences_Entry<bool> has_ibas;
        static MelonPreferences_Entry<bool> has_citv;
        static MelonPreferences_Entry<bool> green_thermals;
        static MelonPreferences_Entry<bool> clear_thermals;

        public static void Config(MelonPreferences_Category cfg) {
            m2_patch = cfg.CreateEntry<bool>("M2 Bradley Patch", true);

            m2a2_armour_package = cfg.CreateEntry<bool>("Addon Armour Package", true);
            m2a2_armour_package.Comment = "Addon 25.4mm steel plates for hull and turret";

            use_m919_apfsds = cfg.CreateEntry<bool>("Use M919 APFSDS-T", true);

            has_lrf = cfg.CreateEntry<bool>("Laser Rangefinder", true);
        }

        public static IEnumerator Convert(GameState _) {
            foreach (Vehicle vic in Mod.vics) {
                if (vic.GetComponent<AlreadyConverted>() != null) continue;
                if (vic.UniqueName != "M2BRADLEY" && vic.UniqueName != "M2BRADLEY(ALT)") continue;

                GameObject go = vic.gameObject;
                Transform rig = go.transform.Find("M2BRADLEY_rig/lp_hull005");
                Transform turret = go.transform.Find("M2BRADLEY_rig/HULL/Turret");
                Transform mantlet = go.transform.Find("M2BRADLEY_rig/HULL/Turret/Mantlet");

                LoadoutManager loadout_manager = vic.GetComponent<LoadoutManager>();
                WeaponsManager weapons_manager = vic.GetComponent<WeaponsManager>();
                WeaponSystem bushmaster = weapons_manager.Weapons[0].Weapon;

                if (has_lrf.Value) {
                    bushmaster.FCS.MaxLaserRange = 4000f;
                }

                if (use_m919_apfsds.Value) {
                    // TODO: replace reserve bins
                    loadout_manager.LoadedAmmoTypes[0] = Ammo.m919_70_clip_codex;

                    GHPC.Weapons.AmmoRack rack = loadout_manager.RackLoadouts[0].Rack;
                    loadout_manager.RackLoadouts[0].OverrideInitialClips[0] = Ammo.m919_70_clip_codex;
                    rack.ClipTypes[0] = Ammo.m919_70_clip_codex.ClipType;
                    Util.EmptyRack(rack);

                    loadout_manager.SpawnCurrentLoadout();
                    bushmaster.Feed.AmmoTypeInBreech = null;
                    bushmaster.Feed.LoadedClipType = null;
                    bushmaster.Feed.Start();
                    loadout_manager.RegisterAllBallistics();
                }

                if (m2a2_armour_package.Value)
                {
                    GameObject armour_kit = GameObject.Instantiate(Assets.m2a2_armour_kit, rig);
                    armour_kit.transform.localEulerAngles = new Vector3(0f, 90f, 90f);

                    Transform mantlet_visual = armour_kit.transform.Find("mantlet");
                    Transform turret_visual = armour_kit.transform.Find("turret");

                    Transform turret_armour = armour_kit.transform.Find("armour/turret");
                    Transform mantlet_armour = turret_armour.Find("mantlet");
                    Transform hull_armour = armour_kit.transform.Find("armour/hull");

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
                    og_sides.GetComponent<MeshFilter>().mesh = Assets.m2_bradley_hull_side_modified;
                    og_sides.GetComponent<MeshFilter>().sharedMesh = Assets.m2_bradley_hull_side_modified;
                    og_sides.GetComponent<MeshCollider>().sharedMesh = Assets.m2_bradley_hull_side_modified;

                    Transform og_sides_applique = original_hull_armour.Find("Hull Sides Hard Steel 0.25\"");
                    og_sides_applique.GetComponent<MeshFilter>().mesh = Assets.m2_bradley_hull_side_applique_modified;
                    og_sides_applique.GetComponent<MeshFilter>().sharedMesh = Assets.m2_bradley_hull_side_applique_modified;
                    og_sides_applique.GetComponent<MeshCollider>().sharedMesh = Assets.m2_bradley_hull_side_applique_modified;

                    Transform original_turret_armour_lft = turret.GetComponent<LateFollowTarget>()._lateFollowers[0].transform;
                    Transform original_turret_armour = original_turret_armour_lft.GetChild(3);
                    original_turret_armour.Find("Turret Storage Basket Hard Steel 0.25\"").gameObject.SetActive(false);

                    SkinnedMeshRenderer smr = rig.GetComponent<SkinnedMeshRenderer>();
                    smr.sharedMesh = Assets.m2_bradley_smr_cleaned;
                }

                if (m2a2_armour_package.Value) {
                    vic._friendlyName = "M2A2";

                    if (has_lrf.Value) {
                        vic._friendlyName = "M2A2 ODS";
                    }
                }

                go.AddComponent<AlreadyConverted>();
            }

            yield break;
        }
    }
}
