using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrVersionText), nameof(scrVersionText.UpdatePage))]
internal static class scrVersionText_UpdatePage
{
    private static bool Prefix(ref scrVersionText __instance)
    {
        __instance.page = (__instance.page + 1) % 3;
        __instance.text.text = __instance.page switch
        {
            1 => __instance.page1,
            2 => AdofaiQolMod.GetVersionText(),
            _ => __instance.page0,
        };
        return false;
    }
}
