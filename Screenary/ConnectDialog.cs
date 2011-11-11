using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using FreeRDP;
using Gtk;
using System.Threading;

namespace Screenary
{
	public partial class ConnectDialog : Gtk.Dialog
	{
		private SurfaceReceiver receiver;
		private Gdk.Window window;
		private string myip;
		private int myport;
		private IConnectObserver observer;
		
		public ConnectDialog (Gdk.Window window, SurfaceReceiver receiver)
		{
			this.Build ();
			this.receiver = receiver;
			this.window = window;
		}
		
		public void setObserver(IConnectObserver observer)
		{
			this.observer = observer;	
		}
		
		public String getIp()
		{
			return ip.Text;
		}
		
		public int getPort()
		{
			return Convert.ToInt32(port.Text);
		}

		protected void OnButtonConnectClicked (object sender, System.EventArgs e)
		{
			Connect newConnection = new Connect();
			newConnection.ipAddress = this.ip.Text;
			newConnection.portNumber = this.port.Text;
			
			myip = this.ip.Text;
			myport = Convert.ToInt32(this.port.Text);
			
			this.Destroy();

			// This is extremely temporary... like holy shit
			if (observer != null)
			{
				observer.NewConnection(myip, myport);
			}
		}
		
		protected void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}		
	}
}

