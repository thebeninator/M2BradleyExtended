using GHPC.Weapons;
using UnityEngine;

namespace M2BradleyExtended
{
    internal sealed class CustomGuidanceComputer : MonoBehaviour
    {
        public FireControlSystem fcs;
        public MissileGuidanceUnit mgu;

        void Update()
        {
            mgu.AimElement = fcs.MainOptic.transform;
        }
    }
}
