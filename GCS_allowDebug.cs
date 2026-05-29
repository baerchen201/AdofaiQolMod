using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(GCS), nameof(GCS.allowDebug), MethodType.Getter)]
internal static class GCS_allowDebug
{
    private static void Postfix(ref bool __result)
    {
        if (AdofaiQolMod.Instance.OverrideAllowDebug)
            __result = true;
    }
}
