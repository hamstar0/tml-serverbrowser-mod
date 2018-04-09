using HamstarHelpers.DebugHelpers;
using HamstarHelpers.UIHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace ServerBrowser.UI {
	partial class UIServerBrowserList : UIPanel {
		private static object MyLock = new object();


		////////////////

		private UITheme Theme;
		private UIList MyList;
		private UIServerModListPopup ModListPopup;
		private Func<UIServerDataElement, UIServerDataElement, int> DefaultComparator;

		public bool FlipComparator { get; private set; }

		private IList<UIServerDataElement> FullServerList = new List<UIServerDataElement>();


		////////////////

		public UIServerBrowserList( UITheme theme ) : base() {
			this.Theme = theme;
			this.FlipComparator = false;

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

		public void SetList( ICollection<UIServerDataElement> list ) {
			if( this.MyList.Count > 0 ) {
				this.MyList.Clear();
			}
			this.MyList.AddRange( list );
			this.MyList.Recalculate();
		}


		public void SetComparator( Func<UIServerDataElement, UIServerDataElement, int> func ) {
			if( this.DefaultComparator == func ) {
				this.FlipComparator = !this.FlipComparator;
			} else {
				this.FlipComparator = false;
			}

			this.DefaultComparator = func;
		}


		////////////////

		internal void UpdateOrder_Yielding() {
			lock( UIServerBrowserList.MyLock ) {
				this.MyList.UpdateOrder();
			}
		}

		private int Comparator( UIServerDataElement prev, UIServerDataElement next ) {
			if( this.FlipComparator ) {
				return this.DefaultComparator( next, prev );
			} else {
				return this.DefaultComparator( prev, next );
			}
		}

		////////////////

		public void Filter_Yielding( Func<UIServerDataElement, bool> filter ) {
			lock( UIServerBrowserList.MyLock ) {
				IList<UIServerDataElement> new_list = new List<UIServerDataElement>(
					this.FullServerList.Where( elem => { return filter( elem ); } )
				);
			
				this.SetList( new_list );
			}
		}

		public void Unfilter_Yielding() {
			lock( UIServerBrowserList.MyLock ) {
				this.SetList( this.FullServerList );
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

		public override void Update( GameTime gameTime ) {
			lock( UIServerBrowserList.MyLock ) {
				base.Update( gameTime );
			}
		}


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
