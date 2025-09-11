using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using HarmonyLib;

namespace AdofaiQolMod;

[HarmonyPatch(typeof(scnCLS), "CreateFloors")]
internal static class scnCLS_CreateFloors
{
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator generator
    ) =>
        new CodeMatcher(instructions, generator)
            .MatchForward(
                false,
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(File), nameof(File.Exists)))
            )
            .SetInstruction(
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(scnCLS_CreateFloors), nameof(a))
                )
            )
            .MatchForward(
                false,
                new CodeMatch(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(File),
                        nameof(File.WriteAllLines),
                        [typeof(string), typeof(IEnumerable<string>)]
                    )
                )
            )
            .Advance(1)
            .MatchForward(
                false,
                new CodeMatch(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(File),
                        nameof(File.WriteAllLines),
                        [typeof(string), typeof(IEnumerable<string>)]
                    )
                )
            )
            .SetInstruction(
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(scnCLS_CreateFloors), nameof(b))
                )
            )
            .InstructionEnumeration();

    private static bool a(string _) => false;

    private static void b(string path, IEnumerable<string> contents) { }
}
