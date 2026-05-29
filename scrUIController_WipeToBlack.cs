using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrUIController), nameof(scrUIController.WipeToBlack))]
internal static class scrUIController_WipeToBlack
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(false);
    }
}
