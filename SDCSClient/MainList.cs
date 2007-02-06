/* $Id$
 * $Log$
 * Revision 1.10  2007/02/06 21:44:39  tim
 * Fixed a small database bug
 *
 * Revision 1.9  2007-02-06 21:33:30  tim
 * Tracked down a bug that was cripling the network communications and implemented most of the rest of the buddy list network code
 *
 * Revision 1.8  2007-02-06 16:33:09  tim
 * Added a client side message pump so i can test the network code
 *
 * Revision 1.7  2007-02-05 14:57:38  tim
 * Made the login form log in
 *
 * Revision 1.6  2007-02-04 20:33:40  tim
 * Some minor cleanups
 *
 * Revision 1.5  2007-02-04 20:13:30  scott
 * Added a nice login window
 *
 * Revision 1.4  2007-02-04 05:28:53  tim
 * Updated all of the XML comments
 *
 * Revision 1.3  2007-02-01 12:00:55  tim
 * Added CVS keywords
 *
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using SDCSCommon;

namespace Client
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainList : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.ListBox lstBuddyList;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuExit;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// ToDo:
		/// </summary>
		public MainList()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// Handler function for received network data
			ClientNetwork.DataReceived += new Client.ClientNetwork.DataReceivedDelegate(ClientNetwork_DataReceived);
			LoginForm initialLogin = new LoginForm();
            if (initialLogin.ShowDialog() != DialogResult.OK)
				MessageBox.Show("Bad login");

			this.Show();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			// Need to kill the listening thread by disconnecting or the program will never exit
			ClientNetwork.Disconnect();

			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lstBuddyList = new System.Windows.Forms.ListBox();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// lstBuddyList
			// 
			this.lstBuddyList.Items.AddRange(new object[] {
															  "This",
															  "is a",
															  "test"});
			this.lstBuddyList.Location = new System.Drawing.Point(8, 16);
			this.lstBuddyList.Name = "lstBuddyList";
			this.lstBuddyList.Size = new System.Drawing.Size(216, 654);
			this.lstBuddyList.TabIndex = 0;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFile});
			// 
			// mnuFile
			// 
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuExit});
			this.mnuFile.Text = "&File";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 0;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// MainList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(232, 741);
			this.Controls.Add(this.lstBuddyList);
			this.Menu = this.mainMenu1;
			this.Name = "MainList";
			this.Text = "People - SDCS";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainList());
		}

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		/// <summary>
		/// Message pump for the network
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientNetwork_DataReceived(object sender, DataReceivedEventArgs e)
		{
			switch (e.Header.DataType)
			{
				case Network.DataTypes.BuddyListUpdate:
					Network.BuddyListData[] bld = Network.BytesToBuddyListData(e.Data);
					foreach (Network.BuddyListData bd in bld)
						MessageBox.Show("User " + bd.username + " is now user ID " + bd.userID.ToString());
					break;
			}
		}
	}
}
