using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace StandaloneHotSpringMoodBuff
{
    [StaticConstructorOnStartup]
    public static class HotSpringMoodPatch
    {
        public static HediffDef BathedInStandaloneHotspringDef;

        static HotSpringMoodPatch()
        {
            BathedInStandaloneHotspringDef = DefDatabase<HediffDef>.GetNamedSilentFail("BathedInStandaloneHotspring");
            if (BathedInStandaloneHotspringDef == null)
            {
                Log.Error("[StandaloneHotSpringMoodBuff] could not find BathedInStandaloneHotspring hediff def");
                return;
            }

            Harmony harmony = new Harmony("tyster.StandaloneHotSpringMoodBuff");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("[StandaloneHotSpringMoodBuff] patched successfully");
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker), "AddHediff", new Type[] { typeof(Hediff), typeof(BodyPartRecord), typeof(DamageInfo?), typeof(DamageWorker.DamageResult) })]
    public static class Patch_AddHediff
    {
        public static void Postfix(Pawn_HealthTracker __instance, Hediff hediff)
        {
            if (hediff?.def != HotSpringMoodPatch.BathedInStandaloneHotspringDef)
                return;

            Pawn pawn = __instance.hediffSet?.pawn;
            if (pawn == null)
                return;

            ThoughtDef hotSpringThought = DefDatabase<ThoughtDef>.GetNamedSilentFail("HotSpring");
            if (hotSpringThought == null)
                return;

            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(hotSpringThought);
        }
    }
}