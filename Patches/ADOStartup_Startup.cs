using System;
using System.Globalization;
using HarmonyLib;

namespace AdofaiQolMod.Patches;

[HarmonyPatch(typeof(ADOStartup), nameof(ADOStartup.Startup))]
internal static class ADOStartup_Startup
{
    private static void Postfix()
    {
        if (
            !DateTime.TryParseExact(
                Releases.buildDate,
                "yyyy/MM/dd h:mm tt",
                null,
                DateTimeStyles.None,
                out var buildDate
            )
        )
        {
            AdofaiQolMod.Logger.LogWarning(
                $"Couldn't adjust game build time (unknown format): {Releases.buildDate}"
            );
            return;
        }

        Releases.buildDate = buildDate.ToString("yyyy-MM-dd HH:mm");
    }
}
