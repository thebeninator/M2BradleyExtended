using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using GHPC.Weaponry;
using ModUtil;

namespace M2BradleyExtended
{
    internal sealed class Ammo
    {
        public static AmmoCodexScriptable m919_round_codex;
        public static AmmoClipCodexScriptable m919_70_clip_codex;
        public static AmmoClipCodexScriptable m919_50_clip_codex;
        public static AmmoClipCodexScriptable m919_230_clip_codex;

        public static AmmoCodexScriptable tow2_round_codex;
        public static AmmoClipCodexScriptable tow2_clip_codex;

        public static AmmoCodexScriptable tow2a_round_codex;
        public static AmmoClipCodexScriptable tow2a_clip_codex;

        public static AmmoCodexScriptable tow2b_round_codex;
        public static AmmoClipCodexScriptable tow2b_clip_codex;

        public static AmmoCodexScriptable m409a1_round_codex;
        public static AmmoClipCodexScriptable m409a1_clip_codex;

        public static AmmoCodexScriptable mgm51b_round_codex;
        public static AmmoClipCodexScriptable mgm51b_clip_codex;

        public static AmmoCodexScriptable xm1203_round_codex;
        public static AmmoClipCodexScriptable xm1203_50_clip_codex;
        public static AmmoClipCodexScriptable xm1203_150_clip_codex;

        public static Dictionary<string, AmmoClipCodexScriptable> tow_missiles = new Dictionary<string, AmmoClipCodexScriptable>() {};

        public static void Init()
        {
            M919();
            TOW2();
            TOW2A();
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
            AmmoType tow2_ammo = new AmmoType();
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
            AmmoType tow2a_ammo = new AmmoType();
            Util.ShallowCopy(tow2a_ammo, tow2_round_codex.AmmoType);
            tow2a_ammo.CachedIndex = -1;
            tow2a_ammo.TntEquivalentKg = 4.5f;
            tow2a_ammo.Name = "BGM-71E TOW-2A";

            string era_schema = Assembly.CreateQualifiedName("PactIncreasedLethality", "PactIncreasedLethality.EraSchema");
            string k1 = Assembly.CreateQualifiedName("PactIncreasedLethality", "PactIncreasedLethality.Kontakt1");
            string k5 = Assembly.CreateQualifiedName("PactIncreasedLethality", "PactIncreasedLethality.Kontakt5");
            string relikt = Assembly.CreateQualifiedName("PactIncreasedLethality", "PactIncreasedLethality.Relikt");
            Type era_schema_type = Type.GetType(era_schema);
            Type k1_type = Type.GetType(k1);
            Type k5_type = Type.GetType(k5);
            Type relikt_type = Type.GetType(relikt);

            if (k1_type != null)
            {
                BindingFlags flags = BindingFlags.Static | BindingFlags.Public;
                FieldInfo era_schema_so = era_schema_type.GetField("era_so", BindingFlags.Public | BindingFlags.Instance);
                Func<Type, ArmorCodexScriptable> get_codex = type => (ArmorCodexScriptable)era_schema_so.GetValue(type.GetField("schema", flags).GetValue(null));

                tow2a_ammo.ArmorOptimizations = new AmmoType.ArmorOptimization[] {
                    new AmmoType.ArmorOptimization() {
                        Armor = get_codex(relikt_type),
                        RhaRatio = 0.10f
                    },
                    new AmmoType.ArmorOptimization() {
                        Armor = get_codex(k5_type),
                        RhaRatio = 0.05f
                    },
                    new AmmoType.ArmorOptimization() {
                        Armor = get_codex(k1_type),
                        RhaRatio = 0.05f
                    }
                };
            }

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

        private static void M919() {
            AmmoType m919_ammo = new AmmoType();
            Util.ShallowCopy(m919_ammo, Assets.m791_round_codex.AmmoType);
            m919_ammo.CachedIndex = -1;
            m919_ammo.RhaPenetration = 105f;
            m919_ammo.MuzzleVelocity = 1400f;
            m919_ammo.Mass = 0.1f;
            m919_ammo.MaximumRange = 2500f;
            m919_ammo.Coeff = 0.008f;
            m919_ammo.Name = "25mm APFSDS-T M919";

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
            AmmoType xm1203_ammo = new AmmoType();
            Util.ShallowCopy(xm1203_ammo, Assets.m791_round_codex.AmmoType);
            xm1203_ammo.CachedIndex = -1;
            xm1203_ammo.RhaPenetration = 180f;
            xm1203_ammo.MuzzleVelocity = 1600f;
            xm1203_ammo.Mass = 0.160f;
            xm1203_ammo.MaximumRange = 3500f;
            xm1203_ammo.Coeff = 0.008f;
            xm1203_ammo.Caliber = 50f;
            xm1203_ammo.SectionalArea *= 2f;
            xm1203_ammo.Name = "50mm APFSDS-T XM1203";

            xm1203_round_codex = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
            xm1203_round_codex.AmmoType = xm1203_ammo;
            xm1203_round_codex.name = "xm1203_ammo";

            AmmoType.AmmoClip clip_50 = new AmmoType.AmmoClip();
            clip_50.Capacity = 50;
            clip_50.Name = "XM1203 APFSDS-T";
            clip_50.MinimalPattern = new AmmoCodexScriptable[1];
            clip_50.MinimalPattern[0] = xm1203_round_codex;
            xm1203_50_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            xm1203_50_clip_codex.name = "clip_50";
            xm1203_50_clip_codex.ClipType = clip_50;

            AmmoType.AmmoClip clip_150 = new AmmoType.AmmoClip();
            clip_150.Capacity = 150;
            clip_150.Name = "XM1203 APFSDS-T";
            clip_150.MinimalPattern = new AmmoCodexScriptable[1];
            clip_150.MinimalPattern[0] = xm1203_round_codex;
            xm1203_150_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            xm1203_150_clip_codex.name = "clip_150";
            xm1203_150_clip_codex.ClipType = clip_150;
        }
    }
}
