﻿using HamstarHelpers.UIHelpers;
using HamstarHelpers.Utilities.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;


namespace ServerBrowser {
	class ServerBrowserMod : Mod {
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
			if( !Main.dedServ ) {
				this.Dialog = new UIServerBrowserDialog( new UITheme() );

				MenuItem.AddMenuItem( "Browse Servers", -80, 12, delegate () {
					Main.menuMode = 77777;
					this.Dialog.Open();
				} );
				MenuItem.AddMenuItem( "Back", 272, 77777, delegate () {
					Main.menuMode = 12;
					this.Dialog.Close();
				} );
			}

			Main.OnPostDraw += this.DrawBrowser;
		}

		public override void Unload() {
			Main.OnPostDraw -= this.DrawBrowser;
		}

		////////////////

		private void DrawBrowser( GameTime game_time ) {
			if( !Main.gameMenu ) { return; }
			
			Main.spriteBatch.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix );

			this.Dialog.Draw( Main.spriteBatch );

			Vector2 bonus = Main.DrawThickCursor( false );
			Main.DrawCursor( bonus, false );

			Main.spriteBatch.End();
		}
	}
}