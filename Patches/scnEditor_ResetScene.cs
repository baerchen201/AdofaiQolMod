using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnEditor), nameof(scnEditor.ResetScene))]
internal static class scnEditor_ResetScene
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(false);
    }
}
