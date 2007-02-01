/* $Id$
 * $Log$
 * Revision 1.2  2007/02/01 12:00:55  tim
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

		/// <summary>
		/// Threads check this and when it is true they exit
		/// </summary>
		public static volatile bool ShuttingDown = false;

		public frmServer()
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
			ShuttingDown = true;
			try
			{
				ServerNetwork.listeningThread.Abort();
			}
			catch
			{}

			for (int i = 0; i < ServerNetwork.netStreams.Count; i++)
			{
				try
				{
					((ServerNetwork.connection)ServerNetwork.netStreams[i]).watchingClass.watchingThread.Abort();
				}
				catch
				{}
			}

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
			((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
			this.SuspendLayout();
			// 
			// btnListen
			// 
			this.btnListen.Location = new System.Drawing.Point(72, 88);
			this.btnListen.Name = "btnListen";
			this.btnListen.Size = new System.Drawing.Size(120, 23);
			this.btnListen.TabIndex = 0;
			this.btnListen.Text = "Start Listening";
			this.btnListen.Click += new System.EventHandler(this.btnListen_Click);
			// 
			// numPort
			// 
			this.numPort.Location = new System.Drawing.Point(64, 40);
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
			// frmServer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.Add(this.numPort);
			this.Controls.Add(this.btnListen);
			this.Name = "frmServer";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
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
	}
}
