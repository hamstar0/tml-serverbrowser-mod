using HamstarHelpers.DebugHelpers;
using HamstarHelpers.NetHelpers;
using HamstarHelpers.UIHelpers;
using HamstarHelpers.UIHelpers.Elements;
using HamstarHelpers.Utilities.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;


namespace ServerBrowser {
	class UIServerDataElement : UIPanel {
		 public static float WorldLabelLeft = 0f;
		public static float WorldLabelTop = 0f;
		public static float UptimeLabelLeft = 160f;
		public static float UptimeLabelTop = 0f;
		public static float PingLabelLeft = 288f;
		public static float PingLabelTop = 0f;
		public static float WorldProgressLabelLeft = 348f;
		public static float WorldProgressLabelTop = 0f;
		public static float WorldEventLabelLeft = 536f;
		public static float WorldEventLabelTop = 0f;
		 public static float MotdLabelLeft = 0f;
		public static float MotdLabelTop = 24f;
		 public static float IPLabelLeft = 0f;
		public static float IPLabelTop = 24f;
		public static float PlayerCountLabelLeft = 128f;
		public static float PlayerCountLabelTop = 24f;
		public static float PlayerPvpCountLabelLeft = 256f;
		public static float PlayerPvpCountLabelTop = 24f;
		public static float TeamsCountLabelLeft = 384f;
		public static float TeamsCountLabelTop = 24f;
		 public static float ModsLabelLeft = 0f;
		public static float ModsLabelTop = 44f;


		////////////////

		public static UIServerDataElement[] GetListFromJsonStr( UITheme theme, string json_str, Action<string, int> on_join ) {
			try {
				var data = JsonConfig<IDictionary<string, ServerBrowserEntry>>.Deserialize( json_str );
				UIServerDataElement[] list = new UIServerDataElement[data.Count];

				int i = 0;
				foreach( var kv in data ) {
					list[i++] = new UIServerDataElement( theme, kv.Value, on_join );
				}

				return list;
			} catch( Exception e ) {
				int len = json_str.Length > 64 ? 64 : json_str.Length;
				LogHelpers.Log( "GetListFromJsonStr - " + e.ToString() + " - " + json_str.Substring(0, len) );
			}

			return new UIServerDataElement[0];
		}


		////////////////

		private UITheme Theme;
		public ServerBrowserEntry Data { get; private set; }
		private Action<string, int> OnJoin;


		////////////////

		public UIServerDataElement( UITheme theme, ServerBrowserEntry data, Action<string, int> on_join ) {
			this.Theme = theme;
			this.Data = data;
			this.OnJoin = on_join;
			
			this.InitializeMe();
		}


		private void InitializeMe() {
			var self = this;
			string server_name = this.Data.WorldName.Substring( 0, this.Data.WorldName.Length > 32 ? 32 : this.Data.WorldName.Length );
			float offset_y = 0f;
			TimeSpan uptime = this.Data.GetTimeSpan();

			this.SetPadding( 4f );
			this.Width.Set( 0f, 1f );
			this.Height.Set( 64, 0f );
			
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

			if( this.Data.Motd != "" ) {
				var motd_label = new UIText( this.Data.Motd, 0.8f );
				motd_label.Left.Set( UIServerDataElement.MotdLabelLeft, 0f );
				motd_label.Top.Set( UIServerDataElement.MotdLabelTop, 0f );
				this.Append( (UIElement)motd_label );

				offset_y = 24f;
			}

			////

			var ip_label = new UIText( this.Data.ServerIP + ":" + this.Data.Port, 0.8f );
			ip_label.Left.Set( UIServerDataElement.IPLabelLeft, 0f );
			ip_label.Top.Set( UIServerDataElement.IPLabelTop + offset_y, 0f );
			this.Append( (UIElement)ip_label );

			var player_count_label = new UIText( "Players: " + this.Data.PlayerCount+"/"+this.Data.MaxPlayerCount, 0.8f );
			player_count_label.Left.Set( UIServerDataElement.PlayerCountLabelLeft, 0f );
			player_count_label.Top.Set( UIServerDataElement.PlayerCountLabelTop + offset_y, 0f );
			this.Append( (UIElement)player_count_label );

			var player_pvp_count_label = new UIText( "PVPers: " + this.Data.PlayerPvpCount+"/"+this.Data.PlayerCount, 0.8f );
			player_pvp_count_label.Left.Set( UIServerDataElement.PlayerPvpCountLabelLeft, 0f );
			player_pvp_count_label.Top.Set( UIServerDataElement.PlayerPvpCountLabelTop + offset_y, 0f );
			this.Append( (UIElement)player_pvp_count_label );

			var teams_count_label = new UIText( "Teams: " + this.Data.TeamsCount, 0.8f );
			teams_count_label.Left.Set( UIServerDataElement.TeamsCountLabelLeft, 0f );
			teams_count_label.Top.Set( UIServerDataElement.TeamsCountLabelTop + offset_y, 0f );
			this.Append( (UIElement)teams_count_label );

			////

			string[] mod_list = this.Data.Mods.Select( kv => kv.Key + " " + kv.Value )
				.OrderBy(k=>k)
				.ToArray();

			var mods = new UIText( string.Join(", ", mod_list), 0.7f );
			mods.Left.Set( UIServerDataElement.ModsLabelLeft, 0f );
			mods.Top.Set( UIServerDataElement.ModsLabelTop + offset_y, 0f );
			mods.Width.Set( 0f, 1f );
			this.Append( (UIElement)mods );

			////

			var join_button = new UITextPanelButton( this.Theme, "Join" );
			join_button.Top.Set( 16f, 0f );
			join_button.Left.Set( -128f, 1f );
			join_button.Width.Set( 128f, 0f );
			join_button.Height.Set( 12f, 0f );
			join_button.OnClick += delegate ( UIMouseEvent evt, UIElement listening_element ) {
				this.OnJoin( this.Data.ServerIP, this.Data.Port );
				NetHelpers.JoinServer( this.Data.ServerIP, this.Data.Port );
			};
			this.Append( (UIElement)join_button );
		}


		////////////////

		public override int CompareTo( object obj ) {
			var data_obj = (UIServerDataElement)obj;

			return this.Data.WorldName.CompareTo( data_obj.Data.WorldName );
		}
	}
}
