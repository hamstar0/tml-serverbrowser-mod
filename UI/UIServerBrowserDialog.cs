using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.UIHelpers.Elements;
using HamstarHelpers.UIHelpers;
using HamstarHelpers.UIHelpers.Elements;
using HamstarHelpers.Utilities.Menu;
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

		public override void InitializeComponents() {
			var self = this;
			var self_theme = this.Theme.Clone();

			this.OuterContainer.Height.Set( -256, 1f );
			this.OuterContainer.MaxHeight.Set( -256, 1f );
			this.SetTopPosition( MenuItem.MenuTopPos - 68f, 0f, false );

			this.RecalculateMe();

			////

			this.Header = new UITextPanel<string>( "Server Browser", 0.8f, true );
			this.Header.HAlign = 0.5f;
			this.Header.Top.Set( -35f, 0f );
			this.Header.SetPadding( 15f );
			this.OuterContainer.Append( (UIElement)this.Header );

			this.Theme.ApplyHeader( this.Header );

			////

			var theme_button_theme = UITheme.Vanilla.Clone();
			var theme_button = new UITextPanelButton( theme_button_theme, "R", 0.8f );
			theme_button.Top.Set( -8f, 0f );
			theme_button.Left.Set( -12f, 1f );
			theme_button.Width.Set( 16f, 0f );
			theme_button.Height.Set( 16f, 0f );
			theme_button.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
				if( theme_button.Text == "R" ) {
					theme_button.SetText( "B" );
					this.Theme.Switch( UITheme.Vanilla );
					theme_button_theme.Switch( self_theme );
				} else {
					theme_button.SetText( "R" );
					this.Theme.Switch( self_theme );
					theme_button_theme.Switch( UITheme.Vanilla );
				}
				theme_button.RefreshTheme();
				this.RefreshTheme();
			};
			this.InnerContainer.Append( (UIElement)theme_button );

			////

			this.SortByNameButton = new UITextPanelButton( this.Theme, "Sort by name", 1.2f );
			this.SortByNameButton.Top.Set( 12f, 0f );
			this.SortByNameButton.Left.Set( 0f, 0f );
			this.SortByNameButton.Width.Set( 128f, 0f );
			this.SortByNameButton.Height.Set( 32f, 0f );
			this.SortByNameButton.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
				this.SortServerListByName();
			};
			this.InnerContainer.Append( (UIElement)this.SortByNameButton );

			this.SortByPingButton = new UITextPanelButton( this.Theme, "Sort by ping", 1.2f );
			this.SortByPingButton.Top.Set( 12f, 0f );
			this.SortByPingButton.Left.Set( 128f + 12f, 0f );
			this.SortByPingButton.Width.Set( 128f, 0f );
			this.SortByPingButton.Height.Set( 32f, 0f );
			this.SortByPingButton.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
				this.SortServerListByPing();
			};
			this.InnerContainer.Append( (UIElement)this.SortByPingButton );

			this.SortByPlayersButton = new UITextPanelButton( this.Theme, "Sort by players", 1.2f );
			this.SortByPlayersButton.Top.Set( 12f, 0f );
			this.SortByPlayersButton.Left.Set( 256f + 16f, 0f );
			this.SortByPlayersButton.Width.Set( 136f, 0f );
			this.SortByPlayersButton.Height.Set( 32f, 0f );
			this.SortByPlayersButton.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
				this.SortServerListByPlayers();
			};
			this.InnerContainer.Append( (UIElement)this.SortByPlayersButton );

			this.FilterByModInput = new UITextField( this.Theme, "Filter by mod" );
			this.FilterByModInput.Top.Set( 12f, 0f );
			this.FilterByModInput.Left.Set( -320f, 1f );
			this.FilterByModInput.Width.Set( 160f, 0f );
			this.FilterByModInput.Height.Set( 32f, 0f );
			this.FilterByModInput.PaddingTop += 4f;
			this.FilterByModInput.OnTextChange += delegate ( object sender, EventArgs e ) {
				this.FilterServerListByMod( ((TextInputEventArgs)e).Text );
			};
			this.InnerContainer.Append( (UIElement)this.FilterByModInput );

			this.FilterByNameInput = new UITextField( this.Theme, "Filter by text" );
			this.FilterByNameInput.Top.Set( 12f, 0f );
			this.FilterByNameInput.Left.Set( -160f, 1f );
			this.FilterByNameInput.Width.Set( 160f, 0f );
			this.FilterByNameInput.Height.Pixels = 32f;
			this.FilterByNameInput.PaddingTop += 4f;
			this.FilterByNameInput.OnTextChange += delegate( object sender, EventArgs e ) {
				this.FilterServerListByText( ((TextInputEventArgs)e).Text );
			};
			this.InnerContainer.Append( (UIElement)this.FilterByNameInput );

			////

			var server_name_col_label = new UIText( "World Name", 0.75f );
			server_name_col_label.Top.Set( 46f, 0f );
			server_name_col_label.Left.Set( UIServerDataElement.WorldLabelLeft + 8f, 0f );
			this.InnerContainer.Append( (UIElement)server_name_col_label );
			
			var uptime_col_label = new UIText( "Up Time", 0.75f );
			uptime_col_label.Top.Set( 46f, 0f );
			uptime_col_label.Left.Set( UIServerDataElement.UptimeLabelLeft + 8f, 0f );
			this.InnerContainer.Append( (UIElement)uptime_col_label );

			var ping_col_label = new UIText( "Ping", 0.75f );
			ping_col_label.Top.Set( 46f, 0f );
			ping_col_label.Left.Set( UIServerDataElement.PingLabelLeft + 8f, 0f );
			this.InnerContainer.Append( (UIElement)ping_col_label );

			var prog_col_label = new UIText( "Progress", 0.75f );
			prog_col_label.Top.Set( 46f, 0f );
			prog_col_label.Left.Set( UIServerDataElement.WorldProgressLabelLeft + 8f, 0f );
			this.InnerContainer.Append( (UIElement)prog_col_label );

			var event_col_label = new UIText( "Current Event", 0.75f );
			event_col_label.Top.Set( 46f, 0f );
			event_col_label.Left.Set( UIServerDataElement.WorldEventLabelLeft + 8f, 0f );
			this.InnerContainer.Append( (UIElement)event_col_label );

			////

			this.ServerList = new UIServerBrowserList( this.Theme );
			this.ServerList.Top.Set( 60f, 0f );
			this.ServerList.Width.Set( 0f, 1f );
			this.ServerList.Height.Set( -74f, 1f );
			this.ServerList.HAlign = 0f;
			this.ServerList.SetPadding( 4f );
			this.ServerList.PaddingTop = 0.0f;
			this.ServerList.Initialize();
			this.InnerContainer.Append( (UIElement)this.ServerList );

			this.ServerListErr = new UIText( "", 1f );
			this.ServerListErr.Top.Set( 60f, 0f );
			this.ServerListErr.Left.Set( -128, 0.5f );
			this.InnerContainer.Append( (UIElement)this.ServerListErr );
			
			////

			var modrecommend_url = new UIWebUrl( this.Theme, "Trouble choosing mods?", "https://sites.google.com/site/terrariamodsuggestions/", true, 0.86f );
			modrecommend_url.Top.Set( -12f, 1f );
			modrecommend_url.Left.Set( 0f, 0f );
			modrecommend_url.Width.Set( 172f, 0f );
			this.InnerContainer.Append( (UIElement)modrecommend_url );

			var support_url = new UIWebUrl( this.Theme, "Support Server Browser!", "https://www.patreon.com/hamstar0", true, 0.86f );
			support_url.Top.Set( -12f, 1f );
			support_url.Left.Set( -172f, 1f );
			support_url.Width.Set( 172f, 0f );
			this.InnerContainer.Append( (UIElement)support_url );
		}


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
