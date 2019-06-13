using Harmony;
using System;
using UnityEngine;

namespace MeasureIt
{
    [HarmonyPatch(typeof(RoadAI), "GetEffectRadius")]
    public static class RoadAIGetEffectRadiusPatch
    {
        static void Postfix(ref float radius)
        {
            try
            {
                radius = radius - 32f + (32f * 0.25f * ModConfig.Instance.Cells);
            }
            catch (Exception e)
            {
                Debug.Log("[Measure It!] RoadAIGetEffectRadiusPatch:Postfix -> Exception: " + e.Message);
            }
        }
    }
}
