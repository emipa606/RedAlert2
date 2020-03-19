using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Verse;

namespace ra2
{
    [StaticConstructorOnStartup]
    public static class StartUp
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        static StartUp()
        {
            new Harmony("Red Alert 2 in Rimworld").PatchAll(Assembly.GetExecutingAssembly());
          
            // ((Texture2D[])typeof(Thing).Assembly.GetType("Verse.TexButton").GetField("SpeedButtonTextures").GetValue(null))[4] = ContentFinder<Texture2D>.Get("UI/TimeControls/TimeSpeedButton_Ultrafast", true);
        }
    }
}
