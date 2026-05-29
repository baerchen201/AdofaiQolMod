using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scrHitTextManager), nameof(scrHitTextManager.ShowHitText))]
internal static class scrHitTextManager_ShowHitText
{
    private static bool Prefix(ref HitMargin hitMargin)
    {
        return hitMargin != HitMargin.Perfect || !AdofaiQolMod.Instance.HidePerfect;
    }
}
