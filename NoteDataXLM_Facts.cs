using System;
using CoreUtilities;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using CoreUtilities.Links;
using System.Data;
using Layout;
using System.ComponentModel;
using System.Threading;


namespace ADD_Facts
{
	public class NoteDataXML_Facts:  NoteDataXML_RichText
	{
		#region constants

		//	public const string NotUsed = "Modifier";
#endregion
		#region interface

#endregion
		
		#region properties
		private string token=Constants.BLANK;

		public string Token {
			get {
				return token;
			}
			set {
				token = value;
			}
		}

#endregion
		
		
		public NoteDataXML_Facts () : base()
		{
			CommonConstructorBehavior ();
		}
		public NoteDataXML_Facts(int height, int width):base(height, width)
		{
			CommonConstructorBehavior ();

		}
		public NoteDataXML_Facts(NoteDataInterface Note) : base(Note)
		{
			//this.Notelink = ((NoteDataXML_Checklist)Note).Notelink;
		}
		protected override void CommonConstructorBehavior ()
		{
			base.CommonConstructorBehavior ();
			Caption = Loc.Instance.GetString("Fact");
			
		}
		
		/// <summary>
		/// Registers the type.
		/// </summary>
		public override string RegisterType()
		{
			return Loc.Instance.GetString("Fact");
		}
		
		protected override string GetIcon ()
		{
			return @"%*note_edit.png";
		}
		
		protected override void DoBuildChildren (LayoutPanelBase Layout)
		{
			base.DoBuildChildren (Layout);
			try {
				Panel BottomInfo = new Panel();
				BottomInfo.Dock = DockStyle.Bottom;
				BottomInfo.Height = 25;

				Button refreshButton = new Button();
				refreshButton.Text = Loc.Instance.GetString("O");
				refreshButton.Click += (object sender, EventArgs e) => {
					NewMessage.Show ("Not done yet.");

				};

				BottomInfo.Controls.Add (refreshButton);
			
				ParentNotePanel.Controls.Add (BottomInfo);
				

				ToolStripMenuItem TokenItem = 
					LayoutDetails.BuildMenuPropertyEdit (Loc.Instance.GetString("Token: [[{0}]]"), 
					                                     Token,
					                                     Loc.Instance.GetString ("Enter the token you use to annotate facts discussing this topic in the text."),
					                                     HandleTokenChange );

				properties.DropDownItems.Add (new ToolStripSeparator());
				properties.DropDownItems.Add (TokenItem);
			} catch (Exception ex) {
				NewMessage.Show (ex.ToString ());
			}
			

			
		}

		void HandleTokenChange (object sender, KeyEventArgs e)
		{
			string tablecaption = Token;
			LayoutDetails.HandleMenuLabelEdit (sender, e, ref tablecaption, SetSaveRequired);
			Token = tablecaption;
		}

		protected override void DoChildAppearance (AppearanceClass app)
		{
			base.DoChildAppearance (app);
			

		}

			
			
			
			

		
		public override void Save ()
		{
			
			
			
			
			base.Save ();
			
			



		}

		//TODO Background worker 16/06/2014
		/*

		BackgroundWorker bw = null;
		
		protected void ResetBackgroundWorker()
		{
			bw = new BackgroundWorker();
			
			// this allows our worker to report progress during work
			bw.WorkerReportsProgress = true;
			
			// what to do in the background thread
			bw.DoWork += new DoWorkEventHandler(
				delegate(object o, DoWorkEventArgs args)
				{
				BackgroundWorker b = o as BackgroundWorker;
				
				// do some simple processing for 10 seconds
				for (int i = 1; i <= 10; i++)
				{
					// report the progress in percent
					b.ReportProgress(i * 10);
					Thread.Sleep(1000);
				}
				
			});
			
			// what to do when progress changed (update the progress bar for example)
			bw.ProgressChanged += new ProgressChangedEventHandler(
				delegate(object o, ProgressChangedEventArgs args)
				{
				label1.Text = string.Format("{0}% Completed", args.ProgressPercentage);
			});
			
			// what to do when worker completes its task (notify the user)
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
				delegate(object o, RunWorkerCompletedEventArgs args)
				{
				label1.Text = "Finished!";
			});
			
			//
			// Moved to the play button bw.RunWorkerAsync();
		}*/
		
	}
}

