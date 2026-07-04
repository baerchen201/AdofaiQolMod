using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnGame), nameof(scnGame.Play))]
internal static class scnGame_Play
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay();
    }
}
