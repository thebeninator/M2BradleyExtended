using System.Collections.Generic;
using System.Linq;
using GHPC;
using GHPC.Equipment.Optics;
using GHPC.Weapons;
using M2BradleyExtended.FNF;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace M2BradleyExtended
{
    internal class Javelin
    {
        public static void Add(UsableOptic day_optic, FireControlSystem fcs,
            WeaponSystem bushmaster, WeaponSystem tow, bool has_ibas, AmmoType ammo_dir
        ) {
            UsableOptic night_optic = day_optic.slot.LinkedNightSight.PairedOptic;
            Transform flir_canvas = night_optic.transform.Find("M2 Bradley GPS canvas (1)");

            if (!has_ibas)
            {
                GameObject.Destroy(day_optic.AdditionalReticleMeshes[0].gameObject);
                GameObject.Destroy(night_optic.AdditionalReticleMeshes[0].gameObject);
                GameObject.Destroy(night_optic.transform.Find("Reticle Mesh WFOV").gameObject);
            }

            GameObject javelin_mode_hud = GameObject.Instantiate(Assets.javelin_mode_hud, flir_canvas.parent);
            GameObject javelin_hud = GameObject.Instantiate(Assets.javelin_hud, flir_canvas.parent);

            GameObject flir_post_seeker = GameObject.Instantiate(night_optic.transform.Find("FLIR Post Processing/FLIR Only Volume").gameObject, night_optic.transform);
            flir_post_seeker.SetActive(false);
            PostProcessProfile profile = flir_post_seeker.GetComponent<PostProcessVolume>().profile;
            ColorGrading color_grading;
            profile.TryGetSettings(out color_grading);
            color_grading.contrast.overrideState = true;
            color_grading.contrast.value = 300f;
            color_grading.postExposure.value = -0.5f;

            night_optic.slot.FLIRBlitMaterialOverride = Assets.flir_blit_mat_green;

            List<UsableOptic.FovLimitedItem> fov_limited_items = new List<UsableOptic.FovLimitedItem>()
            {
                new UsableOptic.FovLimitedItem() {
                    FovRange = new Vector2(7f, 8f),
                    ExclusiveObjects = new GameObject[] { javelin_hud.transform.Find("reticles/wfov").gameObject }
                },
                new UsableOptic.FovLimitedItem() {
                    FovRange = new Vector2(1f, 3f),
                    ExclusiveObjects = new GameObject[] { javelin_hud.transform.Find("reticles/nfov").gameObject }
                },
            };

            if (!has_ibas)
            {
                night_optic.FovLimitedItems = fov_limited_items.ToArray();
            }
            else
            {
                List<UsableOptic.FovLimitedItem> temp_fov_limited = night_optic.FovLimitedItems.ToList();
                temp_fov_limited.AddRange(fov_limited_items);
                night_optic.FovLimitedItems = temp_fov_limited.ToArray();
            }

            FNFManager manager = tow.gameObject.AddComponent<FNFManager>();
            manager.ammo_dir = ammo_dir;

            FNFOptic fnf_optic = night_optic.gameObject.AddComponent<FNFOptic>();
            fnf_optic.gates = javelin_hud.transform.Find("gate holder").GetComponent<RectTransform>();
            fnf_optic.manual_gates = javelin_hud.transform.Find("manual gate holder").GetComponent<RectTransform>();
            fnf_optic.reticles = javelin_hud.transform.Find("reticles");
            fnf_optic.seek_text = javelin_mode_hud.transform.Find("seek");
            fnf_optic.seeker_box = javelin_mode_hud.transform.Find("seeker boxes");
            fnf_optic.modes = new Dictionary<FNFMode, Transform>()
            {
                [FNFMode.TopAttack] = javelin_mode_hud.transform.Find("top"),
                [FNFMode.Direct] = javelin_mode_hud.transform.Find("dir"),
            };
            fnf_optic.point_targeting = javelin_mode_hud.transform.Find("pnt");
            fnf_optic.seeker_post = flir_post_seeker;
            fnf_optic.main_post = night_optic.transform.Find("FLIR Post Processing").gameObject;
            fnf_optic.manager = manager;

            tow.MaxSpeedToDeploy = 999f;

            List<GameObject> temp_tow_exclusive = tow.ExclusiveItems.ToList();
            List<GameObject> javelin_exclusive_go = new List<GameObject>()
            {
                javelin_hud,
                javelin_mode_hud
            };

            if (!has_ibas)
            {
                temp_tow_exclusive.RemoveAt(0);
                temp_tow_exclusive.Add(bushmaster.ExclusiveItems[0]);
                temp_tow_exclusive.Add(javelin_mode_hud);
                temp_tow_exclusive.Add(javelin_hud);
            }
            else
            {
                javelin_exclusive_go.Add(tow.ExclusiveItems[0]);
                temp_tow_exclusive = javelin_exclusive_go;
            }

            tow.ExclusiveItems = temp_tow_exclusive.ToArray();

            AimablePlatform tow_elevation = fcs.Mounts.Where(o => o.name == "TOW elevation scripts").First();
            tow_elevation.enabled = false;
            tow_elevation.Transform.localEulerAngles = new Vector3(342f, 0f, 0f);
            tow_elevation.ForcedStowSpeed = 999f;

            javelin_hud.SetActive(false);
            javelin_mode_hud.SetActive(false);
        }
    }
}
