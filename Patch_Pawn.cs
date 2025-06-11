using System.Collections.Generic;
using Verse;

namespace PirateJargonEvolution
{
    [StaticConstructorOnStartup]
    public static class Patch_Pawn
    {
        static Patch_Pawn()
        {
            InjectPirateModComp();
        }
        static void InjectPirateModComp()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.race != null && def.race.Humanlike)
                {
                    if (def.comps == null)
                    {
                        def.comps = new List<CompProperties>();
                    }

                    bool alreadyHasThinking = def.comps.Any(c => c.compClass == typeof(CompThinking));
                    if (!alreadyHasThinking)
                    {
                        def.comps.Add(new CompProperties
                        {
                            compClass = typeof(CompThinking)
                        });

                        Log.Message($"[PirateJargon] Added CompThinking to {def.defName}");
                    }
                    
                    bool alreadyHasPirateIdentity = def.comps.Exists(c => c.compClass == typeof(CompPirateIdentity));
                    if (!alreadyHasPirateIdentity)
                    {
                        def.comps.Add(new CompProperties_PirateIdentity());
                        Log.Message($"[PirateJargon] Added CompPirateIdentity to {def.defName}");
                    }
                    
                }
            }
        }
    }
}


