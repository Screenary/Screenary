using System;

namespace Screenary.Client
{
	public class ReceiverAuthenticatedState : IClientState
	{
		/**
		 * Class constructor
		 */ 
		public ReceiverAuthenticatedState (MainWindow mainWindow) : base(mainWindow)
		{

		}
		
		/**
		 * Hide create, join and authenticate triggers
		 */ 
		public override void refresh()
		{
			
		}
	}
}