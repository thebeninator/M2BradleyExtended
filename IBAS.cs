using System.Collections.Generic;
using System.Linq;
using GHPC.Equipment.Optics;
using GHPC.Weapons;
using Reticle;
using UnityEngine;
using ModUtil;

namespace M2BradleyExtended
{
    internal class IBASLine
    {
        public AngularLength length;
        public AngularVector2 position;
        public float rot;

        public IBASLine(AngularLength length, AngularVector2 position, float rot = 0f)
        {
            this.length = length;
            this.position = position;
            this.rot = rot;
        }
    }

    internal sealed class IBAS : MonoBehaviour
    {
        public static ReticleSO day_reticleSO;
        public static ReticleSO night_reticleSO;
        public static ReticleSO night_reticle_WFOV_SO;

        private static List<IBASLine> lines = new List<IBASLine>{};
        private static List<IBASLine> wfov_bracket_lines = new List<IBASLine>{};

        private static AngularLength thickness = new AngularLength(angle: 0.12f, unit: AngularLength.AngularUnit.MIL_NATO);
         
        public static void Init()
        {
            // short horz, long horz, long vert
            float[] length = new float[] {1.2f, 8.72f, 11.27f};
            float[] rot = new float[] {0f, 0f, 90f};
            Vector2[] pos_rel_offset = new Vector2[]
            {
                new Vector2(1.3f + length[0] / 2f, 0f),
                new Vector2(5f + length[1] / 2f, 0f),
                new Vector2(0f, 1.11f + length[2] / 2f)
            };

            for (int i = -1; i <= 1; i += 2) 
            {
                for (int j = 0; j <= 2; j++)
                {
                    lines.Add(new IBASLine(
                        new AngularLength(angle: length[j], unit: AngularLength.AngularUnit.MIL_NATO),
                        new AngularVector2(pos_rel_offset[j] * i, unit: AngularLength.AngularUnit.MIL_NATO), 
                        rot[j]                
                    ));
                }
            }

            DayReticle();
            NightReticle();
        }

        public static void Add(UsableOptic day_optic, FireControlSystem fcs, 
            WeaponSystem bushmaster, WeaponSystem tow, WeaponSystem m240
        ) {
            UsableOptic night_optic = day_optic.slot.LinkedNightSight.PairedOptic;

            GameObject.Destroy(day_optic.AdditionalReticleMeshes[0].gameObject);
            GameObject.Destroy(night_optic.AdditionalReticleMeshes[0].gameObject);
            GameObject.Destroy(night_optic.transform.Find("Reticle Mesh WFOV").gameObject);

            Transform flir_canvas = night_optic.transform.Find("M2 Bradley GPS canvas (1)");
            flir_canvas.Find("HUD elements").gameObject.SetActive(false);
            flir_canvas.Find("backdrop").localPosition = new Vector3(0f, -369.3267f, 0f);
            flir_canvas.Find("backdrop").localEulerAngles = new Vector3(0f, 180f, 180f);

            GameObject second_backdrop = GameObject.Instantiate(flir_canvas.Find("backdrop").gameObject, flir_canvas);
            second_backdrop.transform.localPosition = new Vector3(0f, 369.3267f, 0f);
            second_backdrop.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            GameObject ibas_hud = GameObject.Instantiate(Assets.ibas_hud, flir_canvas.parent);

            day_optic.reticleMesh.reticleSO = day_reticleSO;
            day_optic.reticleMesh.SMR = null;
            day_optic.reticleMesh.Load();
            day_optic.RotateAzimuth = true;
            day_optic.Alignment = OpticAlignment.BoresightStabilized;
            day_optic.slot.VibrationShakeMultiplier = 0f;
            day_optic.slot.VibrationBlurScale = 0f;

            night_optic.reticleMesh.reticleSO = night_reticleSO;
            night_optic.reticleMesh.SMR = null;
            night_optic.reticleMesh.Load();
            night_optic.RotateAzimuth = true;
            night_optic.RangeText = ibas_hud.transform.Find("range").GetComponent<TMPro.TextMeshProUGUI>();
            night_optic.FovLimitedItems = new UsableOptic.FovLimitedItem[] {
                new UsableOptic.FovLimitedItem() {
                    FovRange = new Vector2(7f, 8f),
                    ExclusiveObjects = new GameObject[] { ibas_hud.transform.Find("zoom level (2x)").gameObject }
                },
                new UsableOptic.FovLimitedItem() {
                    FovRange = new Vector2(1f, 3f),
                    ExclusiveObjects = new GameObject[] { ibas_hud.transform.Find("zoom level (4x)").gameObject }
                },
            };
            night_optic.Alignment = OpticAlignment.BoresightStabilized;
            night_optic.ReadyToFireObject = ibas_hud.transform.Find("ammo/ready box").gameObject;
            night_optic.slot.VibrationShakeMultiplier = 0f;
            night_optic.slot.VibrationBlurScale = 0f;
            night_optic.slot.FLIRHeight = 720;
            night_optic.slot.FLIRWidth = 1280;
            night_optic.slot.FLIRBlitMaterialOverride = Assets.flir_blit_mat_green;

            fcs._fixParallaxForVectorMode = true;
            fcs.DynamicLead = true;
            fcs.RecordTraverseRateBuffer = true;
            fcs.SuperleadWeapon = true;
            fcs.TraverseBufferSeconds = 0.5f;
            fcs._autoDumpViaPalmSwitches = true;

            List<GameObject> temp_bushmaster_exclusive = bushmaster.ExclusiveItems.ToList();
            temp_bushmaster_exclusive.RemoveRange(0, 2);
            bushmaster.ExclusiveItems = temp_bushmaster_exclusive.ToArray();

            List<GameObject> temp_tow_exclusive = tow.ExclusiveItems.ToList();
            temp_tow_exclusive.RemoveAt(0);
            tow.ExclusiveItems = temp_tow_exclusive.ToArray();

            List<GameObject> temp_m240_exclusive = m240.ExclusiveItems.ToList();
            temp_m240_exclusive.RemoveRange(0, 2);
            m240.ExclusiveItems = temp_m240_exclusive.ToArray();

            bushmaster.ExclusiveItems[1] = ibas_hud.transform.Find("ammo/bushmaster").gameObject;
            bushmaster.Feed.ExclusiveItems[0].Items[1] = ibas_hud.transform.Find("ammo/bushmaster/ap").gameObject;
            bushmaster.Feed.ExclusiveItems[1].Items[1] = ibas_hud.transform.Find("ammo/bushmaster/he").gameObject;
            tow.ExclusiveItems[1] = ibas_hud.transform.Find("ammo/tow").gameObject;
            m240.ExclusiveItems[1] = ibas_hud.transform.Find("ammo/7.62").gameObject;
        }

        private static void DayReticle() {
            day_reticleSO = new ReticleSO();
            day_reticleSO.name = "IBAS-DAY";

            day_reticleSO.lights = new List<ReticleTree.Light>() {
                new ReticleTree.Light() { 
                    type = ReticleTree.Light.Type.Powered,
                    color = new RGB(3.2f, 0f, 0f, true)
                }
            };

            List<ReticleTree.FocalPlane> planes = day_reticleSO.planes;
            ReticleTree.FocalPlane ffp = new ReticleTree.FocalPlane(type: ReticleTree.FocalPlane.Type.First);
            List<ReticleTree.TransformElement> ffp_elements = new List<ReticleTree.TransformElement>();
            ffp.elements = ffp_elements;
            planes.Add(ffp);

            ReticleTree.Angular angular_root = new ReticleTree.Angular(
                position: Vector2.zero,
                parent: null,
                align: ReticleTree.GroupBase.Alignment.Impact
            );
            
            List<ReticleTree.TransformElement> root_elements = angular_root.elements;
            ffp_elements.Add(angular_root);

            ReticleTree.Line dot = new ReticleTree.Line(
                position: new AngularVector2(new Vector2(0f, 0f), unit: AngularLength.AngularUnit.MIL_NATO),
                degrees: 0f,
                length: thickness,
                thickness: thickness,
                roundness: 1f
            );
            dot.visualType = ReticleTree.VisualElement.Type.ReflectedAdditive;
            dot.illumination = ReticleTree.Light.Type.Powered;

            foreach (IBASLine IBAS_line in lines) {
                ReticleTree.Line line = new ReticleTree.Line(
                    position: IBAS_line.position,
                    length: IBAS_line.length,
                    degrees: 0f,
                    thickness: thickness,
                    roundness: 1f
                );
                line.visualType = ReticleTree.VisualElement.Type.ReflectedAdditive;
                line.illumination = ReticleTree.Light.Type.Powered;
                line.rotation = new AngularLength(IBAS_line.rot, unit: AngularLength.AngularUnit.DEG);
                root_elements.Add(line);
            }

            root_elements.Add(dot);      
        }

        private static void NightReticle() {
            night_reticleSO = new ReticleSO();
            Util.ShallowCopy(night_reticleSO, day_reticleSO);
            night_reticleSO.name = "IBAS-FLIR";

            night_reticleSO.lights = new List<ReticleTree.Light>() {
                new ReticleTree.Light() {
                    type = ReticleTree.Light.Type.Powered,
                    color = new RGB(0, 3.2f, 0f, true)
                }
            };

            night_reticle_WFOV_SO = new ReticleSO();
            night_reticle_WFOV_SO.name = "IBAS-FLIR-WFOV";
            night_reticle_WFOV_SO.lights = night_reticleSO.lights;

            List<ReticleTree.FocalPlane> planes = night_reticle_WFOV_SO.planes;
            ReticleTree.FocalPlane ffp = new ReticleTree.FocalPlane(type: ReticleTree.FocalPlane.Type.First);
            List<ReticleTree.TransformElement> ffp_elements = new List<ReticleTree.TransformElement>();
            ffp.elements = ffp_elements;
            planes.Add(ffp);

            ReticleTree.Angular angular_root = new ReticleTree.Angular(
                position: Vector2.zero,
                parent: null,
                align: ReticleTree.GroupBase.Alignment.Impact
            );

            List<ReticleTree.TransformElement> root_elements = angular_root.elements;
            ffp_elements.Add(angular_root);

            ReticleTree.Line line = new ReticleTree.Line(
                position: Vector2.zero,
                length: 4.36f,
                degrees: 0f,
                thickness: thickness,
                roundness: 1f
            );
            line.visualType = ReticleTree.VisualElement.Type.ReflectedAdditive;
            line.illumination = ReticleTree.Light.Type.Powered;
            line.rotation = new AngularLength(0, unit: AngularLength.AngularUnit.DEG);
            root_elements.Add(line);
        }
    }
}
