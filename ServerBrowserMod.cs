using HamstarHelpers.DebugHelpers;
using HamstarHelpers.UIHelpers;
using HamstarHelpers.Utilities.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ServerBrowser.UI;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ServerBrowser {
	class ServerBrowserMod : Mod {
		public static ServerBrowserMod Instance { get; private set; }

		public static string GithubUserName { get { return "hamstar0"; } }
		public static string GithubProjectName { get { return "tml-serverbrowser-mod"; } }


		////////////////

		private UIServerBrowserDialog Dialog;


		////////////////

		public ServerBrowserMod() {
			this.Properties = new ModProperties() {
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		////////////////

		public override void Load() {
			ServerBrowserMod.Instance = this;

			if( !Main.dedServ ) {
				var theme = new UITheme();
				theme.UrlColor = Color.Lerp( theme.UrlColor, Color.White, 0.5f );
				theme.UrlLitColor = Color.Lerp( theme.UrlLitColor, Color.White, 0.5f );
				theme.UrlVisitColor = Color.Lerp( theme.UrlVisitColor, Color.White, 0.5f );
				
				this.Dialog = new UIServerBrowserDialog( theme );

				MenuItem.AddMenuItem( "Browse Servers", MenuItem.MenuTopPos - 80, 12, delegate() {
					Main.OpenPlayerSelect( plr_data => {
						Main.ServerSideCharacter = false;
						plr_data.SetAsActive();

						Main.menuMode = 77777;
						this.Dialog.Open();
					} );
					//Main.LoadPlayers();
					//Main.menuMultiplayer = true;
					//Main.PlaySound( 10, -1, -1, 1, 1f, 0f );
					//Main.menuMode = 1;

					//Main.menuMode = 77777;
					//this.Dialog.Open();
				} );

				MenuItem.AddMenuItem( "Back", -78, 77777, delegate () {
					Main.menuMode = 12;
					this.Dialog.Close();
				} );

				Main.OnPostDraw += ServerBrowserMod._DrawBrowser;
			}
		}

		public override void Unload() {
			if( !Main.dedServ ) {
				this.Dialog.Close();	// Just in case?

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
