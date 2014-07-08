using System;
using CoreUtilities;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using CoreUtilities.Links;
using System.Data;
using Layout;
using System.ComponentModel;
using System.Threading;
using MefAddIns;

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
		// this is the raw string data of the LAST RAN fact parse
		public List<string> results_to_store = new List<string>();

		const string REFRESH_BUTTON = "refreshButton";

		public string Token {
			get {
				return token;
			}
			set {
				token = value;
			}
		}

		private string factParseNote = Constants.BLANK;

		public string FactParseNote {
			get {
				return factParseNote;
			}
			set {
				factParseNote = value;
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

		/// <summary>
		/// Starts the fact gathering.
		/// 
		/// We return an error code for displaying errors.
		/// 
		/// </summary>
		public int StartFactGathering (ref List<string> results,/* string _FactParseNote, */string textFromNote)
		{
			try {



				/*if (Constants.BLANK == _FactParseNote ) {
					return 1;
				} else*/ 
					// Step 1 - Link to a note on this page with the name(s) of another note : Loop
				
					if (textFromNote == Constants.BLANK) {
						return 2;

					} else {
						//NewMessage.Show ("Found = " + note.Data1); // worked

						// to do: will need to parse .data1 and grab lines


						//The Pool;[[Group,Storyboard,Chapters*,*]]

						//string textFromNote = note.GetAsText ();
						string[] lines = textFromNote.Split (new char[1]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
						if (lines != null && lines.Length > 0) {
							// for each Layout we have to open we added to the results list
							// results are stored on the note
							//	List<string> results = new List<string>();
							foreach (string theLine in lines) {


								string noteData1 = theLine;//"The Pool;[[Group,Storyboard,Chapters*,*]]";
								string[] RawView = noteData1.Split (new char[1] {';'}, StringSplitOptions.RemoveEmptyEntries);
								if (RawView != null && RawView.Length == 2) {


									// Second part -- fine deatils

									// Strings will be formatted like this[[Group,Storyboard,Chapters*,*]]
									// logic - strip out [[ and ]]
									// break into stringarray , seperate by ','
									// string[0] = word Group. We ignore and always fill in Group
									// string[1] = Storyboard  -- this is the important bit -- the name of the Storyboard to look
									// string[2] = which "folder" to look in on the storyboard



									// Step 2 - Open that other note
									string LayoutName = RawView [0];//"LayoutToFind";
									string Code = RawView [1];
									string LayoutGUID = MasterOfLayouts.GetGuidFromName (LayoutName);
									if (LayoutGUID != Constants.BLANK && Code != Constants.BLANK) {

										LayoutPanel temporaryLayoutPanel = new LayoutPanel ("", false);
										temporaryLayoutPanel.LoadLayout (LayoutGUID, false, null, true);
										if (temporaryLayoutPanel == null)
										{
											return 100;
										}

									//NewMessage.Show (temporaryLayoutPanel.Count ().ToString());

										string[] code = new string[3];
										// we fake a string array to meet the Fact-Parse system's expectations of Storyboard/GrouporNoGroup/Details
										// even though we do not need the first two lines
										code [0] = "[[factlist]]";
										code [1] = "[[bygroup]]";

										code [2] = Code;
									//NewMessage.Show (Code);
										Hashtable hResults = Addin_YourothermindMarkup.GetFactsOnRemoteNote (temporaryLayoutPanel, code);

										if (hResults != null) {
											foreach (DictionaryEntry gg in hResults) {
										//	NewMessage.Show ("Matching " + gg.Key.ToString() + " with " + token);
											if (gg.Key.ToString () == token)
											{
												string[] factsforentry = gg.Value.ToString().Split(new string[1]{FactListMaker.SEP_PHRASES}, StringSplitOptions.RemoveEmptyEntries);
												if (factsforentry != null)
												{
													
													foreach (string s in factsforentry)
													{
														string sresult = /*gg.Value*/s + FactListMaker.SEP_INSIDEPHRASE + token + FactListMaker.SEP_INSIDEPHRASE + LayoutGUID;
														results.Add (sresult);
													}
												}
											}


//												// only add if they matc h the search criteria
//												if (gg.Key.ToString () == token)
//												{
//													string sresult = gg.Value + FactListMaker.SEP_INSIDEPHRASE + gg.Key + FactListMaker.SEP_INSIDEPHRASE + LayoutGUID;
//													results.Add (sresult);
//												}
										
											}
										}


										// now we have to run the actual Fact - Parsing
										// Step 3 - On that Other Note run the indicated fact search (from Step 1)
									
									
										// Step 4 - Return the list of facts/positions in a usable manner to here.


										// Step 5 - TODO. In case I forget all this has to happen on a BackgroundThread. Do that last, though.
									
									} else {
										return 3;
									}
								} else {
									return 4;

								}
							} // foreach line


				
							// Step 6 - Now we parse the results and add them to the form.
							//  -- nope we have this called in the Progress thing


						}//lines
					else {
							return 5;
						}
					}


			} catch (Exception ex) {
				lg.Instance.Line ("StartFactGathering", ProblemType.EXCEPTION, ex.ToString ());

			}

			return 0;
		}

		/// <summary>
		/// Gets the group box for layout.
		/// 
		/// This is where we put the appropraite label
		/// </summary>
		/// <returns>
		/// The group box for layout.
		/// </returns>
		/// <param name='layoutguid'>
		/// Layoutguid.
		/// </param>
		private GroupBox getGroupBoxForLayout (string layoutguid)
		{
			for (int i = BottomInfo.Controls.Count-1; i >= 0; i--) {
				if (BottomInfo.Controls[i].Name == layoutguid)
				{
					return (GroupBox)BottomInfo.Controls[i];
				}
			}
			return null;
		}

		ToolTip tip = null;
		/// <summary>
		/// Updates the view.
		/// 
		/// Will take the raw string data of the connections and create links to the notes.
		/// 
		/// *Note: Once BackgroundTask is setup, this will be what is called from PROGRESS ... the core number crunching will take place in the handler sans interface 
		/// </summary>
		void UpdateView ()
		{
			if (tip == null) {
				tip = new ToolTip();
				tip.ShowAlways= true;

				//BottomInfo.Controls.Add (tip);

			}
			// clear list
			for (int i = BottomInfo.Controls.Count-1; i >= 0; i--) {
				if (BottomInfo.Controls[i].Name != REFRESH_BUTTON)
				{
					BottomInfo.Controls.Remove(BottomInfo.Controls[i]);
				}

			}

			if (results_to_store != null)
			{
				int count = 0;
				foreach (string s in results_to_store)
				{
					count++; // just for naming labels
					FactRecord factRecord = FactRecord.CreateFactRecord(s, true);
					if (factRecord != null)
					{

						GroupBox box = null;

						box = getGroupBoxForLayout(factRecord.layoutguid);
						if (box == null)
						{
							box = new GroupBox();
							box.Dock = DockStyle.Left;
							box.Text = MasterOfLayouts.GetNameFromGuid(factRecord.layoutguid);
							box.Name= factRecord.layoutguid;
							box.AutoSize = true;
						}

						LinkLabel newLabel = new LinkLabel();
						newLabel.Name = "label" + count;
						newLabel.Text = factRecord.chapter;
						newLabel.AutoSize = true; // if false, it will supply a tooltip and override mine
						newLabel.AutoEllipsis = false;

						string toolTip = factRecord.text.Substring(0, Math.Min (100, factRecord.text.Length)); // also used for the search


						newLabel.Dock = DockStyle.Left;
						newLabel.Click += (object sender, EventArgs e) => 
							{
							LayoutDetails.Instance.LoadLayout(factRecord.layoutguid ,factRecord.noteguid, toolTip/*"[["+factRecord.theFact*/);
							//	NewMessage.Show (factRecord.layoutguid + " -- " + factRecord.noteguid);
							};
						box.Controls.Add (newLabel);
						BottomInfo.Controls.Add (box);

						//tip.SetToolTip(box, "Hello there boxy");

					//	NewMessage.Show (factRecord.text);
						tip.SetToolTip(newLabel, toolTip);
						//tip.SetToolTip(newLabel, "zippy"); worked
						//NewMessage.Show (factRecord.ToString ());
					}
					else
					{
						NewMessage.Show ("Fact Record was null for : " + s);
					}
				}
				
				//ok: this works. Now the hard part
			//	NewMessage.Show ("Matches are = " + results_to_store.Count);
			}
		}
		Button refreshButton = null;
		Panel BottomInfo = null;
		protected override void DoBuildChildren (LayoutPanelBase Layout)
		{
			base.DoBuildChildren (Layout);
			try {
				 BottomInfo = new Panel();
				BottomInfo.Dock = DockStyle.Bottom;
				BottomInfo.Height = 50;

				 refreshButton = new Button();
				refreshButton.Dock = DockStyle.Left;
				refreshButton.Name = REFRESH_BUTTON;
				refreshButton.Text = Loc.Instance.GetString("O");
				refreshButton.Click += (object sender, EventArgs e) => {
					refreshButton.Cursor = Cursors.WaitCursor;
					ResetBackgroundWorker();
					refreshButton.Enabled = false;
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


				ToolStripMenuItem FactParseNoteItem = 
					LayoutDetails.BuildMenuPropertyEdit (Loc.Instance.GetString("Note With Fact Details: {0}"), 
					                                     FactParseNote,
					                                     Loc.Instance.GetString ("Indicate a note on this layout that contains fact-parsing instructions."),
					                                     HandleFactChange );


				properties.DropDownItems.Add (FactParseNoteItem);

			} catch (Exception ex) {
				NewMessage.Show (ex.ToString ());
			}
			

			UpdateView ();
			
		}

		void HandleFactChange (object sender, KeyEventArgs e)
		{
			string tablecaption = FactParseNote;
			LayoutDetails.HandleMenuLabelEdit (sender, e, ref tablecaption, SetSaveRequired);
			FactParseNote = tablecaption;
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


		BackgroundWorker bw = null;

		/// <summary>
		/// Sets the tag. 
		/// 
		/// So far only being used in unit tests
		/// </summary>
		/// <param name='newTag'>
		/// New tag.
		/// </param>
		public void SetTag(string newTag)
		{
			this.token = newTag;
		}

		protected void ResetBackgroundWorker()
		{
			bw = new BackgroundWorker();
			
			// this allows our worker to report progress during work
			bw.WorkerReportsProgress = true;
			NoteDataXML_RichText note = (NoteDataXML_RichText)this.Layout.FindNoteByName (FactParseNote);
			string textFromNote = note.GetAsText ();
			// what to do in the background thread
			bw.DoWork += new DoWorkEventHandler(
				delegate(object o, DoWorkEventArgs args)
				{
				if (o == null) {
					lg.Instance.Line ("XML_FACTs.ResetBackgroundWorker", ProblemType.ERROR, "object was null");
					return;}



				BackgroundWorker b = o as BackgroundWorker;
				int error = 0;
				List<string> results = new List<string>();

				// do some simple processing for 10 seconds
				for (int i = 1; i <= 2; i++)
				{
					//Console.Beep();
					if (1 == i)
					{
						//Do the work
						try
						{
							// suspending title also prevents an unsafe thread call
							LayoutDetails.Instance.SuspendTitleUpdate(true);
							error = StartFactGathering (ref results, /*FactParseNote,*/ textFromNote);
							LayoutDetails.Instance.SuspendTitleUpdate(false);
						}
						catch (Exception ex)
						{
							lg.Instance.Line ("NoteDataXML_Facts:ResetBackgroundWorker", ProblemType.EXCEPTION, ex.ToString ());
						}
					}
					if (2 == i)
					{

						// report the progress in percent
						b.ReportProgress(error, results );
					}
					Thread.Sleep(1000);
				}
				
			});
			
			// what to do when progress changed (update the progress bar for example)
			bw.ProgressChanged += new ProgressChangedEventHandler(
				delegate(object o, ProgressChangedEventArgs args)
				{

				//NewMessage.Show ("Here with " + args.UserState.GetType().ToString ());
				results_to_store = (List<string>)args.UserState;

				if (o == null) return ;
				int error = args.ProgressPercentage;
				switch (error) {
				case 1:
					NewMessage.Show (Loc.Instance.GetString ("Provide the name of a note on this layout with fact gathering instructions."));
					break;
				case 2:
					NewMessage.Show (Loc.Instance.GetStringFmt ("The note [{0}] does not exist on this layout.", FactParseNote));
					break;
				case 3:
					NewMessage.Show (Loc.Instance.GetStringFmt ("The layout [{0}] does not have a valid GUID", "--"));
					break;
				case 4:
					NewMessage.Show (Loc.Instance.GetStringFmt ("Each line must be formated like: LayoutName;[[Group,Storyboard,Chapters*,*]]. The length was [{0}] and the source text was [{1}]. The code must be on the Source Note, not the Fact card!", /*RawView.Length*/0, /*textFromNote*/"--"));
					break;
				case 5: NewMessage.Show (Loc.Instance.GetStringFmt("The source note [{0}] does not have any text on it", FactParseNote));break;
				}

				SetSaveRequired(true);
				UpdateView();
			});
			
			// what to do when worker completes its task (notify the user)
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
				delegate(object o, RunWorkerCompletedEventArgs args)
				{

			

				//label1.Text = "Finished!";
				refreshButton.Enabled = true;
				refreshButton.Cursor = Cursors.Default;
			});
			
			//
			bw.RunWorkerAsync();
		}
		
	}
}

