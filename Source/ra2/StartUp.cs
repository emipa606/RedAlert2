using System.Reflection;
using HarmonyLib;
using Verse;

namespace ra2;

[StaticConstructorOnStartup]
public static class StartUp
{
    static StartUp()
    {
        new Harmony("Red Alert 2 in Rimworld").PatchAll(Assembly.GetExecutingAssembly());
    }
}