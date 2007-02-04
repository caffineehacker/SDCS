using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace WindowsApplication1
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class LoginForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox usernameTextBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button loginButton;
		private System.Windows.Forms.Label UsernameLabel;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.Label PasswordLabel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
			this.usernameTextBox = new System.Windows.Forms.TextBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.loginButton = new System.Windows.Forms.Button();
			this.UsernameLabel = new System.Windows.Forms.Label();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.PasswordLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// usernameTextBox
			// 
			this.usernameTextBox.Location = new System.Drawing.Point(88, 16);
			this.usernameTextBox.MaxLength = 100;
			this.usernameTextBox.Name = "usernameTextBox";
			this.usernameTextBox.Size = new System.Drawing.Size(200, 20);
			this.usernameTextBox.TabIndex = 0;
			this.usernameTextBox.Text = "";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(32, 80);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(112, 40);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			// 
			// loginButton
			// 
			this.loginButton.Location = new System.Drawing.Point(168, 80);
			this.loginButton.Name = "loginButton";
			this.loginButton.Size = new System.Drawing.Size(112, 40);
			this.loginButton.TabIndex = 3;
			this.loginButton.Text = "Login";
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
			this.passwordTextBox.TabIndex = 5;
			this.passwordTextBox.Text = "";
			// 
			// PasswordLabel
			// 
			this.PasswordLabel.Location = new System.Drawing.Point(16, 48);
			this.PasswordLabel.Name = "PasswordLabel";
			this.PasswordLabel.Size = new System.Drawing.Size(64, 16);
			this.PasswordLabel.TabIndex = 6;
			this.PasswordLabel.Text = "Password:";
			// 
			// LoginForm
			// 
			this.AcceptButton = this.loginButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(320, 133);
			this.Controls.Add(this.PasswordLabel);
			this.Controls.Add(this.passwordTextBox);
			this.Controls.Add(this.UsernameLabel);
			this.Controls.Add(this.loginButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.usernameTextBox);
			this.MaximizeBox = false;
			this.Name = "LoginForm";
			this.Text = "SDCS - Login";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}
	}
}
