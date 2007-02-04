/* $Id$
 * $Log$
 * Revision 1.6  2007/02/04 05:28:53  tim
 * Updated all of the XML comments
 *
 * Revision 1.5  2007-02-04 03:59:37  tim
 * Changed some shutdown code so that the UI and the core code are more seperated
 *
 * Revision 1.4  2007-02-01 16:19:41  tim
 * Added code for storing the user's data and adding a new user from the server program.
 *
 * Revision 1.3  2007-02-01 14:05:15  tim
 * Started adding some database code for the server to keep track of users
 *
 * Revision 1.2  2007-02-01 12:00:55  tim
 * Added CVS keywords
 *
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Server
{
	/// <summary>
	/// This is the main form for the server application
	/// </summary>
	public class frmServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnListen;
		private System.Windows.Forms.NumericUpDown numPort;
		/// <summary>
		/// Required designer variable
		/// </summary>
		private System.ComponentModel.Container components = null;
		private Server.UserDatabase userDatabase;
		private System.Windows.Forms.TextBox txtPass;
		private System.Windows.Forms.Button btnAddUser;
		private System.Windows.Forms.TextBox txtUsername;

		/// <summary>
		/// Threads check this and when it is true they exit
		/// </summary>
		public static volatile bool ShuttingDown = false;

		/// <summary>
		/// Standard constructor
		/// </summary>
		public frmServer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			// Load up the database in to a DataSet
			ServerDatabase.loadDatabase();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			ServerNetwork.shutDown();
			if (MessageBox.Show("Save the database?", "Save?", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
				ServerDatabase.saveDatabase();

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
			this.btnListen = new System.Windows.Forms.Button();
			this.numPort = new System.Windows.Forms.NumericUpDown();
			this.userDatabase = new Server.UserDatabase();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.txtPass = new System.Windows.Forms.TextBox();
			this.btnAddUser = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.userDatabase)).BeginInit();
			this.SuspendLayout();
			// 
			// btnListen
			// 
			this.btnListen.Location = new System.Drawing.Point(8, 40);
			this.btnListen.Name = "btnListen";
			this.btnListen.Size = new System.Drawing.Size(120, 23);
			this.btnListen.TabIndex = 0;
			this.btnListen.Text = "Start Listening";
			this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
			// 
			// numPort
			// 
			this.numPort.Location = new System.Drawing.Point(8, 8);
			this.numPort.Maximum = new System.Decimal(new int[] {
																	90000,
																	0,
																	0,
																	0});
			this.numPort.Name = "numPort";
			this.numPort.TabIndex = 1;
			this.numPort.Value = new System.Decimal(new int[] {
																  3000,
																  0,
																  0,
																  0});
			// 
			// userDatabase
			// 
			this.userDatabase.DataSetName = "UserDatabase";
			this.userDatabase.Locale = new System.Globalization.CultureInfo("en-US");
			// 
			// txtUsername
			// 
			this.txtUsername.Location = new System.Drawing.Point(160, 8);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(112, 20);
			this.txtUsername.TabIndex = 2;
			this.txtUsername.Text = "Username";
			// 
			// txtPass
			// 
			this.txtPass.Location = new System.Drawing.Point(160, 40);
			this.txtPass.Name = "txtPass";
			this.txtPass.Size = new System.Drawing.Size(112, 20);
			this.txtPass.TabIndex = 3;
			this.txtPass.Text = "Password";
			// 
			// btnAddUser
			// 
			this.btnAddUser.Location = new System.Drawing.Point(160, 72);
			this.btnAddUser.Name = "btnAddUser";
			this.btnAddUser.Size = new System.Drawing.Size(112, 24);
			this.btnAddUser.TabIndex = 4;
			this.btnAddUser.Text = "Add User";
			this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
			// 
			// frmServer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.btnAddUser);
			this.Controls.Add(this.txtPass);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.numPort);
			this.Controls.Add(this.btnListen);
			this.Name = "frmServer";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.userDatabase)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the server application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmServer());
		}

		private void btnListen_Click(object sender, System.EventArgs e)
		{
			ServerNetwork.startListening((int)numPort.Value);
		}

		private void btnAddUser_Click(object sender, System.EventArgs e)
		{
			if (ServerDatabase.addUser(txtUsername.Text, txtPass.Text) == false)
				MessageBox.Show("Failed to add user");
		}
	}
}
