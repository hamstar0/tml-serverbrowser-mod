using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Components.Config;
using System;


namespace ServerBrowser {
	public class ServerBrowserConfigData : ConfigurationDataBase {
		public static string ConfigFileName { get { return "Server Browser Config.json"; } }


		////////////////

		public string VersionSinceUpdate = new Version(0,0,0,0).ToString();

		public bool DebugModeInfo = false;

		public bool IsServerHiddenFromBrowser = false;
		public bool IsServerHiddenFromBrowserUnlessPortForwardedViaUPNP = true;
		public bool IsServerPromptingUsersBeforeListingOnBrowser = true;
		public int ServerBrowserCustomPort = -1;



		////////////////

		internal bool UpdateToLatestVersion( ServerBrowserMod mymod ) {
			var new_config = new ServerBrowserConfigData();
			var vers_since = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();
			
			if( vers_since >= mymod.Version ) {
				return false;
			}

			this.VersionSinceUpdate = mymod.Version.ToString();

			return true;
		}
	}
}
