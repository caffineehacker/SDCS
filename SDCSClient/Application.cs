using System;

namespace Client
{
	/// <summary>
	/// This is the application class for the Client program.
	/// </summary>
	public class Application
	{
		/// <summary>
		/// The main entry point for the client application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.Run(new MainList());
		}
	}
}
