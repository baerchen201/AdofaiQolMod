using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrPressToStart), nameof(scrPressToStart.ShowText))]
public class scrPressToStart_ShowText
{
    private static void Postfix() => AdofaiQolMod.Instance.UpdateProgressDisplay();
}
