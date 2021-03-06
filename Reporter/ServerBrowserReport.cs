﻿using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.DotNetHelpers;
using HamstarHelpers.Services.Promises;
using System.Collections.Generic;


namespace ServerBrowser.Reporter {
	class ServerBrowserEntry {
		public string ServerIP;
		public int Port;
		public string WorldName;

		public bool IsPassworded;

		public string Motd;
		public string WorldProgress;
		public string WorldEvent;
		//public long Created;
		public int MaxPlayerCount;
		public int PlayerCount;
		public int PlayerPvpCount;
		public int TeamsCount;

		public int AveragePing;

		public IDictionary<string, string> Mods = new Dictionary<string, string>();
		public long Version;
	}



	class ServerBrowserWorkAssignment {
		public bool Success;
		public bool ProofOfWorkNeeded;
		public string Hash;
	}


	class ServerBrowserWorkProof {
		public bool IsReply = true;

		public string ServerIP;
		public int Port;
		public string WorldName;

		public string HashBase;
	}



	/*public class ServerBrowserClientData {
		public bool IsClient = true;
		public string SteamID;
		public string ClientIP;
		public string ServerIP;
		public int Port;
		public string WorldName;
		public int Ping;
		public bool IsPassworded;
		public string HelpersVersion;
	}*/




	partial class ServerBrowserReporter {
		public static string URL => "https://script.google.com/macros/s/AKfycbwB2n0X4LWDhpb6wk_VaeWG9gliBi5qLdG7Y8oPgw/exec";
			//"https://script.google.com/macros/s/AKfycbzQl2JmJzdEHguVI011Hk1KuLktYJPDzpWA_tDbyU_Pk02fILUw/exec";

		private static long LastSendTimestamp;


		////////////////

		public static bool IsHammering() {
			return ( SystemHelpers.TimeStampInSeconds() - ServerBrowserReporter.LastSendTimestamp ) <= 10;
		}



		////////////////

		//private bool IsSendingUpdates = true;


		////////////////

		internal ServerBrowserReporter() {
			Promises.AddWorldLoadEachPromise( delegate {
				this.InitializeLoopingServerAnnounce();
			} );

			Promises.AddWorldUnloadEachPromise( this.OnWorldExit );
		}

		private void OnWorldExit() {
			this.StopLoopingServerAnnounce();
		}
	}
}
