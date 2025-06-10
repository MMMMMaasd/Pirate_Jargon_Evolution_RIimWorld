using System.Collections.Generic;
using Verse;

namespace PirateJargonEvolution
{
    [StaticConstructorOnStartup]
    public static class Patch_Pawn
    {
        static Patch_Pawn()
        {
            InjectThinkingComp();
        }
        static void InjectThinkingComp()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.race != null && def.race.Humanlike)
                {
                    if (def.comps == null)
                    {
                        def.comps = new List<CompProperties>();
                    }

                    bool alreadyHas = def.comps.Any(c => c.compClass == typeof(CompThinking));
                    if (!alreadyHas)
                    {
                        def.comps.Add(new CompProperties
                        {
                            compClass = typeof(CompThinking)
                        });

                        Log.Message($"[PirateJargon] Added CompThinking to {def.defName}");
                    }
                }
            }
        }
    }
}