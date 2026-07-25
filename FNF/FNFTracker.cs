using System.Linq;
using GHPC.Vehicle;
using GHPC.Weapons;
using MelonLoader;
using UnityEngine;

namespace M2BradleyExtended.FNF
{
    internal class FNFTracker : MonoBehaviour
    {
        private MissileGuidanceUnit mgu;
        private Vector3 offset;

        public Vehicle target;
        public Transform tracking_object;
        public FNFMode mode;
        public bool point_targeting;
        public Vector3? point_target;

        void Awake()
        {
            mgu = GetComponent<MissileGuidanceUnit>();
            mgu.GuidanceStopped += HandleGuidanceStopped;
            this.enabled = false;
            offset = Random.insideUnitSphere * 0.1f;
        }

        void HandleGuidanceStopped()
        {
            GameObject.Destroy(this.gameObject);
        }

        void Update()
        {
            if (point_targeting)
            {
                this.transform.position = point_target.Value + Vector3.up * 60f;
                this.transform.LookAt(point_target.Value);
                return;
            }

            if (target == null) return;

            bool is_helo = target.Type == GHPC.UnitType.AirVehicle;

            if (tracking_object == null)
            {
                if (mode == FNFMode.TopAttack)
                {
                    offset.y = 0f;
                }

                try
                {
                    Transform transforms = is_helo ? target.transform : target.transform.Find("transforms");
                    //string desired_transform = mode == FNFMode.TopAttack ? "top" : "center";
                    tracking_object = transforms.GetComponentsInChildren<Transform>().Where(t => t.name.Contains("center")).First();
                }
                catch
                {
                    MelonLogger.Msg("failed to get transform center, defaulting to tracking obj");
                    tracking_object = target.transform.Find("TRACKING OBJECT");
                }
            }

            if (mode == FNFMode.TopAttack)
            {
                this.transform.position = tracking_object.position + Vector3.up * 120f;
            }

            this.transform.LookAt(tracking_object.position);
        }
    }
}
