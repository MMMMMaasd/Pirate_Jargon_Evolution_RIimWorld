using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using RestSharp;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Verse.AI;

namespace PirateJargonEvolution
{
        public class OllamaSettings : ModSettings
        {
            public string modelName = "llama3";
            public string endpoint = "http://localhost:11434";
            public bool enableMod = true;
            public float temperature = 0.7f;
        }

        public class PirateJargonEvolution : Mod
        {
            public static OllamaSettings settings;

            public PirateJargonEvolution(ModContentPack content) : base(content)
            {
                settings = new OllamaSettings();
                var harmony = new Harmony("com.michael.piratejargon");
                harmony.PatchAll();
                Log.Message("Loaded Pirate Jargon Evolution Mod");
            }

            public override string SettingsCategory()
            {
                return "Social Interaction LLM";
            }
        }
    
}