using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scrPressToStart), nameof(scrPressToStart.ShowText))]
internal static class scrPressToStart_ShowText
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay();
    }
}
