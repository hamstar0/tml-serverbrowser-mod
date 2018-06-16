using HamstarHelpers.DebugHelpers;
using HamstarHelpers.DotNetHelpers;
using System;
using System.Collections.Generic;


namespace ServerBrowser {
	class ServerBrowserEntry {
		public string WorldName;
		public string ServerIP;
		public bool IsPassworded;
		public string Motd;
		public int Port;
		public long Created;
		public int AveragePing;
		public string WorldProgress;
		public string WorldEvent;
		public int PlayerCount;
		public int MaxPlayerCount;
		public int PlayerPvpCount;
		public int TeamsCount;
		public IDictionary<string, string> Mods;


		public TimeSpan GetTimeSpan() {
			long now = SystemHelpers.TimeStampInSeconds();
			long total_seconds = now - this.Created;

			long max_seconds = 60 * 60 * 24 * 30;
			long seconds = total_seconds;
			if( total_seconds > max_seconds ) { seconds = max_seconds; } 

			long minutes = seconds / 60;
			long hours = minutes / 60;

			minutes -= ( hours * 60 );
			seconds -= ( minutes * 60 ) + ( hours * 60 * 60 );

			var span = new TimeSpan( (int)hours, (int)minutes, (int)seconds );
			return span;
		}
	}
}
