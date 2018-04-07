using HamstarHelpers.UIHelpers.Elements;
using System.Collections.Generic;


namespace ServerBrowser.UI {
	partial class UIServerBrowserDialog : UIDialog {
		public void SortServerListByName() {
			this.ServerList.DefaultComparator = UIServerDataElement.CompareByWorldName;
			this.ServerList.UpdateOrder();
		}

		public void SortServerListByPing() {
			this.ServerList.DefaultComparator = UIServerDataElement.CompareByPing;
			this.ServerList.UpdateOrder();
		}

		public void SortServerListByPlayers() {
			this.ServerList.DefaultComparator = UIServerDataElement.CompareByPlayers;
			this.ServerList.UpdateOrder();
		}


		public void FilterServerListByMod( string mod_text ) {
			string mod_text_lower = mod_text.ToLower();

			this.ServerList.Filter( elem => {
				foreach( string mod_name in elem.Data.Mods.Keys ) {
					if( mod_name.ToLower().Contains( mod_text_lower ) ) {
						return true;
					}
				}
				return false;
			} );
		}

		public void FilterServerListByText( string text ) {
			this.ServerList.Filter( elem => {
				if( elem.Data.WorldName.ToLower().Contains(text) ) {
					return true;
				}
				if( elem.Data.Motd.ToLower().Contains( text ) ) {
					return true;
				}
				return false;
			} );
		}
	}
}
