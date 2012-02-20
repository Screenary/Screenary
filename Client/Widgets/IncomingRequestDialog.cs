using System;

namespace Screenary.Client
{
	public partial class IncomingRequestDialog : Gtk.Dialog
	{
		private string username;
		private IUserAction observer;
		private Boolean permission = false;
		
		public IncomingRequestDialog (IUserAction observer, string username)
		{
			Gtk.Application.Invoke(delegate {
				this.Build ();
				this.username = username;
				this.observer = observer;
				usernamelbl.LabelProp = this.username; 
			});
		}
		
		public string GetUsername()
		{
			return username;
		}

		protected void OnButtonDenyClicked (object sender, System.EventArgs e)
		{
			observer.OnUserRequestResponse(false, this.username);
			this.Destroy();
		}

		protected void OnButtonGrantClicked (object sender, System.EventArgs e)
		{
			observer.OnUserRequestResponse(true, this.username);
			this.Destroy();
		}
		
	}
}
