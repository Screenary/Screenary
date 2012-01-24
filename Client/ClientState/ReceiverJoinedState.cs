using System;

namespace Screenary.Client
{
	public class ReceiverJoinedState : IClientState
	{
		/**
		 * Class constructor
		 */ 		
		public ReceiverJoinedState (MainWindow mainWindow) : base(mainWindow)
		{

		}
		
		/**
		 * Hide create and join triggers, show authenticate trigger
		 */ 		
		public override void refresh()
		{
			
		}
	}
}