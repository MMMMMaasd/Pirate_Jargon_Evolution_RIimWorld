using System.Collections.Generic;
using Verse;
namespace PirateJargonEvolution
{
    [StaticConstructorOnStartup]
    public static class Patch_InjectPirateComp
    {
        static Patch_InjectPirateComp()
        {
            foreach (var def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.race != null && def.race.Humanlike)
                {
                    if (def.comps == null)
                    {
                        def.comps = new List<CompProperties>();
                    }

                    bool alreadyHas = def.comps.Exists(c => c.compClass == typeof(CompPirateIdentity));
                    if (!alreadyHas)
                    {
                        def.comps.Add(new CompProperties_PirateIdentity());
                        Log.Message($"[PirateJargon] Added CompPirateIdentity to {def.defName}");
                    }
                }
            }
        }
    }
}