using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrCalibrationLine), nameof(scrCalibrationLine.Awake))]
internal static class scrCalibrationLine_Awake
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(false);
    }
}
