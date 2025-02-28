﻿// -----------------------------------------------------------------------
// <copyright file="Spawn.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace NPCs.Commands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using MEC;
    using UnityEngine;

    /// <inheritdoc />
    public class Spawn : ICommand
    {
        /// <inheritdoc />
        public string Command => "spawn";

        /// <inheritdoc />
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc />
        public string Description => "Spawns a npc.";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Player.Get(sender) is not Player player)
            {
                response = "This command must be executed at the game level.";
                return false;
            }

            if (arguments.Count < 1 || !Enum.TryParse(arguments.At(0), true, out RoleType roleType))
                roleType = RoleType.ClassD;

            string name = "Default";
            if (arguments.Count > 1)
                name = string.Join(" ", arguments.Skip(1));

            Npc npc = new Npc(roleType, name, Vector3.one);
            npc.Spawn();
            Timing.CallDelayed(0.1f, () =>
            {
                npc.Position = player.Position;
                npc.Player.Rotation = player.Rotation;
            });

            response = "Done.";
            return true;
        }
    }
}