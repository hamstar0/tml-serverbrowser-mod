using HamstarHelpers.Components.UI.Elements;
using HamstarHelpers.Helpers.DebugHelpers;


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


		public void CycleFilterServerListByLocked() {
			if( this.PasswordFilterMode == 0 ) {
				this.PasswordFilterMode = 1;
			} else if( this.PasswordFilterMode >= 1 ) {
				this.PasswordFilterMode = -1;
			} else {
				this.PasswordFilterMode = 0;
			}

			this.ServerList.Filter_Yielding( elem => {
				if( this.PasswordFilterMode == 0 ) {
					return true;
				} else if( this.PasswordFilterMode >= 1 ) {
					return elem.Data.IsPassworded;
				} else {
					return !elem.Data.IsPassworded;
				}
			} );
		}
	}
}
