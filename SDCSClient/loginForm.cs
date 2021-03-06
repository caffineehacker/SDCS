using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Client
{
	/// <summary>
	/// The username and password login screen
	/// </summary>
	public class LoginForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox usernameTextBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button loginButton;
		private System.Windows.Forms.Label UsernameLabel;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.Label PasswordLabel;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.StatusBar loginFormStatusBar;
		private System.Windows.Forms.MenuItem ChooseServerMenuItem;
		private System.Windows.Forms.MenuItem PreferencesMenuItem;
        private System.Windows.Forms.MenuItem ExitMenuItem;
        private IContainer components;

		/// <summary>
		/// Standard constructor
		/// </summary>
		public LoginForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
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
            this.components = new System.ComponentModel.Container();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loginButton = new System.Windows.Forms.Button();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.ChooseServerMenuItem = new System.Windows.Forms.MenuItem();
            this.PreferencesMenuItem = new System.Windows.Forms.MenuItem();
            this.ExitMenuItem = new System.Windows.Forms.MenuItem();
            this.loginFormStatusBar = new System.Windows.Forms.StatusBar();
            this.SuspendLayout();
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(88, 16);
            this.usernameTextBox.MaxLength = 100;
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(200, 20);
            this.usernameTextBox.TabIndex = 0;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(32, 80);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(112, 40);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(168, 80);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(112, 40);
            this.loginButton.TabIndex = 2;
            this.loginButton.Text = "Login";
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.Location = new System.Drawing.Point(16, 16);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(64, 16);
            this.UsernameLabel.TabIndex = 4;
            this.UsernameLabel.Text = "Username:";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(88, 48);
            this.passwordTextBox.MaxLength = 100;
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(200, 20);
            this.passwordTextBox.TabIndex = 1;
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.Location = new System.Drawing.Point(16, 48);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(64, 16);
            this.PasswordLabel.TabIndex = 6;
            this.PasswordLabel.Text = "Password:";
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
            this.ChooseServerMenuItem,
            this.PreferencesMenuItem,
            this.ExitMenuItem});
            this.menuItem1.Text = "Options";
            // 
            // ChooseServerMenuItem
            // 
            this.ChooseServerMenuItem.Index = 0;
            this.ChooseServerMenuItem.Text = "Choose Server";
            this.ChooseServerMenuItem.Click += new System.EventHandler(this.ChooseServerMenuItem_Click);
            // 
            // PreferencesMenuItem
            // 
            this.PreferencesMenuItem.Index = 1;
            this.PreferencesMenuItem.Text = "Preferences";
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Index = 2;
            this.ExitMenuItem.Text = "Exit";
            this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // loginFormStatusBar
            // 
            this.loginFormStatusBar.Location = new System.Drawing.Point(0, 131);
            this.loginFormStatusBar.Name = "loginFormStatusBar";
            this.loginFormStatusBar.Size = new System.Drawing.Size(322, 24);
            this.loginFormStatusBar.TabIndex = 7;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.loginButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(322, 155);
            this.Controls.Add(this.loginFormStatusBar);
            this.Controls.Add(this.PasswordLabel);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.UsernameLabel);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.usernameTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "LoginForm";
            this.Text = "SDCS - Login";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Handler for when the user clicks on the login button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loginButton_Click(object sender, System.EventArgs e)
		{
            if (ClientNetwork.logInToServer(usernameTextBox.Text, passwordTextBox.Text))
                this.DialogResult = DialogResult.OK;
            else
            {
                MessageBox.Show("Login failed, please check your server settings");
            }
		}

		/// <summary>
		/// Handler for when the user clicks on the cancel button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// Handler for when the user clicks on the Exit menu item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitMenuItem_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Application.Exit();
		}

        private void ChooseServerMenuItem_Click(object sender, EventArgs e)
        {
            frmConnectToServer frm = new frmConnectToServer();
            frm.ShowDialog();
        }
	}
}
