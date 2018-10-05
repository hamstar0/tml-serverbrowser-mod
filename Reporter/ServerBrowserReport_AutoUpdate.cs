using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Services.Timers;
using Microsoft.Xna.Framework;
using System;
using Terraria;


namespace ServerBrowser.Reporter {
	partial class ServerBrowserReporter {
		public static bool CanPromptForBrowserAdd() {
			//return ServerBrowserMod.Instance.Config.IsServerPromptingUsersBeforeListingOnBrowser
			//	&& !ServerBrowserMod.Instance.Config.IsServerHiddenFromBrowser;
			return true;
		}



		////////////////
		
		private void InitializeLoopingServerAnnounce() {
			if( Main.netMode == 0 ) { return; }
				
			Action alert_privacy = delegate {
				string msg = "Server Browser would like to publicly list your server. Type '/sb-private-server' in the chat or server console to cancel this. Otherwise, do nothing for 60 seconds.";

				Main.NewText( msg, Color.Yellow );
				Console.WriteLine( msg );
			};

			if( Main.netMode == 1 ) {
				if( ServerBrowserReporter.CanPromptForBrowserAdd() ) {
					//	3 seconds
					Timers.SetTimer( "ServerBrowserIntro", 60 * 3, delegate {
						alert_privacy();
						return false;
					} );
				}
			}

			if( ServerBrowserReporter.CanAnnounceServer() ) {
				if( ServerBrowserReporter.CanPromptForBrowserAdd() ) {
					Timers.SetTimer( "ServerBrowserReport", 60 * 60, delegate {   // 1 minute, no repeat
						try {
							this.BeginLoopingServerAnnounce();
						} catch { }

						return false;
					} );
				} else {
					this.BeginLoopingServerAnnounce();
				}
			}
		}

		
		private void BeginLoopingServerAnnounce() {
			if( Main.netMode != 2 ) { return; }

			// First time no timer
			if( ServerBrowserReporter.CanAnnounceServer() ) {
				ServerBrowserReporter.AnnounceServer();
			}
			
			Timers.SetTimer( "ServerBrowserReport", (60 * 10) * 60, delegate {  // 10 minutes
				if( ServerBrowserReporter.CanAnnounceServer() ) {
					if( !ServerBrowserReporter.IsHammering() ) {
						ServerBrowserReporter.AnnounceServer();
					}
				}
				return true;
			} );
		}


		////////////////

		internal void StopLoopingServerAnnounce() {
			Timers.UnsetTimer( "ServerBrowserReport" );
			//this.IsSendingUpdates = false;
		}
	}
}
