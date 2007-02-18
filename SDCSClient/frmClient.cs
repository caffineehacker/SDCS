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
	/// Main form of the client program
	/// </summary>
	public class frmClient : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnuConnect;
		private System.Windows.Forms.MenuItem mnuIM;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnuExit;
		private System.Windows.Forms.TextBox txtHistory;
		private System.Windows.Forms.TextBox txtInput;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.ComboBox cmbUser;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Standard initializer
		/// </summary>
		public frmClient()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// Add a handler for the DataReceived event raised by ClientNetwork when data is received from the server
			ClientNetwork.DataReceived += new Client.ClientNetwork.DataReceivedDelegate(DataReceivedHandler);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnuConnect = new System.Windows.Forms.MenuItem();
			this.mnuIM = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mnuExit = new System.Windows.Forms.MenuItem();
			this.txtHistory = new System.Windows.Forms.TextBox();
			this.txtInput = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.cmbUser = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuConnect,
																					  this.mnuIM,
																					  this.menuItem4,
																					  this.mnuExit});
			this.menuItem1.Text = "&File";
			// 
			// mnuConnect
			// 
			this.mnuConnect.Index = 0;
			this.mnuConnect.Text = "&Connect To Server";
			this.mnuConnect.Click += new System.EventHandler(this.mnuConnect_Click);
			// 
			// mnuIM
			// 
			this.mnuIM.Index = 1;
			this.mnuIM.Text = "IM User";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "-";
			// 
			// mnuExit
			// 
			this.mnuExit.Index = 3;
			this.mnuExit.Text = "E&xit";
			// 
			// txtHistory
			// 
			this.txtHistory.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtHistory.Location = new System.Drawing.Point(0, 0);
			this.txtHistory.Multiline = true;
			this.txtHistory.Name = "txtHistory";
			this.txtHistory.ReadOnly = true;
			this.txtHistory.Size = new System.Drawing.Size(296, 96);
			this.txtHistory.TabIndex = 0;
			this.txtHistory.Text = "";
			// 
			// txtInput
			// 
			this.txtInput.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtInput.Location = new System.Drawing.Point(0, 96);
			this.txtInput.Multiline = true;
			this.txtInput.Name = "txtInput";
			this.txtInput.Size = new System.Drawing.Size(296, 104);
			this.txtInput.TabIndex = 1;
			this.txtInput.Text = "";
			// 
			// btnSend
			// 
			this.btnSend.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnSend.Location = new System.Drawing.Point(0, 239);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(296, 48);
			this.btnSend.TabIndex = 2;
			this.btnSend.Text = "Send";
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// cmbUser
			// 
			this.cmbUser.Location = new System.Drawing.Point(8, 208);
			this.cmbUser.Name = "cmbUser";
			this.cmbUser.Size = new System.Drawing.Size(280, 21);
			this.cmbUser.TabIndex = 3;
			this.cmbUser.Text = "Select A User";
			// 
			// frmClient
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(296, 287);
			this.Controls.Add(this.cmbUser);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.txtInput);
			this.Controls.Add(this.txtHistory);
			this.Menu = this.mainMenu1;
			this.Name = "frmClient";
			this.Text = "SDCS Client";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/*[STAThread]
		static void Main() 
		{
			Application.Run(new frmClient());
		}*/

		private void mnuConnect_Click(object sender, System.EventArgs e)
		{
			frmConnectToServer frm = new frmConnectToServer();
			frm.ShowDialog();
			if (ClientNetwork.Connected)
				MessageBox.Show("Successful Login");
			else
				MessageBox.Show("Login Failed");
		}

		/// <summary>
		/// Handles the event raised by ClientNetwork when data is received <seealso cref="ClientNetwork.DataReceived"/>
		/// </summary>
		/// <param name="o">Always received as null, ignored</param>
		/// <param name="e">Contains the header and data received from the server by ClientNetwork <seealso cref="DataReceivedEventArgs"/></param>
		public void DataReceivedHandler(object o, DataReceivedEventArgs e)
		{
			switch (e.Header.DataType)
			{
				case Network.DataTypes.InstantMessage:
					string fromName = "User Unknown";
					foreach (Network.BuddyListData bud in cmbUser.Items)
						if (bud.userID == e.Header.FromID)
							fromName = bud.username;
					txtHistory.Text += fromName + ": " + System.Text.UnicodeEncoding.Unicode.GetString(e.Data) + "\r\n";
					break;
				case Network.DataTypes.WhiteBoard:
					break;
				case Network.DataTypes.BuddyListUpdate:
					Network.BuddyListData[] bldArray = Network.BytesToBuddyListData(e.Data);
					foreach(Network.BuddyListData bld in bldArray)
					{
						for (int i = cmbUser.Items.Count - 1; i >= 0; i--)
						{
							if (((Network.BuddyListData)cmbUser.Items[i]).userID == bld.userID)
								cmbUser.Items.RemoveAt(i);
						}

						if (bld.userState == Network.UserState.Online)
							cmbUser.Items.Add(bld);
					}
					break;
			}
		}

		private void btnSend_Click(object sender, System.EventArgs e)
		{
			if (cmbUser.SelectedIndex != -1)
			{
				if (ClientNetwork.SendIM(((Network.BuddyListData)cmbUser.SelectedItem).userID, txtInput.Text))
				{
					txtHistory.Text += ClientNetwork.Username + ": " + txtInput.Text + "\r\n";
					txtInput.Text = "";
				}
				else
				{
					txtHistory.Text += "Connection to server lost";
				}
			}
			else
				MessageBox.Show("Select a valid user please");
		}
	}
}
