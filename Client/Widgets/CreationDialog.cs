using System;

namespace Screenary.Client
{
	public partial class CreationDialog : Gtk.Dialog
	{
		private string sessionKey;
		
		public CreationDialog (string sessionKey)
		{
			Gtk.Application.Invoke(delegate {
				this.Build ();
				this.sessionKey = sessionKey;
				Gtk.TextBuffer buffer;
				buffer = creationMessage.Buffer;
				buffer.Clear();
				buffer.InsertAtCursor("You have succesfully created a new session!\r\n" + "The session key is:\r\n" + sessionKey);
			});
		}

		protected void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}

