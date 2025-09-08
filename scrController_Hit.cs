using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrController), nameof(scrController.Hit))]
internal static class scrController_Hit
{
    private static void Postfix(ref scrController __instance)
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(__instance);
    }
}
