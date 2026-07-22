using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.TechFeaturedLevelsPortal))]
internal static class scnCLS_TechFeaturedLevelsPortal
{
    private static void Postfix()
    {
        scnCLS_Start._category = scnCLS.Category.Tech;
    }
}
