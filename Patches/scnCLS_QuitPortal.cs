using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.QuitPortal))]
internal static class scnCLS_QuitPortal
{
    private static void Postfix()
    {
        // no need to check if this exits to main menu, either way this should be reset
        scnCLS_Start._category = default;
    }
}
