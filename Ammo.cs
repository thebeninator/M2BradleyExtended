using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using GHPC.Weaponry;
using ModUtil;
using GHPC.Effects;
using MelonLoader;
using GHPC.Weapons;
using GHPC;
using GHPC.Equipment;
using System.Linq;
using System.Collections;
using GHPC.State;

namespace M2BradleyExtended
{
    internal sealed class Ammo
    {
        private static AmmoType m919_ammo = new AmmoType();
        public static AmmoCodexScriptable m919_round_codex;
        public static AmmoClipCodexScriptable m919_70_clip_codex;
        public static AmmoClipCodexScriptable m919_50_clip_codex;
        public static AmmoClipCodexScriptable m919_230_clip_codex;

        private static AmmoType tow2_ammo = new AmmoType();
        public static AmmoCodexScriptable tow2_round_codex;
        public static AmmoClipCodexScriptable tow2_clip_codex;

        private static AmmoType tow2a_ammo = new AmmoType();
        public static AmmoCodexScriptable tow2a_round_codex;
        public static AmmoClipCodexScriptable tow2a_clip_codex;

        private static AmmoType towff_ammo = new AmmoType();
        public static AmmoCodexScriptable towff_round_codex;
        public static AmmoClipCodexScriptable towff_clip_codex;
        public static AmmoType towff_dir_ammo = new AmmoType();

        private static AmmoType towff_mp_ammo = new AmmoType();
        public static AmmoCodexScriptable towff_mp_round_codex;
        public static AmmoClipCodexScriptable towff_mp_clip_codex;

        public static AmmoType towff_mp_dir_ammo = new AmmoType();

        public static AmmoCodexScriptable tow2b_round_codex;
        public static AmmoClipCodexScriptable tow2b_clip_codex;

        public static AmmoCodexScriptable m409a1_round_codex;
        public static AmmoClipCodexScriptable m409a1_clip_codex;

        public static AmmoCodexScriptable mgm51b_round_codex;
        public static AmmoClipCodexScriptable mgm51b_clip_codex;

        private static AmmoType xm1203_ammo = new AmmoType();
        public static AmmoCodexScriptable xm1203_round_codex;
        public static AmmoClipCodexScriptable xm1203_50_clip_codex;
        public static AmmoClipCodexScriptable xm1203_170_clip_codex;

        private static AmmoType xm1204_ammo = new AmmoType();
        public static AmmoCodexScriptable xm1204_round_codex;
        public static AmmoClipCodexScriptable xm1204_50_clip_codex;
        public static AmmoClipCodexScriptable xm1204_170_clip_codex;

        public static Dictionary<string, AmmoClipCodexScriptable> tow_missiles = new Dictionary<string, AmmoClipCodexScriptable>() {};

        public static IEnumerator SetupEraOptimizations(GameState _)
        {
            ArmorCodexScriptable[] armor_codices = Resources.FindObjectsOfTypeAll<ArmorCodexScriptable>();
            ArmorCodexScriptable[] k1_k5_m1_codices = armor_codices.Where
            (
                o => 
                o.name.Contains("Kontakt-1") ||
                o.name.Contains("Kontakt-5") ||
                o.name.Contains("M1 ERA")
            ).ToArray();

            ArmorCodexScriptable[] relikt_codices = armor_codices.Where(o => o.name.Contains("Relikt")).ToArray();

            List<AmmoType.ArmorOptimization> optimizations = new List<AmmoType.ArmorOptimization>();

            foreach (ArmorCodexScriptable codex in k1_k5_m1_codices)
            {
                optimizations.Add(Util.CreateArmourOptimization(codex, 0.05f));
            }

            foreach (ArmorCodexScriptable relikt_codex in relikt_codices)
            {
                optimizations.Add(Util.CreateArmourOptimization(relikt_codex, 0.1f));
            }

            tow2a_ammo.ArmorOptimizations = optimizations.ToArray();
            towff_ammo.ArmorOptimizations = tow2a_ammo.ArmorOptimizations;
            towff_dir_ammo.ArmorOptimizations = tow2a_ammo.ArmorOptimizations;

            yield break;
        }

        public static void Init()
        {
            XM1203();
            XM1204();
            M919();
            TOW2();
            TOW2A();
            TOWFF();
            TOWFFMP();
            //M409A1();
            //MGM51B();
        }

        private static void M409A1() {
            AmmoType m409a1_ammo = new AmmoType();
            Util.ShallowCopy(m409a1_ammo, Assets.br412d_round_codex.AmmoType);
            m409a1_ammo.CachedIndex = -1;
            m409a1_ammo.RhaPenetration = 380f;
            m409a1_ammo.MuzzleVelocity = 682f;
            m409a1_ammo.TntEquivalentKg = 3.73f;
            m409a1_ammo.SpallMultiplier = 2f;
            m409a1_ammo.Category = AmmoType.AmmoCategory.ShapedCharge;
            m409a1_ammo.ShatterOnRicochet = true;
            m409a1_ammo.Caliber = 152;
            m409a1_ammo.SectionalArea = 0.018f;
            m409a1_ammo.Name = "M409A1 HEAT-T";
            m409a1_ammo.RhaToFuse = 0f;
            m409a1_ammo.ShortName = AmmoType.AmmoShortName.Heat;
            m409a1_ammo.ArmorOptimizations = new AmmoType.ArmorOptimization[] { };
            m409a1_ammo.ImpactFuseTime = 0f;

            m409a1_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            m409a1_round_codex.AmmoType = m409a1_ammo;
            m409a1_round_codex.name = "m409a1_ammo";

            AmmoType.AmmoClip m409a1_clip = new AmmoType.AmmoClip();
            m409a1_clip.Capacity = 1;
            m409a1_clip.Name = "M409A1 HEAT-T";
            m409a1_clip.MinimalPattern = new AmmoCodexScriptable[1];
            m409a1_clip.MinimalPattern[0] = m409a1_round_codex;
            m409a1_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            m409a1_clip_codex.name = "m409a1_clip";
            m409a1_clip_codex.ClipType = m409a1_clip;
        }

        private static void MGM51B()
        {
            AmmoType mgm51b_ammo = new AmmoType();
            Util.ShallowCopy(mgm51b_ammo, Assets.atow_round_codex.AmmoType);
            mgm51b_ammo.CachedIndex = -1;
            mgm51b_ammo.RhaPenetration = 431f;
            mgm51b_ammo.TntEquivalentKg = 5.7f;
            mgm51b_ammo.MuzzleVelocity = 286f;
            mgm51b_ammo.Name = "MGM-51B Shillelagh";

            mgm51b_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            mgm51b_round_codex.AmmoType = mgm51b_ammo;
            mgm51b_round_codex.name = "mgm51b_ammo";

            AmmoType.AmmoClip mgm51b_clip = new AmmoType.AmmoClip();
            mgm51b_clip.Capacity = 1;
            mgm51b_clip.Name = "MGM-51B Shillelagh";
            mgm51b_clip.MinimalPattern = new AmmoCodexScriptable[1];
            mgm51b_clip.MinimalPattern[0] = mgm51b_round_codex;
            mgm51b_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            mgm51b_clip_codex.name = "mgm51b_clip";
            mgm51b_clip_codex.ClipType = mgm51b_clip;
        }

        private static void TOW2() {
            Util.ShallowCopy(tow2_ammo, Assets.itow_round_codex.AmmoType);
            tow2_ammo.CachedIndex = -1;
            tow2_ammo.RhaPenetration = 800f;
            tow2_ammo.MuzzleVelocity = 329f;
            tow2_ammo.TntEquivalentKg = 4.41f;
            tow2_ammo.SpallMultiplier = 2.5f;
            tow2_ammo.NoisePowerX = 30f;
            tow2_ammo.NoisePowerY = 30f;
            tow2_ammo.TurnSpeed = 0.18f;
            tow2_ammo.Name = "BGM-71D TOW-2";
            tow2_ammo.NoLeadCompensation = true;
            Util.CacheAmmo(tow2_ammo);

            if (tow2_round_codex != null) return;

            tow2_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            tow2_round_codex.AmmoType = tow2_ammo;
            tow2_round_codex.name = "tow2_ammo";

            AmmoType.AmmoClip tow2_clip = new AmmoType.AmmoClip();
            tow2_clip.Capacity = 2;
            tow2_clip.Name = "BGM-71D TOW-2";
            tow2_clip.MinimalPattern = new AmmoCodexScriptable[1];
            tow2_clip.MinimalPattern[0] = tow2_round_codex;
            tow2_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            tow2_clip_codex.name = "tow2_clip";
            tow2_clip_codex.ClipType = tow2_clip;

            tow_missiles.Add("TOW2", tow2_clip_codex);
        }

        private static void TOW2A()
        {
            Util.ShallowCopy(tow2a_ammo, tow2_round_codex.AmmoType);
            tow2a_ammo.CachedIndex = -1;
            tow2a_ammo.TntEquivalentKg = 4.5f;
            tow2a_ammo.Name = "BGM-71E TOW-2A";
            tow2a_ammo.NoLeadCompensation = true;
            Util.CacheAmmo(tow2a_ammo);

            if (tow2a_round_codex != null) return;

            tow2a_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            tow2a_round_codex.AmmoType = tow2a_ammo;
            tow2a_round_codex.name = "tow2a_ammo";

            AmmoType.AmmoClip tow2a_clip = new AmmoType.AmmoClip();
            tow2a_clip.Capacity = 2;
            tow2a_clip.Name = "BGM-71E TOW-2A";
            tow2a_clip.MinimalPattern = new AmmoCodexScriptable[1];
            tow2a_clip.MinimalPattern[0] = tow2a_round_codex;
            tow2a_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            tow2a_clip_codex.name = "tow2a_clip";
            tow2a_clip_codex.ClipType = tow2a_clip;

            tow_missiles.Add("TOW2A", tow2a_clip_codex);
        }

        private static void TOWFF()
        {
            Util.ShallowCopy(towff_ammo, tow2_round_codex.AmmoType);
            towff_ammo.MuzzleVelocity = 150f;
            towff_ammo.RhaPenetration = 750f;
            towff_ammo.CachedIndex = -1;
            towff_ammo.TntEquivalentKg = 4.5f;
            towff_ammo.Name = "BGM-148A Super Javelin";
            towff_ammo.Guidance = AmmoType.GuidanceType.Laser;
            towff_ammo.Flight = AmmoType.FlightPattern.TopAttack;
            towff_ammo.ClimbAngle = 20f;
            towff_ammo.TurnSpeed = 2.5f;
            towff_ammo.DiveAngle = 89f;
            towff_ammo.LoiterAltitude = 220f;
            towff_ammo.Coeff = 0.5f;
            towff_ammo.AimPointMarch = 0f;
            towff_ammo.RangedFuseTime = 35f;
            towff_ammo.NoisePowerX = 0f;
            towff_ammo.NoisePowerY = 0f;
            towff_ammo.NoiseTimeScale = 1f;

            Util.CacheAmmo(towff_ammo);

            Util.ShallowCopy(towff_dir_ammo, towff_ammo);
            towff_dir_ammo.CachedIndex = -1;
            towff_dir_ammo.ClimbAngle = 18f;
            towff_dir_ammo.DiveAngle = 75f;
            towff_dir_ammo.Flight = AmmoType.FlightPattern.Hump;
            towff_dir_ammo.LoiterAltitude = 60f;
            towff_dir_ammo.LoiterEndDistance = 100f;
            towff_dir_ammo.AimPointMarch = 0f;
            Util.CacheAmmo(towff_dir_ammo);

            if (towff_round_codex != null) return;

            towff_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            towff_round_codex.AmmoType = towff_ammo;
            towff_round_codex.name = "towff_ammo";

            AmmoType.AmmoClip towff_clip = new AmmoType.AmmoClip();
            towff_clip.Capacity = 2;
            towff_clip.Name = "BGM-148A Super Javelin";
            towff_clip.MinimalPattern = new AmmoCodexScriptable[1];
            towff_clip.MinimalPattern[0] = towff_round_codex;
            towff_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            towff_clip_codex.name = "towff_clip";
            towff_clip_codex.ClipType = towff_clip;

            tow_missiles.Add("TOWFF", towff_clip_codex);
        }

        private static void TOWFFMP()
        {
            Util.ShallowCopy(towff_mp_ammo, towff_ammo);
            towff_mp_ammo.RhaPenetration = 650f;
            towff_mp_ammo.CachedIndex = -1;
            towff_mp_ammo.TntEquivalentKg = 10.2f;
            towff_mp_ammo.MinSpallRha = 12f;
            towff_mp_ammo.MaxSpallRha = 19f;
            towff_mp_ammo.MuzzleVelocity = 140f;
            towff_mp_ammo.Name = "BGM-148B Super Javelin";
            towff_mp_ammo.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor()
            {
                HasImpactEffect = true,
                EffectSize = ParticleEffectsManager.EffectSize.Bomb,
                ImpactCategory = ParticleEffectsManager.Category.HighExplosive,
                Flags = ParticleEffectsManager.ImpactModifierFlags.VeryLarge,
                MinFilterStrictness = ParticleEffectsManager.FilterStrictness.Medium,
                RicochetType = ParticleEffectsManager.RicochetType.NormalTracer
            };
            towff_mp_ammo.ArmorOptimizations = new AmmoType.ArmorOptimization[] { };

            Util.CacheAmmo(towff_mp_ammo);

            Util.ShallowCopy(towff_mp_dir_ammo, towff_mp_ammo);
            towff_mp_dir_ammo.CachedIndex = -1;
            towff_mp_dir_ammo.ClimbAngle = 18f;
            towff_mp_dir_ammo.DiveAngle = 75f;
            towff_mp_dir_ammo.Flight = AmmoType.FlightPattern.Hump;
            towff_mp_dir_ammo.LoiterAltitude = 60f;
            towff_mp_dir_ammo.LoiterEndDistance = 100f;
            towff_mp_dir_ammo.AimPointMarch = 0f;
            Util.CacheAmmo(towff_mp_dir_ammo);

            if (towff_mp_round_codex != null) return;

            towff_mp_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            towff_mp_round_codex.AmmoType = towff_mp_ammo;
            towff_mp_round_codex.name = "towff_mp_ammo";

            AmmoType.AmmoClip towff_mp_clip = new AmmoType.AmmoClip();
            towff_mp_clip.Capacity = 2;
            towff_mp_clip.Name = "BGM-148B Super Javelin";
            towff_mp_clip.MinimalPattern = new AmmoCodexScriptable[1];
            towff_mp_clip.MinimalPattern[0] = towff_mp_round_codex;
            towff_mp_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            towff_mp_clip_codex.name = "towff_mp_clip";
            towff_mp_clip_codex.ClipType = towff_mp_clip;

            tow_missiles.Add("TOWFFMP", towff_mp_clip_codex);
        }


        private static void M919() {
            Util.ShallowCopy(m919_ammo, Assets.m791_round_codex.AmmoType);
            m919_ammo.CachedIndex = -1;
            m919_ammo.RhaPenetration = 105f;
            m919_ammo.MuzzleVelocity = 1400f;
            m919_ammo.Mass = 0.1f;
            m919_ammo.MaximumRange = 2500f;
            m919_ammo.Coeff = 0.008f;
            m919_ammo.Name = "25mm APFSDS-T M919";
            Util.CacheAmmo(m919_ammo);

            if (m919_round_codex != null) return;

            m919_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            m919_round_codex.AmmoType = m919_ammo;
            m919_round_codex.name = "m919_ammo";

            AmmoType.AmmoClip clip_70 = new AmmoType.AmmoClip();
            clip_70.Capacity = 70;
            clip_70.Name = "M919 APFSDS-T";
            clip_70.MinimalPattern = new AmmoCodexScriptable[1];
            clip_70.MinimalPattern[0] = m919_round_codex;
            m919_70_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            m919_70_clip_codex.name = "clip_70";
            m919_70_clip_codex.ClipType = clip_70;

            AmmoType.AmmoClip clip_50 = new AmmoType.AmmoClip();
            clip_50.Capacity = 50;
            clip_50.Name = "M919 APFSDS-T";
            clip_50.MinimalPattern = new AmmoCodexScriptable[1];
            clip_50.MinimalPattern[0] = m919_round_codex;
            m919_50_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            m919_50_clip_codex.name = "clip_50";
            m919_50_clip_codex.ClipType = clip_50;

            AmmoType.AmmoClip clip_230 = new AmmoType.AmmoClip();
            clip_230.Capacity = 230;
            clip_230.Name = "M919 APFSDS-T";
            clip_230.MinimalPattern = new AmmoCodexScriptable[1];
            clip_230.MinimalPattern[0] = m919_round_codex;
            m919_230_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            m919_230_clip_codex.name = "clip_230";
            m919_230_clip_codex.ClipType = clip_230;
        }

        private static void XM1203()
        {
            Util.ShallowCopy(xm1203_ammo, Assets.m791_round_codex.AmmoType);
            xm1203_ammo.CachedIndex = -1;
            xm1203_ammo.RhaPenetration = 190f;
            xm1203_ammo.MuzzleVelocity = 1600f;
            xm1203_ammo.Mass = 0.160f;
            xm1203_ammo.MaximumRange = 3500f;
            xm1203_ammo.Coeff = 0.0035f;
            xm1203_ammo.Caliber = 50f;
            xm1203_ammo.SectionalArea *= 1.35f;
            xm1203_ammo.Name = "50mm APFSDS-T M1203";
            Util.CacheAmmo(xm1203_ammo);

            if (xm1203_round_codex != null) return;

            xm1203_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            xm1203_round_codex.AmmoType = xm1203_ammo;
            xm1203_round_codex.name = "xm1203_ammo";

            AmmoType.AmmoClip clip_50 = new AmmoType.AmmoClip();
            clip_50.Capacity = 50;
            clip_50.Name = "M1203 APFSDS-T";
            clip_50.MinimalPattern = new AmmoCodexScriptable[1];
            clip_50.MinimalPattern[0] = xm1203_round_codex;
            xm1203_50_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            xm1203_50_clip_codex.name = "clip_50";
            xm1203_50_clip_codex.ClipType = clip_50;

            AmmoType.AmmoClip clip_170 = new AmmoType.AmmoClip();
            clip_170.Capacity = 170;
            clip_170.Name = "M1203 APFSDS-T";
            clip_170.MinimalPattern = new AmmoCodexScriptable[1];
            clip_170.MinimalPattern[0] = xm1203_round_codex;
            xm1203_170_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            xm1203_170_clip_codex.name = "clip_170";
            xm1203_170_clip_codex.ClipType = clip_170;
        }

        private static void XM1204()
        {
            Util.ShallowCopy(xm1204_ammo, Assets.m792_round_codex.AmmoType);
            xm1204_ammo.CachedIndex = -1;
            xm1204_ammo.RhaPenetration = 19f;
            xm1204_ammo.MuzzleVelocity = 1001f;
            xm1204_ammo.Mass = 0.250f;
            xm1204_ammo.MaximumRange = 3500f;
            xm1204_ammo.Coeff = 0f;
            xm1204_ammo.Caliber = 50f;
            xm1204_ammo.TntEquivalentKg = 0.095f;
            xm1204_ammo.SectionalArea *= 4f;
            xm1204_ammo.DetonateSpallCount = 60;
            xm1204_ammo.ImpactFuseTime = 0.0025f; ;
            xm1204_ammo.Name = "50mm HEAB-T M1204";
            Util.CacheAmmo(xm1204_ammo);

            if (xm1204_round_codex != null) return;

            xm1204_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            xm1204_round_codex.AmmoType = xm1204_ammo;
            xm1204_round_codex.name = "xm1204_ammo";

            AmmoType.AmmoClip clip_50 = new AmmoType.AmmoClip();
            clip_50.Capacity = 50;
            clip_50.Name = "M1204 HEAB-T";
            clip_50.MinimalPattern = new AmmoCodexScriptable[1];
            clip_50.MinimalPattern[0] = xm1204_round_codex;
            xm1204_50_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            xm1204_50_clip_codex.name = "clip_50";
            xm1204_50_clip_codex.ClipType = clip_50;

            AmmoType.AmmoClip clip_170 = new AmmoType.AmmoClip();
            clip_170.Capacity = 170;
            clip_170.Name = "M1204 HEAB-T";
            clip_170.MinimalPattern = new AmmoCodexScriptable[1];
            clip_170.MinimalPattern[0] = xm1204_round_codex;
            xm1204_170_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            xm1204_170_clip_codex.name = "clip_170";
            xm1204_170_clip_codex.ClipType = clip_170;

            BallisticComputer xm1204_bc = BallisticComputerRepository._instance.RegisterAmmoType(xm1204_ammo);
            xm1204_bc.SimTimeStep = 0.00015f;
            //xm1204_bc._useVelocityOverride = true;
            //xm1204_bc._velocityOverride = 1000f;
            xm1204_bc.RefreshData();
        }
    }
}
