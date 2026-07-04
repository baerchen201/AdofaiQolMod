using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scrUIController), nameof(scrUIController.WipeToBlack))]
internal static class scrUIController_WipeToBlack
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(false);
    }
}
