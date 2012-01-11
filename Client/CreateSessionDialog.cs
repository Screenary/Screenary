using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using FreeRDP;
using Gtk;
using System.Threading;

namespace Screenary.Client
{
	public partial class CreateSessionDialog : Gtk.Dialog
	{
		private IUserAction observer;
		
		public CreateSessionDialog(IUserAction observer)
		{
			this.Build();
			this.observer = observer;
		}

		protected void OnButtonCreateClicked(object sender, System.EventArgs e)
		{
			int port;
			string hostname;
			
			hostname = txtHostname.Text;
			port = Convert.ToInt32(this.txtPort.Text);
			
			this.Destroy();

			observer.OnUserConnect(hostname, port);
		}
		
		protected void OnButtonCancelClicked(object sender, System.EventArgs e)
		{
			this.Destroy();
		}		
	}
}

