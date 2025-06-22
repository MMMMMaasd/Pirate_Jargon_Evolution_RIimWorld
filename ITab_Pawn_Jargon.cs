using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace PirateJargonEvolution
{
    public class ITab_Pawn_Jargon : ITab
    {
        private const float Width = 630f;
        private const float Height = 430f;
        private Vector2 scrollPosition = Vector2.zero;

        private Pawn PawnForJargon
        {
            get
            {
                if (SelPawn != null) return SelPawn;
                if (SelThing is Corpse corpse) return corpse.InnerPawn;
                return null;
            }
        }

        public ITab_Pawn_Jargon()
        {
            size = new Vector2(Width, Height);
            labelKey = "TabPirateJargon";
            tutorTag = "Jargon";
        }

        public override bool IsVisible => true;

        protected override void FillTab()
        {
            Pawn pawn = PawnForJargon;
            if (pawn == null)
            {
                Log.Error("[PirateJargon] Jargon tab couldn't find pawn.");
                return;
            }

            var comp = pawn.TryGetComp<CompPirateIdentity>();
            if (comp == null || comp.knownJargon == null)
            {
                Widgets.Label(new Rect(10f, 10f, size.x - 20f, size.y - 20f), "No pirate identity found.");
                return;
            }

            var manager = Current.Game.GetComponent<PirateFactionManager>();
            if (!manager.pirateFactions.TryGetValue(comp.pirateFactionId, out var factionMemory))
            {
                Widgets.Label(new Rect(10f, 10f, size.x - 20f, size.y - 20f), "No faction memory found.");
                return;
            }

            float viewHeight = Mathf.Max(100f, 24f * comp.knownJargon.Count + 30f);
            Rect outRect = new Rect(10f, 10f, size.x - 20f, size.y - 20f);
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, viewHeight);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(viewRect);

            listing.Label("Known Pirate Jargon:");

            foreach (string word in comp.knownJargon)
            {
                var entry = factionMemory.GetJargonInfo(word);
                if (entry != null)
                    listing.Label($"• {entry.JargonWord}: {entry.Meaning}");
                else
                    listing.Label($"• {word}: ???");
            }

            listing.End();
            Widgets.EndScrollView();
        }
    }
}
