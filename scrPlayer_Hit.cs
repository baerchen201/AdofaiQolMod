using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrPlayer), nameof(scrPlayer.Hit))]
internal static class scrPlayer_Hit
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay();
    }
}
