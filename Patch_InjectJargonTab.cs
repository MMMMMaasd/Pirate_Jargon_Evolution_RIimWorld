using System;
using System.Collections.Generic;
using Verse;

namespace PirateJargonEvolution
{
    [StaticConstructorOnStartup]
    public static class Patch_InjectJargonTab
    {
        static Patch_InjectJargonTab()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.race != null && def.race.Humanlike)
                {
                    if (def.inspectorTabs == null)
                    {
                        def.inspectorTabs = new List<Type>();
                    }

                    if (!def.inspectorTabs.Contains(typeof(ITab_Pawn_Jargon)))
                    {
                        def.inspectorTabs.Add(typeof(ITab_Pawn_Jargon));
                        Log.Message($"[PirateJargon] Injected ITab_Pawn_Jargon into {def.defName}");
                    }
                }
            }
        }
    }
}