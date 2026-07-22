using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.FeaturedLevelsPortal))]
internal static class scnCLS_FeaturedLevelsPortal
{
    private static void Postfix()
    {
        scnCLS_Start._category = scnCLS.Category.Featured;
    }
}
