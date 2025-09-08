using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scnEditor), "ResetScene")]
internal static class scnEditor_ResetScene
{
    private static void Postfix() => AdofaiQolMod.Instance.UpdateProgressDisplay(false);
}
