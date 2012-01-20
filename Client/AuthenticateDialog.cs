using System;

namespace Screenary.Client
{
	public partial class AuthenticateDialog : Gtk.Dialog
	{
		
		private string sessionKey;
		private IUserAction observer;
		
		public AuthenticateDialog(IUserAction observer, string sk)
		{
			this.Build ();
			this.sessionKey = sk;
			this.observer = observer;
			this.Title = "Authentication for session: " + this.sessionKey;
		}

		protected void OnButtonOkClicked(object sender, System.EventArgs e)
		{
			//observer.OnUserJoinSession(sessionKey);
			observer.OnUserAuthenticateSession(sessionKey, txtUsername.Text, txtPassword.Text);
			this.Destroy();
		}

		protected void OnButtonCancelClicked(object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}

