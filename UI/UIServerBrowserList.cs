using HamstarHelpers.DebugHelpers;
using HamstarHelpers.NetHelpers;
using HamstarHelpers.UIHelpers;
using HamstarHelpers.Utilities.Config;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace ServerBrowser.UI {
	class UIServerBrowserList : UIPanel {
		private static object MyLock = new object();


		////////////////

		public static IList<UIServerDataElement> GetListFromJsonStr( UITheme theme, string json_str,
				Func<UIServerDataElement, UIServerDataElement, int> comparator,
				Action<string, int> pre_join, out bool success ) {
			IList<UIServerDataElement> list;

			try {
				var data = JsonConfig<IDictionary<string, ServerBrowserEntry>>.Deserialize( json_str );
				list = new List<UIServerDataElement>( data.Count );
				
				foreach( var kv in data ) {
					list.Add( new UIServerDataElement( theme, kv.Value, comparator, pre_join ) );
				}

				success = true;
			} catch( Exception e ) {
				list = new List<UIServerDataElement>();

				int len = json_str.Length > 64 ? 64 : json_str.Length;
				LogHelpers.Log( "GetListFromJsonStr - " + e.ToString() + " - " + json_str.Substring( 0, len ) );

				success = false;
			}

			return list;
		}


		////////////////

		private UITheme Theme;
		private UIList MyList;
		private UIServerModListPopup ModListPopup;
		public Func<UIServerDataElement, UIServerDataElement, int> DefaultComparator { get; internal set; }

		private IList<UIServerDataElement> FullServerList = new List<UIServerDataElement>();


		////////////////

		public UIServerBrowserList( UITheme theme ) : base() {
			this.Theme = theme;

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

			////

			this.DefaultComparator = UIServerDataElement.CompareByWorldName;

			this.RefreshTheme_Yielding();
		}


		////////////////

		public void RenderList( ICollection<UIServerDataElement> list ) {
			if( this.MyList.Count > 0 ) {
				this.MyList.Clear();
			}
			this.MyList.AddRange( list );
			this.MyList.Recalculate();
		}


		public void RefreshServerList_Yielding( Action<string, int> pre_join, Action on_success, Action on_err ) {
			if( this.FullServerList.Count > 0 ) {
				//new Thread( () => { } ).Start();
				lock( UIServerBrowserList.MyLock ) {
					this.FullServerList.Clear();
					this.MyList.Clear();
					this.MyList.Recalculate();
				}
			}

			Action<string> list_ready = delegate ( string output ) {
				bool success;

				lock( UIServerBrowserList.MyLock ) {
					this.FullServerList = UIServerBrowserList.GetListFromJsonStr( this.Theme, output, this.Comparator, pre_join, out success );
					
					if( this.FullServerList.Count > 0 ) {
						this.MyList.AddRange( this.FullServerList );
						this.Recalculate();
					}
				}

				if( success ) {
					on_success();
				} else {
					on_err();
				}
			};

			Action<Exception, string> list_error = delegate ( Exception e, string output ) {
				LogHelpers.Log( "List could not load " + e.ToString() );
				on_err();
			};

			NetHelpers.MakeGetRequestAsync( "https://script.google.com/macros/s/AKfycbzQl2JmJzdEHguVI011Hk1KuLktYJPDzpWA_tDbyU_Pk02fILUw/exec",
				list_ready, list_error );
		}


		////////////////

		internal void UpdateOrder_Yielding() {
			lock( UIServerBrowserList.MyLock ) {
				this.MyList.UpdateOrder();
			}
		}

		private int Comparator( UIServerDataElement prev, UIServerDataElement next ) {
			return this.DefaultComparator( prev, next );
		}

		////////////////

		public void Filter_Yielding( Func<UIServerDataElement, bool> filter ) {
			lock( UIServerBrowserList.MyLock ) {
				IList<UIServerDataElement> new_list = new List<UIServerDataElement>(
					this.FullServerList.Where( elem => { return filter( elem ); } )
				);
			
				this.RenderList( new_list );
			}
		}

		public void Unfilter_Yielding() {
			lock( UIServerBrowserList.MyLock ) {
				this.RenderList( this.FullServerList );
			}
		}


		////////////////

		public void RefreshTheme_Yielding() {
			this.Theme.ApplyList( this );
			this.Theme.ApplyPanel( this.ModListPopup );

			if( this.FullServerList.Count > 0 ) {
				lock( UIServerBrowserList.MyLock ) {
					foreach( UIElement item in this.FullServerList ) {
						var server_data_elem = (UIServerDataElement)item;
						server_data_elem.RefreshTheme();
					}
				}
			}
		}


		////////////////

		public override void Draw( SpriteBatch sb ) {
			bool is_hovering_server = false;

			lock( UIServerBrowserList.MyLock ) {
				base.Draw( sb );

				foreach( UIElement item in this.MyList._items ) {
					if( !item.IsMouseHovering ) { continue; }
					is_hovering_server = true;

					var server_data_elem = (UIServerDataElement)item;

					this.ModListPopup.SetServer( server_data_elem.Data );
					break;
				}
			}

			if( is_hovering_server ) {
				this.ModListPopup.Draw( sb );
			}
		}
	}
}
