using ADOFAI;
using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnGame), nameof(scnGame.ApplyEvent))]
internal static class scnGame_ApplyEvent
{
    private static bool Prefix(ref LevelEvent evnt)
    {
        return AdofaiQolMod.Instance.AllowEvent(evnt);
    }
}
