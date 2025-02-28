// -----------------------------------------------------------------------
// <copyright file="Destroy.cs" company="Build">
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
    using Exiled.Permissions.Extensions;

    /// <inheritdoc />
    public class Destroy : ICommand
    {
        /// <inheritdoc />
        public string Command => "destroy";

        /// <inheritdoc />
        public string[] Aliases { get; } = { "d" };

        /// <inheritdoc />
        public string Description => "Destruye un NPC.";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("npc.destroy"))
            {
                response = "No tienes permisos para usar este comando.";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Uso: npc destroy <id>";
                return false;
            }

            Player pNpc = Player.Get(arguments.ElementAt(0));

            if (!Npc.Dictionary.TryGetValue(pNpc.GameObject, out Npc n))
            {
                response = "Ese jugador no es un NPC!";
                return false;
            }

            n.Destroy();
            response = "NPC Destruido.";
            return true;
        }
    }
}