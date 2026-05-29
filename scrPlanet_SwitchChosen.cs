using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrPlanet), nameof(scrPlanet.SwitchChosen))]
internal static class scrPlanet_SwitchChosen
{
    private static void Postfix(ref scrController __instance)
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(__instance);
    }
}
