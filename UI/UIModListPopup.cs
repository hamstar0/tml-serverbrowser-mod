using HamstarHelpers.UIHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;


namespace ServerBrowser.UI {
	class UIServerModListPopup : UIPanel {
		public UITheme Theme { get; private set; }
		public ServerBrowserEntry CurrentEntry { get; private set; }

		private int LingerTime = 0;


		////////////////
		
		public UIServerModListPopup( UITheme theme ) {
			this.Theme = theme;
			this.CurrentEntry = null;

			this.Width.Set( 128f, 0f );
			this.Height.Set( 16f * 12f, 0f );

			theme.ApplyPanel( this );
		}

		////////////////

		public void SetServer( ServerBrowserEntry entry ) {
			if( this.CurrentEntry != null && this.CurrentEntry.ServerIP == entry.ServerIP ) { return; }

			this.CurrentEntry = entry;
			this.LingerTime = 6;

			this.Width.Set( ((entry.Mods.Count / 12) * 128), 0f );
		}


		////////////////

		public override void Draw( SpriteBatch sb ) {
			if( this.CurrentEntry == null ) { return; }

			base.Draw( sb );

			float x = this.Top.Pixels;
			float y = this.Left.Pixels;
			int rows = 0;

			foreach( var kv in this.CurrentEntry.Mods ) {
				if( rows++ > 12 ) {
					rows = 0;
					x += 128f;
					y = 0;
				}

				sb.DrawString( Main.fontMouseText, kv.Key + " " + kv.Value, new Vector2( x, y ), Color.White );
			}

			if( --this.LingerTime <= 0 ) {
				this.CurrentEntry = null;
			}
		}
	}
}
