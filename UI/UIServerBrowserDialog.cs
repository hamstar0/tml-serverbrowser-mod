using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.UIHelpers.Elements;
using HamstarHelpers.NetHelpers;
using HamstarHelpers.UIHelpers;
using HamstarHelpers.UIHelpers.Elements;
using HamstarHelpers.Utilities.Menu;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace ServerBrowser.UI {
	public class UIServerBrowserDialog : UIDialog {
		private static object MyLock = new object();


		private UITextPanelButton SortByNameButton;
		private UITextPanelButton SortByPingButton;
		private UITextPanelButton SortByPlayersButton;
		private UITextField FilterByNameInput;
		private UIList ServerList;

		private UIServerModListPopup ModListPopup;


		////////////////

		public UIServerBrowserDialog( UITheme theme ) : base( theme, 768, 346 ) { }

		////////////////

		public override void InitializeComponents() {
			var self = this;

			this.SetTopPosition( MenuItem.MenuTopPos - 64f, 0f, false );
			
			var title = new UITextPanel<string>( "Server Browser", 0.8f, true );
			title.HAlign = 0.5f;
			title.Top.Set( -35f, 0f );
			title.SetPadding( 15f );
			this.OuterContainer.Append( (UIElement)title );

			this.Theme.ApplyPanel( title );
			title.BackgroundColor.A = 255;

			this.SortByNameButton = new UITextPanelButton( this.Theme, "Sort by name" );
			this.SortByNameButton.Top.Set( 12f, 0f );
			this.SortByNameButton.Left.Set( 0f, 0f );
			this.SortByNameButton.Width.Set( 128f, 0f );
			this.SortByNameButton.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
			};
			this.InnerContainer.Append( (UIElement)this.SortByNameButton );

			this.SortByPingButton = new UITextPanelButton( this.Theme, "Sort by ping" );
			this.SortByPingButton.Top.Set( 12f, 0f );
			this.SortByPingButton.Left.Set( 128f + 8f, 0f );
			this.SortByPingButton.Width.Set( 128f, 0f );
			this.SortByPingButton.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
			};
			this.InnerContainer.Append( (UIElement)this.SortByPingButton );

			this.SortByPlayersButton = new UITextPanelButton( this.Theme, "Sort by players" );
			this.SortByPlayersButton.Top.Set( 12f, 0f );
			this.SortByPlayersButton.Left.Set( 256f + 16f, 0f );
			this.SortByPlayersButton.Width.Set( 136f, 0f );
			this.SortByPlayersButton.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
			};
			this.InnerContainer.Append( (UIElement)this.SortByPlayersButton );

			this.FilterByNameInput = new UITextField( this.Theme, "Filter by name" );
			this.FilterByNameInput.Top.Set( 0f, 0f );
			this.FilterByNameInput.Left.Set( -160f, 1f );
			this.FilterByNameInput.Width.Set( 160f, 0f );
			this.FilterByNameInput.Height.Pixels = 40f;
			this.FilterByNameInput.PaddingTop += 4f;
			//this.FilterByNameInput.HAlign = 0f;
			this.FilterByNameInput.OnTextChange += delegate( object sender, EventArgs e ) {
			};
			this.InnerContainer.Append( (UIElement)this.FilterByNameInput );
			
			var mod_list_panel = new UIPanel();
			{
				mod_list_panel.Top.Set( 44f, 0f );
				mod_list_panel.Width.Set( 0f, 1f );
				mod_list_panel.Height.Set( 256f, 0f );
				mod_list_panel.HAlign = 0f;
				mod_list_panel.SetPadding( 4f );
				mod_list_panel.PaddingTop = 0.0f;
				this.InnerContainer.Append( (UIElement)mod_list_panel );

				this.ServerList = new UIList();
				{
					this.ServerList.Width.Set( -25, 1f );
					this.ServerList.Height.Set( 0f, 1f );
					this.ServerList.HAlign = 0f;
					this.ServerList.ListPadding = 4f;
					this.ServerList.SetPadding( 0f );
					mod_list_panel.Append( (UIElement)this.ServerList );

					UIScrollbar scrollbar = new UIScrollbar();
					{
						scrollbar.Top.Set( 8f, 0f );
						scrollbar.Height.Set( -16f, 1f );
						scrollbar.SetView( 100f, 1000f );
						scrollbar.HAlign = 1f;
						mod_list_panel.Append( (UIElement)scrollbar );

						this.ServerList.SetScrollbar( scrollbar );
					}
				}
			}

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

			////

			/*this.ModListPopup = new UIServerModListPopup( this.Theme );
			this.ModListPopup.Top.Set( 0f, 0f );
			this.ModListPopup.Left.Set( 0f, 0f );
			this.Append( (UIElement)this.ModListPopup );*/

			////

			this.Theme.ApplyList( mod_list_panel );
		}


		public override void OnActivate() {
			base.OnActivate();
			this.RefreshServerList();
		}

		////////////////

		public void RefreshServerList() {
			//lock( UIServerBrowserDialog.MyLock ) {
				if( this.ServerList.Count > 0 ) {
					this.ServerList.Clear();
					this.ServerList.Recalculate();
				}
			//}
			
			Action<string> list_ready = delegate( string output ) {
				UIServerDataElement[] list = UIServerDataElement.GetListFromJsonStr( this.Theme, output, (ip, port) => {
					this.Close();
				} );

				if( list.Length > 0 ) {
					//lock( UIServerBrowserDialog.MyLock ) {
						this.ServerList.AddRange( list );
						this.Recalculate();
					//}
				}
			};
			Action<Exception, string> list_error = delegate ( Exception e, string output ) {
				// TODO: Add error display to list UI
				LogHelpers.Log( "List could not load " + e.ToString() );
			};

			NetHelpers.MakeGetRequestAsync( "https://script.google.com/macros/s/AKfycbzQl2JmJzdEHguVI011Hk1KuLktYJPDzpWA_tDbyU_Pk02fILUw/exec",
				list_ready, list_error );
		}


		////////////////

		/*public override void Draw( SpriteBatch sb ) {
			lock( UIServerBrowserDialog.MyLock ) {
				base.Draw( sb );

				if( this.ServerList != null ) {
					foreach( UIElement item in this.ServerList._items ) {
						if( item.IsMouseHovering ) {
							var server_data_elem = (UIServerDataElement)item;

							this.ModListPopup.SetServer( server_data_elem.Data );
						}
					}
				}
			}
		}*/
	}
}
