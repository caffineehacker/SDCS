using System;

namespace Client
{
	/// <summary>
	/// Summary description for Application.
	/// </summary>
	public class Application
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.Run(new MainList());
		}
	}
}
