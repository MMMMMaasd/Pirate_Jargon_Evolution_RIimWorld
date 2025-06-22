using HarmonyLib;
using Verse;
using System;
using System.Collections.Generic;
using RimWorld;

namespace PirateJargonEvolution
{
    [StaticConstructorOnStartup]
    public static class Patch_AddTabResolver
    {
        static Patch_AddTabResolver()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.race != null && def.race.Humanlike)
                {
                    if (def.inspectorTabs == null)
                        def.inspectorTabs = new List<Type>();

                    if (!def.inspectorTabs.Contains(typeof(ITab_Pawn_Jargon)))
                    {
                        def.inspectorTabs.Add(typeof(ITab_Pawn_Jargon));
                        Log.Message($"[PirateJargon] Added ITab_Pawn_Jargon to {def.defName}");
                    }

                    // 手动构建 inspectorTabsResolved
                    def.inspectorTabsResolved = new List<InspectTabBase>();
                    foreach (var tabType in def.inspectorTabs)
                    {
                        try
                        {
                            var tabInstance = (ITab)Activator.CreateInstance(tabType);
                            def.inspectorTabsResolved.Add(tabInstance);
                        }
                        catch (Exception e)
                        {
                            Log.Warning($"[PirateJargon] Failed to instantiate tab {tabType.Name} for {def.defName}: {e}");
                        }
                    }
                }
            }
        }
    }
}