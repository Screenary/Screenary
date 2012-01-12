using System;

namespace Screenary.Client
{
	public partial class AuthenticateDialog : Gtk.Dialog
	{
		
		private string sessionKey;
		
		public AuthenticateDialog(string sk)
		{
			this.Build ();
			this.sessionKey = sk;
			this.Title = "Authentication for session: " + this.sessionKey;
		}

		protected void OnButtonOkClicked(object sender, System.EventArgs e)
		{
			throw new System.NotImplementedException ();
		}

		protected void OnButtonCancelClicked(object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}

