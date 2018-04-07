using HamstarHelpers.DebugHelpers;
using HamstarHelpers.NetHelpers;
using HamstarHelpers.UIHelpers;
using HamstarHelpers.Utilities.Config;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace ServerBrowser.UI {
	class UIServerBrowserList : UIPanel {
		private static object MyLock = new object();


		////////////////

		public static UIServerDataElement[] GetListFromJsonStr( UITheme theme, string json_str,
				Func<UIServerDataElement, UIServerDataElement, int> comparator,
				Action<string, int> pre_join ) {
			try {
				var data = JsonConfig<IDictionary<string, ServerBrowserEntry>>.Deserialize( json_str );
				UIServerDataElement[] list = new UIServerDataElement[data.Count];

				int i = 0;
				foreach( var kv in data ) {
					list[i++] = new UIServerDataElement( theme, kv.Value, comparator, pre_join );
				}

				return list;
			} catch( Exception e ) {
				int len = json_str.Length > 64 ? 64 : json_str.Length;
				LogHelpers.Log( "GetListFromJsonStr - " + e.ToString() + " - " + json_str.Substring( 0, len ) );
			}

			return new UIServerDataElement[0];
		}


		////////////////

		private UITheme Theme;
		private UIList MyList;

		public ICollection<UIServerDataElement> FullServerList;

		private UIServerModListPopup ModListPopup;
		
		public Func<UIServerDataElement, UIServerDataElement, int> DefaultComparator { get; internal set; }


		////////////////

		public UIServerBrowserList( UITheme theme ) : base() {
			this.Theme = theme;

			theme.ApplyList( this );

			////

			this.MyList = new UIList();
			this.MyList.Width.Set( -25, 1f );
			this.MyList.Height.Set( 0f, 1f );
			this.MyList.HAlign = 0f;
			this.MyList.ListPadding = 4f;
			this.MyList.SetPadding( 0f );
			this.Append( (UIElement)this.MyList );

			UIScrollbar scrollbar = new UIScrollbar();
			scrollbar.Top.Set( 8f, 0f );
			scrollbar.Height.Set( -16f, 1f );
			scrollbar.SetView( 100f, 1000f );
			scrollbar.HAlign = 1f;
			this.Append( (UIElement)scrollbar );

			this.MyList.SetScrollbar( scrollbar );

			////

			this.ModListPopup = new UIServerModListPopup( this.Theme );
			this.ModListPopup.Top.Set( 0f, 0f );
			this.ModListPopup.Left.Set( 0f, 0f );
			//this.Append( (UIElement)this.ModListPopup );

			////

			this.DefaultComparator = UIServerDataElement.CompareByWorldName;
		}


		////////////////

		public void RenderList( ICollection<UIServerDataElement> list ) {
			lock( UIServerBrowserList.MyLock ) {
				if( this.MyList.Count > 0 ) {
					this.MyList.Clear();
					this.MyList.AddRange( list );
					this.MyList.Recalculate();
				}
			}
		}


		public void RefreshServerList( Action<string, int> pre_join ) {
			lock( UIServerBrowserList.MyLock ) {
				if( this.MyList.Count > 0 ) {
					this.MyList.Clear();
					this.MyList.Recalculate();
				}
			}

			Action<string> list_ready = delegate ( string output ) {
				this.FullServerList = UIServerBrowserList.GetListFromJsonStr( this.Theme, output, this.Comparator, pre_join );
				
				if( this.FullServerList.Count > 0 ) {
					lock( UIServerBrowserList.MyLock ) {
						this.MyList.AddRange( this.FullServerList );
						this.Recalculate();
					}
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

		internal void UpdateOrder() {
			this.MyList.UpdateOrder();
		}

		private int Comparator( UIServerDataElement prev, UIServerDataElement next ) {
			return this.DefaultComparator( prev, next );
		}

		////////////////

		public void Filter( Func<UIServerDataElement, bool> filter ) {
			IList<UIServerDataElement> new_list = new List<UIServerDataElement>(
				this.FullServerList.Where( elem => { return filter( elem ); } )
			);

			this.RenderList( new_list );
		}


		////////////////

		public override void Draw( SpriteBatch sb ) {
			lock( UIServerBrowserList.MyLock ) {
				base.Draw( sb );

				bool is_hovering_server = false;
				
				foreach( UIElement item in this.MyList._items ) {
					if( !item.IsMouseHovering ) { continue; }
					is_hovering_server = true;

					var server_data_elem = (UIServerDataElement)item;

					this.ModListPopup.SetServer( server_data_elem.Data );

					CalculatedStyle dim = this.ModListPopup.GetDimensions();
					float top = Main.mouseY + 16f;
					float left = Main.mouseX - ( dim.Width * 0.5f );

					this.ModListPopup.Top.Set( top, 0f );
					this.ModListPopup.Left.Set( left, 0f );
					this.ModListPopup.Recalculate();
				}

				if( is_hovering_server ) {
					this.ModListPopup.Draw( sb );
				}
			}
		}
	}
}
