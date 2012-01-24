using System;

namespace Screenary.Client
{
	public abstract class IClientState
	{
		/* Holds main window that alters in behavior based on state */
		protected MainWindow mainWindow;
		
		/**
		 * Class constructor, set the mainWindow
		 */ 
		public IClientState (MainWindow mainWindow)
		{
			this.mainWindow = mainWindow;
		}
		
		/**
		 * Refresh UI layout
		 */ 
		public abstract void refresh();
	}
}

