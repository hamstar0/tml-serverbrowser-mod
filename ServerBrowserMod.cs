﻿using HamstarHelpers.Components.UI;
using HamstarHelpers.Components.UI.Menu;
using HamstarHelpers.Helpers.DebugHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ServerBrowser.UI;
using ServerBrowser.Reporter;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ServerBrowser {
	partial class ServerBrowserMod : Mod {
		public static ServerBrowserMod Instance { get; private set; }



		////////////////

		private UIServerBrowserDialog Dialog;
		internal ServerBrowserReporter Reporter;



		////////////////

		public ServerBrowserMod() { }

		////////////////

		public override void Load() {
			ServerBrowserMod.Instance = this;

			this.Reporter = new ServerBrowserReporter();

			if( !Main.dedServ ) {
				var theme = new UITheme();
				theme.UrlColor = Color.Lerp( theme.UrlColor, Color.White, 0.5f );
				theme.UrlLitColor = Color.Lerp( theme.UrlLitColor, Color.White, 0.5f );
				theme.UrlVisitColor = Color.Lerp( theme.UrlVisitColor, Color.White, 0.5f );
				
				this.Dialog = new UIServerBrowserDialog( theme );

				MenuItem.AddMenuItem( "Browse Servers", MenuItem.MenuTopPos - 80, 12, () => {
					Main.OpenPlayerSelect( plr_data => {
						Main.ServerSideCharacter = false;
						plr_data.SetAsActive();

						Main.menuMode = 77777;
						ServerBrowserMod.Instance.Dialog.Open();
					} );
				} );

				MenuItem.AddMenuItem( "Back", -78, 77777, delegate () {
					Main.menuMode = 12;
					ServerBrowserMod.Instance.Dialog.Close();
				} );

				Main.OnPostDraw += ServerBrowserMod._DrawBrowser;
			}
		}

		public override void Unload() {
			if( !Main.dedServ ) {
				//this.Dialog.Close();    // Just in case?
				this.Dialog = null;

				Main.ClearPendingPlayerSelectCallbacks();
				Main.OnPostDraw -= ServerBrowserMod._DrawBrowser;
			}

			ServerBrowserMod.Instance = null;
		}

		////////////////

		private static void _DrawBrowser( GameTime game_time ) {
			if( ServerBrowserMod.Instance != null ) {
				ServerBrowserMod.Instance.DrawBrowser( game_time );
			}
		}

		private bool HasErrored = false;

		private void DrawBrowser( GameTime game_time ) {
			if( !Main.gameMenu ) { return; }
			
			Main.spriteBatch.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix );

			try {
				this.Dialog.Draw( Main.spriteBatch );
			} catch( Exception e ) {
				if( !this.HasErrored ) {
					this.HasErrored = true;
					LogHelpers.Log( e.ToString() );
				}
			}

			Vector2 bonus = Main.DrawThickCursor( false );
			Main.DrawCursor( bonus, false );

			Main.spriteBatch.End();
		}
	}
}
