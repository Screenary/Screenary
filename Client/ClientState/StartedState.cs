using System;

namespace Screenary.Client
{
	public class StartedState : IClientState
	{
		/**
		 * Class constructor
		 */ 		
		public StartedState (MainWindow mainWindow) : base(mainWindow)
		{

		}
		
		/**
		 * Only show create or join triggers
		 */ 		
		public override void refresh()
		{

		}
	}
}

