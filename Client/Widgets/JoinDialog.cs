using System;

namespace Screenary.Client
{
	public partial class JoinDialog : Gtk.Dialog
	{
		private IUserAction observer;
		
		public JoinDialog (IUserAction observer)
		{
			this.Build ();
			this.observer = observer;
		}
		
		/**
		 * Initial Join Session procedure. Forwards the session key information to the Authenticate Dialog
		 **/
		protected void OnButtonJoinClicked (object sender, System.EventArgs e)
		{
			string sessionKey;
			sessionKey = txtSessionKey.Text;
			AuthenticateDialog authentication = new AuthenticateDialog(observer, sessionKey);
			this.Destroy();
		}

		protected void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}
