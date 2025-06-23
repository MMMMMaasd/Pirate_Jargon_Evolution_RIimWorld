using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;

namespace PirateJargonEvolution
{
    [HarmonyPatch(typeof(Dialog_InfoCard), "DoWindowContents")]
    public static class Patch_InfoCard_FactionJargon
    {
        public static void Postfix(Dialog_InfoCard __instance, Rect inRect)
        {
            Faction faction = Traverse.Create(__instance).Field("faction").GetValue<Faction>();

            if (faction == null)
            {
                object entity = Traverse.Create(__instance).Field("thingOrDef").GetValue();
                if (entity is Faction f) faction = f;
                else if (entity is FactionDef def)
                    faction = Find.FactionManager.AllFactions.FirstOrDefault(fac => fac.def == def);
            }

            if (faction == null)
            {
                return;
            }

            // FIX: Use faction's unique LoadID instead of defName
            string memoryKey = faction.GetUniqueLoadID();

            var memory = Current.Game.GetComponent<PirateFactionManager>()?.GetFactionMemory(memoryKey);
            if (memory == null || memory.JargonEvolutionHistory.Count == 0)
            {
                Log.Message($"------------[PirateJargon] No memory or no jargon history for faction {faction.Name} ----------");
                return;
            }

            float x = inRect.x + 20f;
            float y = inRect.y + 400f;
            float width = inRect.width - 40f;

            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(x, y, width, 24f), "Known Faction Jargon:");
            y += 26f;

            foreach (var entry in memory.JargonEvolutionHistory)
            {
                if (y > inRect.yMax - 30f) break;
                Widgets.Label(new Rect(x + 10f, y, width - 10f, 22f), $"• {entry.JargonWord}: {entry.Meaning}");
                y += 22f;
            }
        }
    }
}