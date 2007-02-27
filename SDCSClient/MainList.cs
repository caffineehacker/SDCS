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
		private System.Windows.Forms.MenuItem menuItem1;
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
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// lstBuddyList
			// 
			this.lstBuddyList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstBuddyList.Location = new System.Drawing.Point(0, 0);
			this.lstBuddyList.Name = "lstBuddyList";
			this.lstBuddyList.Size = new System.Drawing.Size(232, 732);
			this.lstBuddyList.TabIndex = 0;
			this.lstBuddyList.DoubleClick += new System.EventHandler(this.lstBuddyList_DoubleClick);
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
																					this.menuItem1,
																					this.mnuExit});
			this.mnuFile.Text = "&File";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 1;
			this.mnuExit.Text = "E&xit";
			this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Logoff";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// MainList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(232, 741);
			this.Controls.Add(this.lstBuddyList);
			this.Menu = this.mainMenu1;
			this.Name = "MainList";
			this.Text = "People - SDCS";
			this.Load += new System.EventHandler(this.MainList_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void mnuExit_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Application.Exit();
		}

		/// <summary>
		/// This is to make sure all of this code executes on the main thread
		/// </summary>
		private delegate void ClientNetwork_DataReceivedDelegate(object sender, DataReceivedEventArgs e);
		/// <summary>
		/// Message pump for the network
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ClientNetwork_DataReceived(object sender, DataReceivedEventArgs e)
		{
			if (InvokeRequired)
				this.Invoke(new ClientNetwork_DataReceivedDelegate(ClientNetwork_DataReceived), new object[] {sender, e});
			else
			{
				switch (e.Header.DataType)
				{
					case Network.DataTypes.BuddyListUpdate:
						Network.BuddyListData[] bld = Network.BytesToBuddyListData(e.Data);
						foreach (Network.BuddyListData bd in bld)
						{
							for ( int i = 0 ; i < lstBuddyList.Items.Count; i++ )
							{
								if (((Network.BuddyListData)lstBuddyList.Items[i]).userID == bd.userID)
								{
									lstBuddyList.Items.RemoveAt(i);
									i = lstBuddyList.Items.Count;
								}
							}
							if (bd.userState == SDCSCommon.Network.UserState.Online)
								lstBuddyList.Items.Add(bd);
						}
						break;
					case Network.DataTypes.InstantMessage:
						foreach (Network.BuddyListData buddyDat in lstBuddyList.Items)
							if (buddyDat.userID == e.Header.FromID)
							{
								createIMWindow(buddyDat, false);
								((IMForm)buddyDat.Tag).recIM(System.Text.UnicodeEncoding.Unicode.GetString(e.Data));
							}

						break;
				}
			}
		}

		private void lstBuddyList_DoubleClick(object sender, System.EventArgs e)
		{
			if ( lstBuddyList.SelectedItem != null )
			{
				Network.BuddyListData buddyData = (Network.BuddyListData)lstBuddyList.SelectedItem ;
				createIMWindow(buddyData, true);
			}
		}

		/// <summary>
		/// Create an IM Window or activate one if it already exists
		/// </summary>
		/// <param name="bld">The buddy list data to make the window for</param>
		/// <param name="activate">If true the window will be activated, else it will only be activated if it is new</param>
		private void createIMWindow(Network.BuddyListData bld, bool activate)
		{
			if ( bld.Tag == null )
			{
				IMForm IMWindow = new IMForm(bld);
				bld.Tag = IMWindow;
				IMWindow.Show();
			}
			
			if (activate)
				((IMForm)bld.Tag).Activate();
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
		
		}

		private void MainList_Load(object sender, System.EventArgs e)
		{
			ClientNetwork.DataReceived += new Client.ClientNetwork.DataReceivedDelegate(ClientNetwork_DataReceived);
			LoginForm initialLogin = new LoginForm();
			if (initialLogin.ShowDialog() != DialogResult.OK)
				this.Dispose();
			else
				this.Show();
		}
	}
}
