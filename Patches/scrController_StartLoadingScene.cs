using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scrController), nameof(scrController.StartLoadingScene))]
internal static class scrController_StartLoadingScene
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(false);
    }
}
