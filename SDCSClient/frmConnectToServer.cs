using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Client
{
	/// <summary>
	/// Summary description for frmConnectToServer.
	/// </summary>
	public class frmConnectToServer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtIP;
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.NumericUpDown numPort;
		private System.Windows.Forms.TextBox txtUsername;
		private System.Windows.Forms.TextBox txtPassword;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of frmConnectToServer class
		/// </summary>
		public frmConnectToServer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			txtIP.Text = ClientNetwork.IPAddress;
			numPort.Value = ClientNetwork.Port;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
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
			this.txtIP = new System.Windows.Forms.TextBox();
			this.numPort = new System.Windows.Forms.NumericUpDown();
			this.btnConnect = new System.Windows.Forms.Button();
			this.txtUsername = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
			this.SuspendLayout();
			// 
			// txtIP
			// 
			this.txtIP.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtIP.Location = new System.Drawing.Point(0, 0);
			this.txtIP.Name = "txtIP";
			this.txtIP.Size = new System.Drawing.Size(464, 20);
			this.txtIP.TabIndex = 0;
			this.txtIP.Text = "Server IP";
			// 
			// numPort
			// 
			this.numPort.Dock = System.Windows.Forms.DockStyle.Top;
			this.numPort.Location = new System.Drawing.Point(0, 20);
			this.numPort.Maximum = new System.Decimal(new int[] {
																	90000,
																	0,
																	0,
																	0});
			this.numPort.Name = "numPort";
			this.numPort.Size = new System.Drawing.Size(464, 20);
			this.numPort.TabIndex = 1;
			this.numPort.Value = new System.Decimal(new int[] {
																  3000,
																  0,
																  0,
																  0});
			// 
			// btnConnect
			// 
			this.btnConnect.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnConnect.Location = new System.Drawing.Point(0, 79);
			this.btnConnect.Name = "btnConnect";
			this.btnConnect.Size = new System.Drawing.Size(464, 104);
			this.btnConnect.TabIndex = 2;
			this.btnConnect.Text = "&Connect To Server";
			this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
			// 
			// txtUsername
			// 
			this.txtUsername.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtUsername.Location = new System.Drawing.Point(0, 40);
			this.txtUsername.Name = "txtUsername";
			this.txtUsername.Size = new System.Drawing.Size(464, 20);
			this.txtUsername.TabIndex = 3;
			this.txtUsername.Text = "Username";
			// 
			// txtPassword
			// 
			this.txtPassword.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtPassword.Location = new System.Drawing.Point(0, 60);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(464, 20);
			this.txtPassword.TabIndex = 4;
			this.txtPassword.Text = "";
			// 
			// frmConnectToServer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(464, 183);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.txtUsername);
			this.Controls.Add(this.btnConnect);
			this.Controls.Add(this.numPort);
			this.Controls.Add(this.txtIP);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "frmConnectToServer";
			this.Text = "frmConnectToServer";
			((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Handles the event for when the user clicks on the connect button
		/// </summary>
		/// <param name="sender">Ignored input</param>
		/// <param name="e">Ignored input</param>
		private void btnConnect_Click(object sender, System.EventArgs e)
		{
			ClientNetwork.IPAddress = txtIP.Text;
			ClientNetwork.Port = (int)numPort.Value;
			if (!(ClientNetwork.logInToServer(txtUsername.Text, txtPassword.Text)))
				MessageBox.Show("Login Failed");
			this.Dispose();
		}
	}
}
