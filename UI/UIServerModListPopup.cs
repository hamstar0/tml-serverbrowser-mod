using HamstarHelpers.UIHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ServerBrowser.UI {
	class UIServerModListPopup : UIPanel {
		public static int MaxModsPerColumn = 16;
		public static int ColumnWidth = 160;
		

		////////////////

		public UITheme Theme { get; private set; }
		public ServerBrowserEntry CurrentEntry { get; private set; }

		private int LingerTime = 0;


		////////////////
		
		public UIServerModListPopup( UITheme theme ) {
			this.Theme = theme;
			this.CurrentEntry = null;

			this.Width.Set( 176f, 0f );
			this.Height.Set( (16f * UIServerModListPopup.MaxModsPerColumn) + 16f, 0f );

			theme.ApplyPanel( this );
		}

		////////////////

		public void SetServer( ServerBrowserEntry entry ) {
			if( this.CurrentEntry != null && this.CurrentEntry.ServerIP == entry.ServerIP ) { return; }

			int mod_count = entry.Mods.Count;
			int columns = (mod_count / UIServerModListPopup.MaxModsPerColumn) + ((mod_count % UIServerModListPopup.MaxModsPerColumn) == 0 ? 0 : 1);
			float width = (float)(columns * UIServerModListPopup.ColumnWidth);

			this.CurrentEntry = entry;
			this.LingerTime = 6;

			this.Width.Set( width + 16f, 0f );
		}


		////////////////

		public void UpdatePosition() {
			CalculatedStyle dim = this.GetDimensions();
			float top = Main.mouseY + 16f;
			float left = Main.mouseX - ( dim.Width * 0.5f );

			if( ( top + this.Height.Pixels ) > Main.screenHeight ) {
				top = Main.screenHeight - dim.Height;
				left = (Main.mouseX - 16f) - dim.Width;
			}

			this.Top.Set( top, 0f );
			this.Left.Set( left, 0f );
			this.Recalculate();
		}


		public override void Draw( SpriteBatch sb ) {
			if( this.CurrentEntry == null ) { return; }

			this.UpdatePosition();

			base.Draw( sb );

			float x = this.Left.Pixels + 8f;
			float y = this.Top.Pixels + 8f;
			int rows = 0;

			foreach( var kv in this.CurrentEntry.Mods ) {
				string mod_name = (kv.Key.Length > 19 ? kv.Key.Substring(0, 17)+"..." : kv.Key) + " " + kv.Value;

				if( rows >= UIServerModListPopup.MaxModsPerColumn ) {
					rows = 0;
					x += UIServerModListPopup.ColumnWidth;
					y = this.Top.Pixels + 8f;
				}

				sb.DrawString( Main.fontMouseText, mod_name, new Vector2(x, y), Color.White, 0f, default(Vector2), 0.7f, SpriteEffects.None, 1f );

				rows++;
				y += 16f;
			}

			if( --this.LingerTime <= 0 ) {
				this.CurrentEntry = null;
			}
		}
	}
}
