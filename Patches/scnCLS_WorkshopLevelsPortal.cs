using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.WorkshopLevelsPortal))]
internal static class scnCLS_WorkshopLevelsPortal
{
    private static void Postfix()
    {
        scnCLS_Start._category = scnCLS.Category.Workshop;
    }
}
