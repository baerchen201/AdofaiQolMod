using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnCLS), nameof(scnCLS.Awake))]
internal static class scnCLS_Awake
{
    private static void Postfix()
    {
        var instance = AdofaiQolMod.Instance;
        instance.useLDM = instance.LDMLevel;
    }
}
