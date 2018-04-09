using HamstarHelpers.DebugHelpers;
using HamstarHelpers.UIHelpers.Elements;


namespace ServerBrowser.UI {
	partial class UIServerBrowserDialog : UIDialog {
		public void SortServerListByName() {
			this.ServerList.SetComparator( UIServerDataElement.CompareByWorldName );
			this.ServerList.UpdateOrder_Yielding();
		}

		public void SortServerListByPing() {
			this.ServerList.SetComparator( UIServerDataElement.CompareByPing );
			this.ServerList.UpdateOrder_Yielding();
		}

		public void SortServerListByPlayers() {
			this.ServerList.SetComparator( UIServerDataElement.CompareByPlayers );
			this.ServerList.UpdateOrder_Yielding();
		}


		public void FilterServerListByMod( string mod_text ) {
			if( mod_text == "" ) {
				this.ServerList.Unfilter_Yielding();
				return;
			}

			string mod_text_lower = mod_text.ToLower();

			this.ServerList.Filter_Yielding( elem => {
				foreach( string mod_name in elem.Data.Mods.Keys ) {
					if( mod_name.ToLower().Contains( mod_text_lower ) ) {
						return true;
					}
				}
				return false;
			} );
		}

		public void FilterServerListByText( string text ) {
			if( text == "" ) {
				this.ServerList.Unfilter_Yielding();
				return;
			}

			string text_lower = text.ToLower();

			this.ServerList.Filter_Yielding( elem => {
				if( elem.Data.WorldName.ToLower().Contains( text_lower ) ) {
					return true;
				}
				if( elem.Data.Motd.ToLower().Contains( text_lower ) ) {
					return true;
				}
				return false;
			} );
		}
	}
}
