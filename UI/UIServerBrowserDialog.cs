using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.UIHelpers.Elements;
using HamstarHelpers.UIHelpers;
using HamstarHelpers.UIHelpers.Elements;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace ServerBrowser.UI {
	partial class UIServerBrowserDialog : UIDialog {
		private UITextPanel<string> Header;

		private UITextPanelButton SortByNameButton;
		private UITextPanelButton SortByPingButton;
		private UITextPanelButton SortByPlayersButton;
		private UITextField FilterByModInput;
		private UITextField FilterByNameInput;
		private UIText ServerListErr;

		private UIServerBrowserList ServerList;
		
		private int LastSeenScreenWidth;
		private int LastSeenScreenHeight;


		////////////////

		public UIServerBrowserDialog( UITheme theme ) : base( theme, 768, 346 ) {
			this.LastSeenScreenWidth = Main.screenWidth;
			this.LastSeenScreenHeight = Main.screenHeight;
		}


		////////////////

		public override void OnActivate() {
			Action<string, int> pre_join = ( ip, port ) => {
				this.Close();
			};

			Action on_success = () => {
				this.ServerListErr.SetText( "" );
			};

			Action on_err = () => {
				this.ServerListErr.SetText( "Server busy. Try again later." );
			};

			base.OnActivate();
			this.ServerList.RefreshServerList( pre_join, on_success, on_err );
		}


		////////////////

		public override void RefreshTheme() {
			base.RefreshTheme();

			this.Theme.ApplyHeader( this.Header );

			this.SortByNameButton.RefreshTheme();
			this.SortByPingButton.RefreshTheme();
			this.SortByPlayersButton.RefreshTheme();
			this.FilterByModInput.RefreshTheme();
			this.FilterByNameInput.RefreshTheme();
			this.ServerList.RefreshTheme();
		}
		
		////////////////

		public override void Draw( SpriteBatch sb ) {
			if( this.LastSeenScreenWidth != Main.screenWidth || this.LastSeenScreenHeight != Main.screenHeight ) {
				this.LastSeenScreenWidth = Main.screenWidth;
				this.LastSeenScreenHeight = Main.screenHeight;

				this.RecalculateMe();
			}

			base.Draw( sb );
		}
	}
}
