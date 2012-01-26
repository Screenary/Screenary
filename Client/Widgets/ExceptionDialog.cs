using System;

namespace Screenary.Client
{
	public partial class ExceptionDialog : Gtk.Dialog
	{
		public ExceptionDialog (string title, string message)
		{
			this.Build ();
			this.Title = title;
			
			Gtk.TextBuffer buffer;
			buffer = txtMessage.Buffer;
			buffer.Clear();
			buffer.InsertAtCursor(message);
		}

		protected void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}

