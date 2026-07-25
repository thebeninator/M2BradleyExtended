using UnityEngine;
using HarmonyLib;

namespace M2BradleyExtended.FNF
{
    [HarmonyPatch(typeof(GHPC.Equipment.Optics.UsableOptic), "LateUpdate")]
    internal class FNFTrackingGateHandler
    {
        private static float cd = 0f;

        public static void Postfix(GHPC.Equipment.Optics.UsableOptic __instance)
        {
            FNFOptic fnf_optic = __instance.GetComponent<FNFOptic>() ?? null;

            if (fnf_optic == null) return;

            FNFManager fnf_manager = fnf_optic.manager;

            if (fnf_manager.target == null) return;
            if (fnf_optic.gates == null) return;

            Transform tracking_object = fnf_manager.target.gameObject.transform.Find("TRACKING OBJECT");

            if (tracking_object == null) return;

            Camera camera = Camera.main;
            Bounds bounds = tracking_object.GetComponent<MeshRenderer>().bounds;

            Vector3[] ss_corners = new Vector3[] {
                camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z)),
                camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)),
                camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z)),
                camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z)),
                camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z)),
                camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z)),
                camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z)),
                camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z))
            };

            float min_x = ss_corners[0].x;
            float min_y = ss_corners[0].y;
            float max_x = ss_corners[0].x;
            float max_y = ss_corners[0].y;

            for (int i = 1; i < 8; i++)
            {
                min_x = Mathf.Min(min_x, ss_corners[i].x);
                min_y = Mathf.Min(min_y, ss_corners[i].y);
                max_x = Mathf.Max(max_x, ss_corners[i].x);
                max_y = Mathf.Max(max_y, ss_corners[i].y);
            }

            fnf_optic.gates.position = new Vector2(min_x, min_y);
            fnf_optic.gates.sizeDelta = new Vector2(max_x - min_x, max_y - min_y);
        }
    }
}
