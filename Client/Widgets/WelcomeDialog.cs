using Gtk;
using System;
using System.IO;
using System.Threading;
using Screenary.Client;
using Screenary;
using FreeRDP;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Screenary.Client
{
	public partial class WelcomeDialog : Gtk.Dialog
	{
		public WelcomeDialog ()
		{
			this.Build ();
		}

		protected void OnButtonSenderClicked (object sender, System.EventArgs e)
		{
			MainWindow newSender = new MainWindow(0);
			this.Destroy();
		}

		protected void OnButtonReceiverClicked (object sender, System.EventArgs e)
		{
			MainWindow newReceiver = new MainWindow(1);
			this.Destroy();
		}
	}
}

