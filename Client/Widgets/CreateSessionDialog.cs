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
		
		/**
		 * Send user information to OnUserCreateSession 
		 **/
		protected void OnButtonCreateClicked(object sender, System.EventArgs e)
		{
			string username;
			string password;
			
			username = txtUsername.Text;
			password = txtPassword.Text;
						
			observer.OnUserCreateSession(username, password);
			
			this.Destroy();
		}
		
		protected void OnButtonCancelClicked(object sender, System.EventArgs e)
		{
			this.Destroy();
		}		
	}
}

