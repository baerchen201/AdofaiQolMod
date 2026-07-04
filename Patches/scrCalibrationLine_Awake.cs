using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scrCalibrationLine), nameof(scrCalibrationLine.Awake))]
internal static class scrCalibrationLine_Awake
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.UpdateProgressDisplay(false);
    }
}
