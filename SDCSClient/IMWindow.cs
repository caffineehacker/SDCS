using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Client
{
	/// <summary>
	/// This is the IM form. All simple text messaging is done from this form
	/// </summary>
	public class IMForm : System.Windows.Forms.Form
	{
		private string time;
		private bool stamp = false;
		private SDCSCommon.Network.BuddyListData buddyData;
		private System.Windows.Forms.Button SendBtn;
		private System.Windows.Forms.TextBox imHistBox;
		private System.Windows.Forms.TextBox imTypeBox;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem ExitMenuItem;
		private System.Windows.Forms.MenuItem timestamp;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// constructor of IMForm
		/// </summary>
		public IMForm(SDCSCommon.Network.BuddyListData bld)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			buddyData = bld;
		}

		/// <summary>
		/// Call this function when a new IM is received from the user associted with this instance
		/// </summary>
		/// <param name="message"></param>
		public void recIM(string message)
		{
			updateTime();
			imHistBox.Text += time + buddyData.username + ": " + message + "\r\n";//add time in
			this.Activate();

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			buddyData.Tag = null;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(IMForm));
			this.SendBtn = new System.Windows.Forms.Button();
			this.imHistBox = new System.Windows.Forms.TextBox();
			this.imTypeBox = new System.Windows.Forms.TextBox();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.ExitMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.timestamp = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// SendBtn
			// 
			this.SendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.SendBtn.BackColor = System.Drawing.SystemColors.Control;
			this.SendBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.SendBtn.Image = ((System.Drawing.Image)(resources.GetObject("SendBtn.Image")));
			this.SendBtn.Location = new System.Drawing.Point(227, 256);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(80, 88);
			this.SendBtn.TabIndex = 1;
			this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
			// 
			// imHistBox
			// 
			this.imHistBox.AutoSize = false;
			this.imHistBox.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.imHistBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.imHistBox.Dock = System.Windows.Forms.DockStyle.Top;
			this.imHistBox.Location = new System.Drawing.Point(0, 0);
			this.imHistBox.Multiline = true;
			this.imHistBox.Name = "imHistBox";
			this.imHistBox.ReadOnly = true;
			this.imHistBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.imHistBox.Size = new System.Drawing.Size(306, 248);
			this.imHistBox.TabIndex = 99;
			this.imHistBox.TabStop = false;
			this.imHistBox.Text = "";
			// 
			// imTypeBox
			// 
			this.imTypeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.imTypeBox.AutoSize = false;
			this.imTypeBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.imTypeBox.Location = new System.Drawing.Point(0, 256);
			this.imTypeBox.Name = "imTypeBox";
			this.imTypeBox.Size = new System.Drawing.Size(227, 88);
			this.imTypeBox.TabIndex = 0;
			this.imTypeBox.Text = "";
			this.imTypeBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.imTypeBox_KeyPress);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem2,
																					  this.menuItem3,
																					  this.menuItem4});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.ExitMenuItem});
			this.menuItem1.Text = "File";
			// 
			// ExitMenuItem
			// 
			this.ExitMenuItem.Index = 0;
			this.ExitMenuItem.Text = "Exit";
			this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "Edit";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.timestamp});
			this.menuItem3.Text = "View";
			// 
			// timestamp
			// 
			this.timestamp.Index = 0;
			this.timestamp.RadioCheck = true;
			this.timestamp.Text = "Timestamp OFF";
			this.timestamp.Click += new System.EventHandler(this.timestamp_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.Text = "Help";
			// 
			// IMForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(306, 324);
			this.Controls.Add(this.imTypeBox);
			this.Controls.Add(this.imHistBox);
			this.Controls.Add(this.SendBtn);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Menu = this.mainMenu1;
			this.Name = "IMForm";
			this.Text = "WhoIAmTalkingTo";
			this.Load += new System.EventHandler(this.IMForm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// When clicked, close the IM window. Ignore inputs.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitMenuItem_Click(object sender, System.EventArgs e)
		{
			this.Dispose();
		}

		/// <summary>
		/// IM Window's send button click - sends a text message
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SendBtn_Click(object sender, System.EventArgs e)
		{
			updateTime();
			ClientNetwork.SendIM(buddyData.userID, imTypeBox.Text);
			imHistBox.Text += time + ClientNetwork.Username + ": " + imTypeBox.Text + "\r\n";

			imTypeBox.Text = "";

		}

		/// <summary>
		/// Occurs when IM window is shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IMForm_Load(object sender, System.EventArgs e)
		{
			this.Text = buddyData.username;
		}

		private void timestamp_Click(object sender, System.EventArgs e)
		{
			stamp = !stamp;

			if(stamp)
                timestamp.Text = "Timestamp ON";
			else
				timestamp.Text = "Timestamp OFF";
		}

		/// <summary>
		/// Handler for the KeyPress event emitted by imTypeBox. We handle this in order to detect return key presses.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void imTypeBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			// Return is key code 13
			if(e.KeyChar == 13)
				SendBtn_Click(sender, (System.EventArgs)e);
		}

		/// <summary>
		/// Updates the time variable
		/// </summary>
		private void updateTime()
		{	
			if(stamp)
			{
				string hour = System.DateTime.Now.Hour.ToString();
				string minute = System.DateTime.Now.Minute.ToString();
				string second = System.DateTime.Now.Second.ToString();
				time = "(" + hour + ":" + minute + ":" + second + ") ";
			}
			else
			{
				time = "";
			}
		}
	}
}
