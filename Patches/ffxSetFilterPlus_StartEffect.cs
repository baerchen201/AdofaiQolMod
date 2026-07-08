using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(ffxSetFilterPlus), nameof(ffxSetFilterPlus.StartEffect))]
internal static class ffxSetFilterPlus_StartEffect
{
    private static bool Prefix(ref ffxSetFilterPlus __instance)
    {
        return !AdofaiQolMod.Instance.SuppressFilter(__instance.filter);
    }
}
