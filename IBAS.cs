using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC;
using GHPC.Equipment.Optics;
using GHPC.Weapons;
using Reticle;
using UnityEngine;

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

        private static List<IBASLine> lines = new List<IBASLine> {};

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

            for (int i = -1; i <= 1; i += 2) {
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

        public static void Add(UsableOptic day_optic, FireControlSystem fcs) {
            UsableOptic night_optic = day_optic.slot.LinkedNightSight.PairedOptic;

            GameObject.Destroy(day_optic.AdditionalReticleMeshes[0].gameObject);
            GameObject.Destroy(night_optic.AdditionalReticleMeshes[0].gameObject);
            GameObject.Destroy(night_optic.transform.Find("Reticle Mesh WFOV").gameObject);

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
            night_optic.FovLimitedItems = new UsableOptic.FovLimitedItem[] { };
            night_optic.Alignment = OpticAlignment.BoresightStabilized;
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

            night_reticleSO.lights = new List<ReticleTree.Light>() {
                new ReticleTree.Light() {
                    type = ReticleTree.Light.Type.Powered,
                    color = new RGB(0, 3.2f, 0f, true)
                }
            };
        }
    }
}
