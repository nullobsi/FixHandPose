using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Renderite.Shared;

namespace FixHandPose.Shared
{
    [HarmonyPatch(typeof(VR_ControllerState))]
    class VR_ControllerState_Patch
    {
        private static readonly FieldInfo IsTrackingField = AccessTools.Field(typeof(VR_ControllerState), nameof(VR_ControllerState.isTracking))
            ?? throw new MissingFieldException(typeof(VR_ControllerState).FullName, nameof(VR_ControllerState.isTracking));
        private static readonly FieldInfo HasBoundHandField = AccessTools.Field(typeof(VR_ControllerState), nameof(VR_ControllerState.hasBoundHand))
            ?? throw new MissingFieldException(typeof(VR_ControllerState).FullName, nameof(VR_ControllerState.hasBoundHand));

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(VR_ControllerState.Pack))]
        static IEnumerable<CodeInstruction> Pack_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RemoveBranches(instructions);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(VR_ControllerState.Unpack))]
        static IEnumerable<CodeInstruction> Unpack_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return RemoveBranches(instructions);
        }

        private static IEnumerable<CodeInstruction> RemoveBranches(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = instructions.ToList();
            HashSet<FieldInfo> remainingFields = new() { IsTrackingField, HasBoundHandField };

			// The branch is a `ldarg.0`, `ldfld`, then `brfalse.s` in the
			// DLL. `ldarg.0` loads the object to the stack, and `ldfld`
			// reaplaces the object with a field value on the stack.

			// Thus, we can simply replace the `brfalse.s` instruction
			// with a `pop` instruction to remove the value from the
			// stack and continue execution.

			// There are two such places, reading this.isTracking and
			// this.hasBoundHand.
            for (int i = 0; i < codes.Count - 1; i++)
            {
                FieldInfo? field = codes[i].operand as FieldInfo;
				// We only care about `ldfld` instructions for the two
				// fields specified
                if (field is null || !remainingFields.Contains(field) || !codes[i].LoadsField(field))
                {
                    continue;
                }

				// And, only if they're followed by a `brfalse.s` instruction
                if (codes[i + 1].opcode != OpCodes.Brfalse && codes[i + 1].opcode != OpCodes.Brfalse_S)
                {
                    continue;
                }

				// Replace with a `pop`.
                codes[i + 1].opcode = OpCodes.Pop;
                codes[i + 1].operand = null;
                remainingFields.Remove(field);
            }

            if (remainingFields.Count != 0)
            {
                throw new InvalidOperationException($"Could not remove VR_ControllerState guards for: {string.Join(", ", remainingFields.Select(field => field.Name))}.");
            }

            return codes;
        }
    }
}

