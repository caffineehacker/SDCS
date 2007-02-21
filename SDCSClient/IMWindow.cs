using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Client
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class IMForm : System.Windows.Forms.Form
	{
		private int userID;
		private string userName;
		private System.Windows.Forms.Button SendBtn;
		private System.Windows.Forms.TextBox imHistBox;
		private System.Windows.Forms.TextBox imTypeBox;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem ExitMenuItem;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// constructor of IMForm
		/// </summary>
		public IMForm(int userid, string username)
		{
			userID = userid;
			userName = username;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public void recIM(string message)
		{
			imHistBox.Text += (userID + ": " + message);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(IMForm));
			this.SendBtn = new System.Windows.Forms.Button();
			this.imHistBox = new System.Windows.Forms.TextBox();
			this.imTypeBox = new System.Windows.Forms.TextBox();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.ExitMenuItem = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// SendBtn
			// 
			this.SendBtn.BackColor = System.Drawing.SystemColors.Control;
			this.SendBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.SendBtn.Image = ((System.Drawing.Image)(resources.GetObject("SendBtn.Image")));
			this.SendBtn.Location = new System.Drawing.Point(272, 296);
			this.SendBtn.Name = "SendBtn";
			this.SendBtn.Size = new System.Drawing.Size(96, 80);
			this.SendBtn.TabIndex = 0;
			this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
			// 
			// imHistBox
			// 
			this.imHistBox.AutoSize = false;
			this.imHistBox.BackColor = System.Drawing.SystemColors.InactiveBorder;
			this.imHistBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.imHistBox.Location = new System.Drawing.Point(0, 0);
			this.imHistBox.Name = "imHistBox";
			this.imHistBox.Size = new System.Drawing.Size(368, 296);
			this.imHistBox.TabIndex = 1;
			this.imHistBox.Text = "";
			// 
			// imTypeBox
			// 
			this.imTypeBox.AutoSize = false;
			this.imTypeBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.imTypeBox.Location = new System.Drawing.Point(0, 296);
			this.imTypeBox.Name = "imTypeBox";
			this.imTypeBox.Size = new System.Drawing.Size(272, 80);
			this.imTypeBox.TabIndex = 2;
			this.imTypeBox.Text = "";
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
			this.menuItem3.Text = "View";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.Text = "Help";
			// 
			// IMForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(368, 374);
			this.Controls.Add(this.imTypeBox);
			this.Controls.Add(this.imHistBox);
			this.Controls.Add(this.SendBtn);
			this.Font = new System.Drawing.Font("Elephant", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
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
			ClientNetwork.SendIM(userID, imTypeBox.Text);
			imHistBox.Text += (ClientNetwork.Username + ": " + imTypeBox.Text);

			imTypeBox.Text = "";

		}

		/// <summary>
		/// Occurs when IM window is shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void IMForm_Load(object sender, System.EventArgs e)
		{
			this.Text = userName;
		}

	}
}
