﻿using HamstarHelpers.Helpers.DebugHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace ServerBrowser.Commands {
	class ServerBrowserPrivateCommand : ModCommand {
		public override CommandType Type {
			get {
				return CommandType.Console | CommandType.World;
			}
		}
		public override string Command { get { return "sb-private-server"; } }
		public override string Usage { get { return "/" + this.Command; } }
		public override string Description { get { return "Sets current server to not be listed on the server browser."; } }


		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			if( Main.netMode == 1 ) {
				LogHelpers.Log( "ServerBrowserPrivateCommand - Not supposed to run on client." );
				return;
			}

			var mymod = (ServerBrowserMod)this.mod;

			if( /*!mymod.Config.IsServerPromptingUsersBeforeListingOnBrowser &&*/ (caller.CommandType & CommandType.Console) == 0 ) {
				caller.Reply( "Cannot set server private; grace period has expired. Set \"IsServerHiddenFromBrowser: true\" in config file, then restart server." );
				return;
			}

			mymod.Reporter.StopLoopingServerAnnounce();

			//caller.Reply( "Server set private. For future servers, set \"IsServerHiddenFromBrowser: true\" in the Mod Helpers config settings.", Color.GreenYellow );
			caller.Reply( "Server set private. For permanent private servers, Server Browser mod must be disabled (for now).", Color.GreenYellow );
		}
	}
}
