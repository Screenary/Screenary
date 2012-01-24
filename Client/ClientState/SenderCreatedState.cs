using System;

namespace Screenary.Client
{
	public class SenderCreatedState : IClientState
	{
		/**
		 * Class constructor
		 */ 		
		public SenderCreatedState (MainWindow mainWindow) : base(mainWindow)
		{

		}
		
		/**
		 * Hide all triggers, show close trigger
		 */ 		
		public override void refresh()
		{
			
		}
	}
}