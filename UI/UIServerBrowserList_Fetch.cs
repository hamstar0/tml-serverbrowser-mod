using HamstarHelpers.Components.Config;
using HamstarHelpers.Components.UI;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.NetHelpers;
using System;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;


namespace ServerBrowser.UI {
	partial class UIServerBrowserList : UIPanel {
		public static IList<UIServerDataElement> GetListFromJsonStr( UITheme theme, string json_str,
				Func<UIServerDataElement, UIServerDataElement, int> comparator,
				Action<string, int> pre_join, out bool success ) {
			IList<UIServerDataElement> list;

			try {
				var data = JsonConfig<IDictionary<string, ServerBrowserEntry>>.Deserialize( json_str );
				list = new List<UIServerDataElement>( data.Count );
				
				foreach( var kv in data ) {
					list.Add( new UIServerDataElement( theme, kv.Value, comparator, pre_join ) );
				}

				success = true;
			} catch( Exception e ) {
				list = new List<UIServerDataElement>();

				int len = json_str.Length > 64 ? 64 : json_str.Length;
				LogHelpers.Log( "GetListFromJsonStr - " + e.ToString() + " - " + json_str.Substring( 0, len ) );

				success = false;
			}

			return list;
		}



		public void RefreshServerList_Yielding( Action<string, int> pre_join, Action on_success, Action on_err ) {
			if( this.FullServerList.Count > 0 ) {
				//new Thread( () => { } ).Start();
				lock( UIServerBrowserList.MyLock ) {
					this.FullServerList.Clear();
					this.MyList.Clear();
					this.MyList.Recalculate();
				}
			}

			Action<string> list_ready = delegate ( string output ) {
				bool success;

				lock( UIServerBrowserList.MyLock ) {
					this.FullServerList = UIServerBrowserList.GetListFromJsonStr( this.Theme, output, this.Comparator, pre_join, out success );
					
					if( this.FullServerList.Count > 0 ) {
						this.MyList.AddRange( this.FullServerList );
						this.Recalculate();
					}
				}

				if( success ) {
					on_success();
				} else {
					on_err();
				}
			};

			Action<Exception, string> list_error = delegate ( Exception e, string output ) {
				LogHelpers.Log( "List could not load " + e.ToString() );
				on_err();
			};

			NetHelpers.MakeGetRequestAsync(
				"https://script.google.com/macros/s/AKfycbzQl2JmJzdEHguVI011Hk1KuLktYJPDzpWA_tDbyU_Pk02fILUw/exec",
				list_ready, list_error );
		}
	}
}
