using System;

namespace Screenary.Client
{
	public partial class ExceptionDialog : Gtk.Dialog
	{
		/**
		 * The dialog takes two strings, one is the title that will be on the box, the other is the message that will be displayed
		 **/
		public ExceptionDialog (string title, string message)
		{
			Gtk.Application.Invoke(delegate {
				this.Build ();
				this.Title = title;
				
				Gtk.TextBuffer buffer;
				buffer = txtMessage.Buffer;
				buffer.Clear();
				buffer.InsertAtCursor(message);
			});
		}

		protected void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}

