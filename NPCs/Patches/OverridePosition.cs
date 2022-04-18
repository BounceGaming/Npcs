﻿// -----------------------------------------------------------------------
// <copyright file="OverridePosition.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace NPCs.Patches
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using UnityEngine;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerMovementSync.OverridePosition"/> to prevent calls to <see cref="PlayerMovementSync.ForceRotation"/> when the player is a npc.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))]
    internal static class OverridePosition
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label skipRotationLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.OperandIs(PropertyGetter(typeof(Vector3), nameof(Vector3.up))));
            newInstructions.RemoveRange(index, 2);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.transform))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localScale))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.y))),
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Newobj, Constructor(typeof(Vector3), new[] { typeof(float), typeof(float), typeof(float) })),
                new CodeInstruction(OpCodes.Ldc_R4, 1.3f),
            });

            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarga_S);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Npc), nameof(Npc.Dictionary))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.gameObject))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<GameObject, Npc>), nameof(Dictionary<GameObject, Npc>.ContainsKey))),
                new CodeInstruction(OpCodes.Brtrue_S, skipRotationLabel),
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_0);
            newInstructions[index].labels.Add(skipRotationLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}