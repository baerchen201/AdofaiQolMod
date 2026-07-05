#if DEBUG
using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(GCS), nameof(GCS.allowDebug), MethodType.Getter)]
internal static class GCS_allowDebug
{
    private static bool Prefix(ref bool __result)
    {
        if (AdofaiQolMod.Instance.OverrideAllowDebug)
        {
            __result = true;
            return false;
        }

        return true;
    }
}
#endif
