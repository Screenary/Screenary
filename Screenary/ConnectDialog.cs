using System;

namespace Screenary
{
	public partial class ConnectDialog : Gtk.Dialog
	{
		public ConnectDialog ()
		{
			this.Build ();
		}

		protected void OnButtonConnectClicked (object sender, System.EventArgs e)
		{
			Connect newConnection = new Connect();
			newConnection.ipAddress = this.ip.Text;
			newConnection.portNumber = this.port.Text;
			this.Destroy();
		}

		protected void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
	}
}

