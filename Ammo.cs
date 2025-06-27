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

        public static void Init() {
            AmmoType m919_ammo = new AmmoType();
            Util.ShallowCopy(m919_ammo, Assets.m791_round_codex.AmmoType);
            m919_ammo.CachedIndex = -1;
            m919_ammo.RhaPenetration = 102f;
            m919_ammo.MuzzleVelocity = 1400f;
            m919_ammo.Mass = 0.1f;
            m919_ammo.MaximumRange = 2500f;
            m919_ammo.Coeff = 0.010f;
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
            clip_230.Capacity = 50;
            clip_230.Name = "M919 APFSDS-T";
            clip_230.MinimalPattern = new AmmoCodexScriptable[1];
            clip_230.MinimalPattern[0] = m919_round_codex;
            m919_230_clip_codex = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
            m919_230_clip_codex.name = "clip_230";
            m919_230_clip_codex.ClipType = clip_230;
        }
    }
}
