using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.DeactivateCustomLevelModifiers))]
internal static class scnCLS_DeactivateCustomLevelModifiers
{
    private static void Postfix()
    {
        AdofaiQolMod.Instance.useLDM = false;
    }
}
