using System;

namespace Screenary.Client
{
	public partial class JoinDialog : Gtk.Dialog
	{
		public JoinDialog ()
		{
			this.Build ();
		}

		protected void OnButtonJoinClicked (object sender, System.EventArgs e)
		{
			string sessionKey;
			sessionKey = txtSessionKey.Text;
			AuthenticateDialog authentication = new AuthenticateDialog(sessionKey);
			this.Destroy();
		}

		protected void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}

