using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scrUIController), nameof(scrUIController.WipeFromBlack))]
internal static class scrUIController_WipeFromBlack
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay();
    }
}
