using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public static Dictionary<string, AmmoClipCodexScriptable> tow_missiles = new Dictionary<string, AmmoClipCodexScriptable>() {};

        public static void Init()
        {
            M919();
            TOW2();
        }

        private static void TOW2() {
            AmmoType tow2_ammo = new AmmoType();
            Util.ShallowCopy(tow2_ammo, Assets.itow_round_codex.AmmoType);
            tow2_ammo.CachedIndex = -1;
            tow2_ammo.RhaPenetration = 800f;
            tow2_ammo.MuzzleVelocity = 329f;
            tow2_ammo.TntEquivalentKg = 4.41f;
            tow2_ammo.SpallMultiplier = 2.5f;
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

        private static void M919 () {
            AmmoType m919_ammo = new AmmoType();
            Util.ShallowCopy(m919_ammo, Assets.m791_round_codex.AmmoType);
            m919_ammo.CachedIndex = -1;
            m919_ammo.RhaPenetration = 102f;
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
    }
}
