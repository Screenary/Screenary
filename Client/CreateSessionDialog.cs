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
			string username;
			string password;
			
			username = txtUsername.Text;
			password = txtPassword.Text;
			
			Console.WriteLine("creating session with username {0} and password {1}", username, password);
			
			observer.OnUserCreateSession(username, password);
			
			this.Destroy();
		}
		
		protected void OnButtonCancelClicked(object sender, System.EventArgs e)
		{
			this.Destroy();
		}		
	}
}

