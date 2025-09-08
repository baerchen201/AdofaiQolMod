using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrController), nameof(scrController.ShowHitText))]
internal static class scrController_ShowHitText
{
    private static bool Prefix(ref HitMargin hitMargin) =>
        hitMargin != HitMargin.Perfect || !AdofaiQolMod.Instance.HidePerfect;
}
