using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrCalibrationPlanet), "Start")]
internal static class scrCalibrationPlanet_Start
{
    private static void Postfix() => AdofaiQolMod.Instance.UpdateProgressDisplay(false);
}
