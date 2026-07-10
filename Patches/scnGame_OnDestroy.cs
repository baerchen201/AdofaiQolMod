using System;
using HarmonyLib;
using Steamworks;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(scnGame), nameof(scnGame.OnDestroy))]
internal static class scnGame_OnDestroy
{
    private static void Postfix()
    {
#if DEBUG
        AdofaiQolMod.Logger.LogDebug("Stopping all playtime tracking...");
#endif
        try
        {
            SteamUGC.StopPlaytimeTrackingForAllItems();
        }
        catch (Exception e)
        {
            AdofaiQolMod.Logger.LogError($"Failed to stop playtime tracking: {e}");
        }
    }
}
