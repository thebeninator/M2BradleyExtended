using GHPC.Weapons;
using UnityEngine;

namespace M2BradleyExtended.Airburst
{
    internal static class WeaponSystemExtension
    {
        public static void AddProgrammedFuse(this WeaponSystem weapon_system, AmmoType ammo_type, LiveRound round)
        {
            AirburstManager manager = weapon_system.GetComponent<AirburstManager>();

            if (manager == null) return;
            if (!manager.AirburstActive) return;
            if (ammo_type.CachedIndex != manager.AirburstAmmo.CachedIndex) return;

            float range = weapon_system.FCS.CurrentRange;
            float muzzle_offset = Vector3.Distance(weapon_system.MuzzleIdentity.position, weapon_system.FCS.ReferenceTransform.position);
            float flight_time = BallisticComputerRepository._instance.GetFlightTime(ammo_type, range + muzzle_offset + 2f);
            round._rangedFuseActive = true;
            round._fusedStatus = GHPC.Effects.ParticleEffectsManager.FusedStatus.Fuzed;
            round._rangedFuseCountdown = flight_time;
        }
    }
}
