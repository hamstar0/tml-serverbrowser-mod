using HamstarHelpers.Components.UI;
using HamstarHelpers.Components.UI.Elements;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.NetHelpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace ServerBrowser.UI {
	class UIServerDataElement : UIPanel {
		public static float WorldLabelLeft = 0f;
		public static float WorldLabelTop = 0f;
		public static float UptimeLabelLeft = 176f;
		public static float UptimeLabelTop = 0f;
		public static float PingLabelLeft = 288f;
		public static float PingLabelTop = 0f;
		public static float WorldProgressLabelLeft = 348f;
		public static float WorldProgressLabelTop = 0f;
		public static float WorldEventLabelLeft = 536f;
		public static float WorldEventLabelTop = 0f;
		 public static float IPLabelLeft = 0f;
		public static float IPLabelTop = 24f;
		public static float PlayerCountLabelLeft = 136f;
		public static float PlayerCountLabelTop = 24f;
		public static float PlayerPvpCountLabelLeft = 256f;
		public static float PlayerPvpCountLabelTop = 24f;
		public static float TeamsCountLabelLeft = 384f;
		public static float TeamsCountLabelTop = 24f;
		 public static float ModsLabelLeft = 0f;
		public static float ModsLabelTop = 44f;
		 public static float MotdLabelLeft = 0f;
		public static float MotdLabelTop = 44f;


		////////////////

		public static int CompareByWorldName( UIServerDataElement prev, UIServerDataElement next ) {
			return prev.Data.WorldName.CompareTo( next.Data.WorldName );
		}

		public static int CompareByPing( UIServerDataElement prev, UIServerDataElement next ) {
			return next.Data.AveragePing - prev.Data.AveragePing;
		}

		public static int CompareByPlayers( UIServerDataElement prev, UIServerDataElement next ) {
			int way = next.Data.PlayerCount - prev.Data.PlayerCount;

			if( way == 0 ) {
				way = next.Data.MaxPlayerCount - prev.Data.MaxPlayerCount;
			}
			if( way == 0 ) {
				way = next.Data.TeamsCount - prev.Data.TeamsCount;
			}
			if( way == 0 ) {
				way = next.Data.PlayerPvpCount - prev.Data.PlayerPvpCount;
			}

			return way;
		}

		////////////////

		public static string[] GetMotdLines( string motd, int line_width ) {
			string[] motd_chunks;

			if( motd.Length > line_width ) {
				int chunks = (int)Math.Ceiling( (float)motd.Length / (float)line_width );

				motd_chunks = new string[chunks];
				for( int i = 0; i < motd_chunks.Length; i++ ) {
					int width = ( i + 1 ) * line_width <= motd.Length ? line_width : motd.Length - ( i * line_width );
					motd_chunks[i] = motd.Substring( i * line_width, width );
				}
			} else {
				motd_chunks = new string[] { motd };
			}

			return motd_chunks;
		}



		////////////////

		private UITheme Theme;
		private UITextPanelButton JoinButton;

		public ServerBrowserEntry Data { get; private set; }

		private Action<string, int> PreJoinAction;
		private Func<UIServerDataElement, UIServerDataElement, int> Comparator;


		////////////////

		public UIServerDataElement( UITheme theme, ServerBrowserEntry data, Func<UIServerDataElement, UIServerDataElement, int> comparator,
				Action<string, int> pre_join ) {
			this.Theme = theme;
			this.Data = data;

			this.PreJoinAction = pre_join;
			this.Comparator = comparator;
			
			this.InitializeMe();

			this.RefreshTheme();
		}


		private void InitializeMe() {
			var self = this;
			string server_name = this.Data.WorldName.Length > 20 ? this.Data.WorldName.Substring( 0, 18 ) + "..." : this.Data.WorldName;
			TimeSpan uptime = this.Data.GetTimeSpan();

			this.SetPadding( 4f );
			this.Width.Set( 0f, 1f );
			this.Height.Set( 48f, 0f );
			
			var world_label = new UIText( server_name );
			world_label.Left.Set( UIServerDataElement.WorldLabelLeft, 0f );
			world_label.Top.Set( UIServerDataElement.WorldLabelTop, 0f );
			this.Append( (UIElement)world_label );

			var uptime_label = new UIText( uptime.ToString() );
			uptime_label.Left.Set( UIServerDataElement.UptimeLabelLeft, 0f );
			uptime_label.Top.Set( UIServerDataElement.UptimeLabelTop, 0f );
			this.Append( (UIElement)uptime_label );

			var ping_label = new UIText( this.Data.AveragePing+"ms" );
			ping_label.Left.Set( UIServerDataElement.PingLabelLeft, 0f );
			ping_label.Top.Set( UIServerDataElement.PingLabelTop, 0f );
			this.Append( (UIElement)ping_label );

			var world_prog_label = new UIText( this.Data.WorldProgress );
			world_prog_label.Left.Set( UIServerDataElement.WorldProgressLabelLeft, 0f );
			world_prog_label.Top.Set( UIServerDataElement.WorldProgressLabelTop, 0f );
			this.Append( (UIElement)world_prog_label );

			var world_event_label = new UIText( this.Data.WorldEvent );
			world_event_label.Left.Set( UIServerDataElement.WorldEventLabelLeft, 0f );
			world_event_label.Top.Set( UIServerDataElement.WorldEventLabelTop, 0f );
			this.Append( (UIElement)world_event_label );

			////

			var ip_label = new UIText( "IP: "+this.Data.ServerIP + ":" + this.Data.Port, 0.8f );
			ip_label.Left.Set( UIServerDataElement.IPLabelLeft, 0f );
			ip_label.Top.Set( UIServerDataElement.IPLabelTop, 0f );
			this.Append( (UIElement)ip_label );

			var player_count_label = new UIText( "Players: " + this.Data.PlayerCount+"/"+this.Data.MaxPlayerCount, 0.8f );
			player_count_label.Left.Set( UIServerDataElement.PlayerCountLabelLeft, 0f );
			player_count_label.Top.Set( UIServerDataElement.PlayerCountLabelTop, 0f );
			this.Append( (UIElement)player_count_label );

			var player_pvp_count_label = new UIText( "PVPers: " + this.Data.PlayerPvpCount+"/"+this.Data.PlayerCount, 0.8f );
			player_pvp_count_label.Left.Set( UIServerDataElement.PlayerPvpCountLabelLeft, 0f );
			player_pvp_count_label.Top.Set( UIServerDataElement.PlayerPvpCountLabelTop, 0f );
			this.Append( (UIElement)player_pvp_count_label );

			var teams_count_label = new UIText( "Teams: " + this.Data.TeamsCount, 0.8f );
			teams_count_label.Left.Set( UIServerDataElement.TeamsCountLabelLeft, 0f );
			teams_count_label.Top.Set( UIServerDataElement.TeamsCountLabelTop, 0f );
			this.Append( (UIElement)teams_count_label );

			////

			/*string[] mod_list = this.Data.Mods.Select( kv => kv.Key + " " + kv.Value )
				.OrderBy(k=>k)
				.ToArray();

			var mods = new UIText( string.Join(", ", mod_list), 0.7f );
			mods.Left.Set( UIServerDataElement.ModsLabelLeft, 0f );
			mods.Top.Set( UIServerDataElement.ModsLabelTop + offset_y, 0f );
			mods.Width.Set( 0f, 1f );
			this.Append( (UIElement)mods );*/

			////

			if( this.Data.Motd != "" ) {
				int line_height = 16;
				string[] motd_chunks = UIServerDataElement.GetMotdLines( this.Data.Motd, 96 );

				for( int i=0; i<motd_chunks.Length; i++ ) {
//LogHelpers.Log( "motd "+i+" "+motd_chunks[i]+", "+ ( i * 24f )+" - "+(line_height * motd_chunks.Length) );
					var motd_label = new UIText( motd_chunks[i], 0.8f );
					motd_label.Left.Set( UIServerDataElement.MotdLabelLeft, 0f );
					motd_label.Top.Set( UIServerDataElement.MotdLabelTop + (i * line_height ), 0f );
					this.Append( (UIElement)motd_label );
				}
				
				this.Height.Set( this.Height.Pixels + (line_height * motd_chunks.Length) + (motd_chunks.Length > 0 ? 4 : 0), 0f );
			}

			////

			this.JoinButton = new UITextPanelButton( this.Theme, "Join" );
			this.JoinButton.Top.Set( 16f, 0f );
			this.JoinButton.Left.Set( -128f, 1f );
			this.JoinButton.Width.Set( 128f, 0f );
			this.JoinButton.Height.Set( 12f, 0f );
			this.JoinButton.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
				this.PreJoinAction( this.Data.ServerIP, this.Data.Port );
				
				try {
					NetHelpers.JoinServer( this.Data.ServerIP, this.Data.Port );
				} catch( Exception e ) {
					LogHelpers.Log( e.ToString() );
				}
			};
			this.Append( (UIElement)this.JoinButton );

			////

			if( this.Data.IsPassworded && Main.itemTexture[327] != null ) {
				Texture2D tex = Main.itemTexture[327];

				var lock_icon = new UIImage( tex );
				lock_icon.Top.Set( 17f, 0f );
				lock_icon.Left.Set( -116f, 1f );
				lock_icon.Width.Set( tex.Width, 0f );
				lock_icon.Height.Set( tex.Height, 0f );
				this.Append( (UIElement)lock_icon );
			}
		}


		////////////////

		public override int CompareTo( object obj ) {
			return this.Comparator( this, (UIServerDataElement)obj );
		}


		////////////////

		public void RefreshTheme() {
			this.Theme.ApplyPanel( this );
			this.JoinButton.RefreshTheme();
		}
	}
}
