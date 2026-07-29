using System.Collections.Generic;
using GHPC.Weapons;
using UnityEngine;
using HarmonyLib;
using GHPC.Vehicle;
using System;
using GHPC;
using GHPC.PhysicsHelpers;
using GHPC.Player;
using GHPC.AI;

namespace M2BradleyExtended.FNF
{
    internal class FNFManager : MonoBehaviour
    {
        public static HashSet<AmmoType> ff_missiles = new HashSet<AmmoType>();

        private MissileGuidanceUnit current_mgu = null;
        private WeaponSystem weapon_system;
        private int weapon_system_id;
        private FireControlSystem fcs;
        private float seek_cd = 0f;
        private int self;

        public Vehicle target = null;
        public Vector3? point_target = null;
        public bool point_targeting = false;
        public bool seeker_active = false;
        public bool target_locked = false;
        public FNFMode current_mode = FNFMode.TopAttack;
        public Action<FNFMode, FNFMode> ModeChanged;
        public Action<bool> SeekerToggled;
        public Action<bool> PointTargetingToggled;
        public Action JustFired;

        void Awake()
        {
            self = GetComponentInParent<Vehicle>().GetInstanceID();
            weapon_system = GetComponent<WeaponSystem>();
            weapon_system_id = weapon_system.GetInstanceID();
            fcs = weapon_system.FCS;

            weapon_system.Fired += HandleFired;
            fcs.LaserFired += HandleLased;

            CreateMGU();
        }

        private void OnMissileDestroyed(LiveRound round, Vector3 vec)
        {
            weapon_system.GuidanceUnit.IsGuidingMissile = false;
            round.Destroyed -= OnMissileDestroyed;
        }

        void Update()
        {
            if (fcs.CurrentWeaponSystem.GetInstanceID() != weapon_system_id)
            {
                target = null;
                target_locked = false;
                SetSeeker(false);
                return;
            }

            if (PlayerInput.Instance?.CurrentPlayerWeapon?.Weapon.GetInstanceID() != weapon_system_id)
            {
                return;
            }

            if (seeker_active)
            {
                if (!point_targeting)
                {
                    PerformSeek();
                }
                else
                {
                    PerformPointTargetSeek();
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FNFMode old_mode = current_mode;
                current_mode = FNFMode.TopAttack;
                ModeChanged?.Invoke(old_mode, current_mode);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                FNFMode old_mode = current_mode;
                current_mode = FNFMode.Direct;
                ModeChanged?.Invoke(old_mode, current_mode);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                target = null;
                target_locked = false;
                point_targeting = !point_targeting;
                PointTargetingToggled?.Invoke(point_targeting);
            }
        }

        private void PerformPointTargetSeek()
        {
            if (seek_cd > 0f)
            {
                seek_cd -= Time.deltaTime;
                return;
            }

            Ray ray = new Ray(fcs.AimTransform.position, fcs.AimWorldVector);
            RaycastHit raycast_hit;

            RaycastColliderUtils.Raycast(ray, out raycast_hit, 5000f, ConstantsAndInfoManager.Instance.LaserRangefinderLayerMask);

            if (raycast_hit.transform == null)
            {
                point_target = null;
                return;
            }

            point_target = raycast_hit.point;

            seek_cd = 0.05f;
        }

        private void PerformSeek()
        {
            if (seek_cd > 0f)
            {
                seek_cd -= Time.deltaTime;
                return;
            }

            Ray ray = new Ray(fcs.ReferenceTransform.position, fcs.AimWorldVector);
            RaycastHit raycast_hit;

            int main_body_layer = 1 << 14;
            int terrain_layer = 1 << 18;
            Physics.Raycast(ray, out raycast_hit, 5000f, main_body_layer | terrain_layer);
            GameObject raycast_hit_obj = raycast_hit.transform?.gameObject;
            Vehicle possible_target = raycast_hit_obj?.GetComponentInParent<Vehicle>();

            if (possible_target != null && possible_target.GetInstanceID() != self)
            {
                target = possible_target;
            }

            if (target != null && possible_target != null && target_locked)
            {
                ray = new Ray(fcs.ReferenceTransform.position, target.gameObject.transform.Find("TRACKING OBJECT").transform.position - fcs.ReferenceTransform.position);
                Physics.Raycast(ray, out raycast_hit, 5000f, main_body_layer | terrain_layer);

                raycast_hit_obj = raycast_hit.transform?.gameObject;

                if (raycast_hit_obj == null || raycast_hit_obj.GetComponentInParent<Vehicle>() == null)
                {
                    target = null;
                    target_locked = false;
                }
            }

            seek_cd = 0.30f;
        }

        private void CreateMGU()
        {
            GameObject mgu_holder = new GameObject();
            mgu_holder.transform.localEulerAngles = Vector3.down;
            mgu_holder.name = "mgu holder " + mgu_holder.GetInstanceID();
            current_mgu = mgu_holder.AddComponent<MissileGuidanceUnit>();
            current_mgu.AimElement = current_mgu.transform;
            FNFTracker tracker = mgu_holder.AddComponent<FNFTracker>(); 
        }

        private void HandleFired(AmmoType ammo_type, LiveRound live_round)
        {
            FNFTracker tracker = current_mgu.GetComponent<FNFTracker>();
            tracker.target = target;
            tracker.mode = current_mode;
            tracker.point_targeting = point_targeting;
            tracker.point_target = point_target;

            if (current_mode == FNFMode.Direct)
            {
                live_round.Info = Ammo.towff_dir_ammo;
                tracker.transform.SetParent(live_round.transform);
                tracker.transform.localPosition = Vector3.zero;
            }

            tracker.enabled = true;
       
            weapon_system.GuidanceUnit.CurrentMissiles.Clear();
            weapon_system.GuidanceUnit.IsGuidingMissile = false;
            if (PlayerInput.Instance.CurrentPlayerWeapon.Weapon.GetInstanceID() != weapon_system_id)
            {
                weapon_system.GuidanceUnit.IsGuidingMissile = true;
                live_round.Destroyed += OnMissileDestroyed;
            }
            current_mgu.AddMissile(live_round);
            JustFired?.Invoke();
            SetSeeker(false);
            target = null;
            target_locked = false;

            CreateMGU();
        }

        private void HandleLased()
        {
            if (fcs.CurrentWeaponSystem.GetInstanceID() != weapon_system_id) return;

            SetSeeker(!seeker_active);
            target_locked = false;
            target = null;
        }

        public void SetSeeker(bool enabled)
        {
            seeker_active = enabled;
            SeekerToggled?.Invoke(seeker_active);
        }

        public void SetTargetLocked(bool locked)
        {
            target_locked = locked;
        }
    }

    [HarmonyPatch(typeof(WeaponSystem), "Fire")]
    internal class FNFFireHandler
    {
        private static float AI_TIME_TO_FIRE = 6.5f;

        private static bool Prefix(WeaponSystem __instance)
        {
            FNFManager fnf = __instance.GetComponent<FNFManager>();

            if (fnf == null) return true;

            if (__instance._unit.GetInstanceID() != PlayerInput.Instance.CurrentPlayerUnit.GetInstanceID())
            {
                UnitAI unit_ai = __instance._unit.InfoBroker.AI;
                Vehicle target = unit_ai.Target as Vehicle;

                if (unit_ai.Target != null)
                {
                    __instance.FireWhileGuidingMissile = false;
                    __instance.TriggerHoldTime = AI_TIME_TO_FIRE;
                    fnf.target = target;
                    fnf.target_locked = true;
                    fnf.SetSeeker(true);
                    
                    if (target.Type == UnitType.AirVehicle)
                    {
                        FNFMode old_mode = fnf.current_mode;
                        fnf.current_mode = FNFMode.Direct;
                        fnf.ModeChanged?.Invoke(old_mode, fnf.current_mode);
                    }

                    return true;
                }
            }

            __instance.TriggerHoldTime = 0.5f;
            __instance.FireWhileGuidingMissile = true;

            if (!fnf.seeker_active) return false;
            if (fnf.point_targeting && fnf.point_target.HasValue) return true;
            if (fnf.target == null || !fnf.target_locked) return false;

            return true;
        }
    }
}
