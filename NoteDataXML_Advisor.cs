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
using LayoutPanels;
using System.Linq;
using SimpleWordMatch;




namespace ADD_Facts
{

	static class MyExtensions
	{
		public static void Shuffle<T>(this IList<T> list)  
		{  
			Random rng = new Random();  
			int n = list.Count;  
			while (n > 1) {  
				n--;  
				int k = rng.Next(n + 1);  
				T value = list[k];  
				list[k] = list[n];  
				list[n] = value;  
			}  
		}
	}



	public class NoteDataXML_Advisor:  NoteDataXML
	{
	
		// this is the INDEX (name column) into the system table that contains the queries
		// it will then lookup the appropriate query as required
		private string currentFilterName = Constants.BLANK;
		
		public string CurrentFilterName {
			get {
				return currentFilterName;
			}
			set {
				currentFilterName = value;
			}
		}

		private const int THROTTLE = 2500;


		public NoteDataXML_Advisor () : base()
		{
			CommonConstructorBehavior ();
		}
		public NoteDataXML_Advisor(int height, int width):base(height, width)
		{
			CommonConstructorBehavior ();
			
		}
		protected override void CommonConstructorBehavior ()
		{
			base.CommonConstructorBehavior ();
			Caption = Loc.Instance.GetString("Advisor");
			//ResetBackgroundWorker();
			
		}
		protected override string GetIcon ()
		{
			return @"%*note_edit.png";
		}
		/// <summary>
		/// Registers the type.
		/// </summary>
		public override string RegisterType()
		{
			return Loc.Instance.GetString("Advisor");
		}
		private lookupControl lookup;
		private void Pause ()
		{
			// we need to set this here otherwise the interface is confusing if you leave a layout and come back (i.e., woudl be paused bout would not show that)

			// NOTE: This takes a few seconds before kicking in. Maybe?

			IsPlaying=false;
		//	NewMessage.Show ("Setting to false");
			WorkerRunning = false;
			if (bw != null) {
				//bw.CancelAsync();
				bw.Dispose ();
				bw = null;
			}
			playButton.Text = ">";
		}
		Button playButton;
		private Label label1;
		RichTextBox richText;
		ComboBox  CurrentFilterDropDown  = null;
		ComboBox CurrentModeDropDown = null;
		private bool IsPlaying = false;
		protected override void DoBuildChildren (LayoutPanelBase Layout)
		{
			base.DoBuildChildren (Layout);

			#region hideparent
//			if (this.SearchDetails != null) {
//				// hide some of the base controls
//				this.SearchDetails.Visible = false;
//			}
//			TokenItem.Visible = false;
//			count.Visible = false;
//			refresh.Visible = false;
//			blurb.Visible = false;
//			mode.Visible = false;
//			this.list.Dock = DockStyle.Top;
			#endregion



			/*LOGIC - keep interface simple as possible
			 * Shwo name of Current Layout/Card -- this is CARD MATCH MODE
			 * If a word is elected in Current Card show that TOO -- this is WORD MATCH MODE
			 *
			 * Both modes can be active at same time.
			 * Pause effects both modes (i.e, when paused the LOOKUP WORD stops)
			 * CONSIDER: Current word our cursor is under is always checked (not just selected); this might be too slow
			 *
			 *CARD MATCH MODE
			 * - like my Idea River
			 * 
			 * WORD MATCH MODE
			 * - shows the LOOKUP word panel + with Englex Additions, for the current word.


I could create associations between NOTE TYPES and the data pulled.

On a character card we might see the advice to HAVE DAMAGED CHARACTERS pulled from 
Writing Prompts.

Something like:
    if (characternote) then list = character prompts prioarity 10, 
notecardtype=characteradvice

*/


//			dropdown for Mode (Filter or Card Name Match)
			//ParentNotePanel.Dock = DockStyle.Top;
			CaptionLabel.Dock = DockStyle.Top;


//			dropdown for Fitler to use
			CurrentFilterDropDown = new ComboBox();
			CurrentFilterDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
			CurrentFilterDropDown.Dock= DockStyle.Bottom;
			CurrentFilterDropDown.Enabled = false;


			CurrentModeDropDown = new ComboBox();
			CurrentModeDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
			CurrentModeDropDown.Dock = DockStyle.Bottom;
			CurrentModeDropDown.Enabled = true;

			CurrentModeDropDown.Items.Add (Loc.Instance.GetString("By Current Note")); // 0
			CurrentModeDropDown.Items.Add (Loc.Instance.GetString("By Current Layout")); // 1
			CurrentModeDropDown.Items.Add (Loc.Instance.GetString("By Filter")); // 2
			CurrentModeDropDown.Items.Add (Loc.Instance.GetString("By Any Keyword")); // 3
			CurrentModeDropDown.Items.Add (Loc.Instance.GetString("By All Keywords")); // 4
			CurrentModeDropDown.SelectedIndex = 0;

			CurrentModeDropDown.SelectedIndexChanged+= (object sender, EventArgs e) => {
				GenerateAppropriateList();
				if (CurrentModeDropDown.SelectedIndex == 2)
				{
					CurrentFilterDropDown.Enabled = true;
				}
				else
				{
					CurrentFilterDropDown.Enabled = false;
				}
			};


			// create our own cotnrols
			label1 = new Label();
			label1.Dock = DockStyle.Bottom;
			label1.Text = "";

			richText = new RichTextBox();
			richText.Dock = DockStyle.Fill;
			//richText.Height = 400;
			richText.Text = "Details of Note";
			richText.ReadOnly = true;

			 playButton = new Button();
			playButton.Dock = DockStyle.Fill;//DockStyle.Left;
			playButton.Click+= (object sender, EventArgs e) => {
				//NewMessage.Show ("Is playing = " + IsPlaying);
				if (IsPlaying == false)
				{
					ParentNotePanel.Cursor = Cursors.WaitCursor;
					playButton.Text = "||";
					IsPlaying = true;
					if (bw == null) ResetBackgroundWorker();
					if (bw.IsBusy == false)
					{
						bw.RunWorkerAsync();
					}
				}
				else
				{

					Pause ();
				}
			};
			playButton.Text = Loc.Instance.GetString(">");

//			Button pauseButton = new Button();
//			pauseButton.Dock = DockStyle.Right;
//			pauseButton.Text = Loc.Instance.GetString ("||");
//			pauseButton.Click+= (object sender, EventArgs e) => 
//			{
//				Pause();
//			};

			Button nextButton = new Button();
			nextButton.Text = Loc.Instance.GetString (">>");
			nextButton.Dock = DockStyle.Right;
			nextButton.Click+= (object sender, EventArgs e) => {
				ParentNotePanel.Cursor = Cursors.WaitCursor;
				SkipABeat = 0;
				// ** Select a Note from the List **
				SelectAndOpenRandomNote(true);
				ParentNotePanel.Cursor = Cursors.Default;
			};

			Button prevButton = new Button();
			prevButton.Text = Loc.Instance.GetString ("<<");
			prevButton.Dock = DockStyle.Left;
			prevButton.Click+= (object sender, EventArgs e) => {
				ParentNotePanel.Cursor = Cursors.WaitCursor;
				SkipABeat = 0;
				// ** Select a Note from the List **
				SelectAndOpenRandomNote(false);
				ParentNotePanel.Cursor = Cursors.Default;
			};


			Panel playControls = new Panel();
			playControls.Name ="playcontrols";
			playControls.Height = 25;
			playControls.Dock = DockStyle.Top;

			playControls.Controls.Add (prevButton);
			playControls.Controls.Add (playButton);
		//	playControls.Controls.Add (pauseButton);
			playControls.Controls.Add (nextButton);

			prevButton.SendToBack();

			Panel lowerPanel = new Panel();
			lowerPanel.Name = "lowerpanel";


			lowerPanel.Controls.Add (label1);
			lowerPanel.Controls.Add (playControls);
			lowerPanel.Controls.Add (richText);
			lowerPanel.Controls.Add (CurrentFilterDropDown);
			lowerPanel.Controls.Add (CurrentModeDropDown);
			playControls.BringToFront();
			richText.BringToFront();
			lowerPanel.Parent = ParentNotePanel;
			lowerPanel.Height = 400;
			lowerPanel.Dock = DockStyle.Fill;
			//playControls.SendToBack();
			lowerPanel.BringToFront();
			//ParentNotePanel.Controls.Add (lowerPanel);

		//	richText.BringToFront();
			// because we need the tables to be loaded we CANNOT load this data now
			LayoutDetails.Instance.UpdateAfterLoadList.Add (this);


			// Add lookup form
			 lookup = new lookupControl();
			lookup.Height = 200;
			lookup.Parent = ParentNotePanel;
			lookup.Dock = DockStyle.Bottom;
			//lookup.BringToFront();


		}
		private bool WorkerRunning = false; // we use this as a manual toggle to stop running the worker

		protected override void DoChildAppearance (AppearanceClass app)
		{
			base.DoChildAppearance (app);

			;
			
		}
		BackgroundWorker bw = null;

		protected void ResetBackgroundWorker()
		{
			WorkerRunning = true;
			bw = new BackgroundWorker();
			
			// this allows our worker to report progress during work
			bw.WorkerReportsProgress = true;


			// what to do in the background thread
			bw.DoWork += new DoWorkEventHandler(
				delegate(object o, DoWorkEventArgs args)
				{
				if ( o == null) return;

				BackgroundWorker b = o as BackgroundWorker;
				
				// do some simple processing for 10 seconds
			//	for (int i = 1; i <= 10; i++)

				while (WorkerRunning)
				{

					if (LayoutDetails.Instance.CurrentLayout != null)
					{
						// we test to see if we are no longer the current layout.
						// if we are not, we stop running.
						if (LayoutDetails.Instance.CurrentLayout != this.Layout)
						{
							Pause();
						}
					}

					// report the progress in percent
					b.ReportProgress(0);


					// slowed from 1000 to 2000; see impact 16/016/2014
					Thread.Sleep(THROTTLE);
				}
				
			});
			
			// what to do when progress changed (update the progress bar for example)
			bw.ProgressChanged += new ProgressChangedEventHandler(
				delegate(object o, ProgressChangedEventArgs args)
				{
				UpdateSearch();
				//label1.Text = string.Format("{0}% Completed", args.ProgressPercentage);
			});
			
			// what to do when worker completes its task (notify the user)
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
				delegate(object o, RunWorkerCompletedEventArgs args)
				{
				// set wait cursor back to normal
				ParentNotePanel.Cursor = Cursors.Default;
			//	label1.Text = "";
			});

			//
			// Moved to the play button bw.RunWorkerAsync();
		}

		private string LastLayoutWeWereOn=Constants.BLANK;

		// this is used to SLOW how often we show a a NEW NOTE -- we want the update to run fast
		// to see if we change notes (context) but we don't watn the parade of random notes to slide past too quickly.
		// so we throttle it down a bit.
		private int SkipABeat = 0;
		private void UpdateSearch ()
		{
			string layoutname="(none)";
			string cardname = "";
			string selectedText = "";
			//label1.Text = string.Format("{0}% Completed", args.ProgressPercentage);
			if (LayoutDetails.Instance.CurrentLayout != null) {
				layoutname = LayoutDetails.Instance.CurrentLayout.Caption;
				if (LayoutDetails.Instance.CurrentLayout.CurrentTextNote != null)
				{
					cardname = LayoutDetails.Instance.CurrentLayout.CurrentTextNote.Caption;
					try
					{
					if (LayoutDetails.Instance.CurrentLayout.CurrentTextNote.SelectedText != Constants.BLANK)
						selectedText = LayoutDetails.Instance.CurrentLayout.CurrentTextNote.SelectedText;
					}
					catch (Exception)
					{
					}
					if (LastLayoutWeWereOn == cardname)
					{
						SkipABeat++;
						if (SkipABeat > 5) // 10 was really slow
						{
						richText.Text = "We Are On The Same Note But Time Passed. We should pick another note to show.";
						SkipABeat = 0;
						// ** Select a Note from the List **
						SelectAndOpenRandomNote(true);
						}

					}
					else
					{

						// ** Label Display ** 

						//richText.Text = "We Are On New Note!!";
						LastLayoutWeWereOn = cardname;
					

						//////////////////////////////////
						// * GENERATE THE ACTUAL IDEA RIVER

						// ** Generate a New List **

						GenerateAppropriateList();
						// ** Select a Note from the List **
						SelectAndOpenRandomNote(true);


					}
					if (selectedText != "")
					{

						/// -- Show the definition/thesaurus for words when they are selected
						if (selectedText != lastSelectedText)
						{
							lastSelectedText = selectedText;
							if (LookupTurnedOn)
							{
								lookup.SetNewWord(lastSelectedText);
							}
						}
						selectedText = String.Format ("({0})", selectedText);
					}
					label1.Text = String.Format("{0}: {1} {2}", layoutname, cardname, selectedText);
				}


			}

		}
		private string lastSelectedText = "";
		// this is toggled with a button  (or could be. At this point I'm not using it, because the Play/Pause button
		// is effectively capturing this behavior
		private bool LookupTurnedOn = true;

		/// <summary>
		/// Generates the appropriate list.
		/// based on the current mode
		/// </summary> 
		private void GenerateAppropriateList ()
		{

			string details = "";

			advisorQueryMakerBase QueryMaker = null;

			int mode = CurrentModeDropDown.SelectedIndex;
			switch (mode) {
				case 0:
				if (LayoutDetails.Instance.CurrentLayout != null && LayoutDetails.Instance.CurrentLayout.CurrentTextNote != null) {
					details = LayoutDetails.Instance.CurrentLayout.CurrentTextNote.Caption;
					details = details.ToLower ();
				}
				QueryMaker = new advisorQueryMakerBase(details);

				break;  // current note
				case 1: 
				if (LayoutDetails.Instance.CurrentLayout != null ) {
					details = LayoutDetails.Instance.CurrentLayout.Caption;
					details = details.ToLower ();
				}
				QueryMaker = new advisorQueryMakerBase(details);

				break; // current layout
				case 2:
				string filterstring = "All";
				if (CurrentFilterDropDown.SelectedItem != null)
				{
					filterstring = CurrentFilterDropDown.SelectedItem.ToString();
				}
				//NewMessage.Show ("Using Filter = " + filterstring);
					QueryMaker = new advisorQueryJustFilter(filterstring);

				break; // the filter
			case 3:
				if (LayoutDetails.Instance.CurrentLayout != null) {
					details = LayoutDetails.Instance.CurrentLayout.Keywords;
					//NewMessage.Show (details);
				}
				QueryMaker = new advisorKeywords(details, false);
				break;
			case 4:
				if (LayoutDetails.Instance.CurrentLayout != null) {
					details = LayoutDetails.Instance.CurrentLayout.Keywords;
					//NewMessage.Show (details);
				}
				QueryMaker = new advisorKeywords(details, true);
				break;
			}
			
			
			

			
			GenerateNewList(QueryMaker);
			SkipABeat = 0;
		}

		List<MasterOfLayouts.NameAndGuid> lastFoundItems = null;

		/// <summary>
		/// Generates the new list.
		/// 
		/// A MEMORY only list -- we can get (and should) get rid of the note listCONTROL  itself soon TODO
		/// </summary>
		private void GenerateNewList (advisorQueryMakerBase QueryMaker)
		{

			lastFoundItems = QueryMaker.BuildQuery();

			// we shuffle now so that the list can still be navigated later (i.e., tehre's still a concept for previous and next)
			lastFoundItems.Shuffle();

//			string notelist = "";
//			if (lastFoundItems != null) {
//				foreach (MasterOfLayouts.NameAndGuid name in lastFoundItems)
//				{
//					notelist = notelist + "\n" + name.Caption;
//				}
//			}
			//richText.Text = richText.Text +"\n" + "List of notes found: " + notelist;
			ShuffleCount = 0;
		}
		private int ShuffleCount = 0;
		private void SelectAndOpenRandomNote (bool forward)
		{
			LayoutDetails.Instance.SuspendTitleUpdate(true);

			try {
				richText.Text = "";
				// take the first note

				if (lastFoundItems != null && lastFoundItems.Count > 0 && ShuffleCount < lastFoundItems.Count) {

					MasterOfLayouts.NameAndGuid FoundRecord = lastFoundItems [ShuffleCount];
					string returnresult = richText.Text + "\n" + FoundRecord.Caption;
					richText.Text = "";


					ContextMenuStrip TextEditContextStrip = null; 
					LayoutPanel temporaryLayoutPanel = new LayoutPanel ("", false);
					temporaryLayoutPanel.LoadLayout (FoundRecord.Guid, false, TextEditContextStrip);


					NoteDataInterface table = temporaryLayoutPanel.GetRandomNote ();
					if (table != null && (table.IsPanel || table is NoteDataXML_Timeline)) {
						// skip Panel Notes
					} else {
						if (table != null) {
							returnresult = returnresult + "Note: " + table.Caption;
						}

						if (table != null && !(table is NoteDataXML_Table)) {
							// not a table. Just grab text details.
							if (table.Data1 != Constants.BLANK) {

								richText.Rtf = table.Data1;
							}
						} else
					if (table != null && table is NoteDataXML_Table) {
							//	returnresult = returnresult + " Examing table ";
							// now convert the note to a proper object (with a layout to reference, which is needed when looking for notes on a SUBPANEL)
							table = temporaryLayoutPanel.GoToNote (table);
							if (table != null) {
								//found = true;
								returnresult = returnresult + "\n" + ((NoteDataXML_Table)table).GetRandomTableResults ();
							}
						}

						richText.Text = returnresult + "\n" + richText.Text;
					}

					if (forward)
						ShuffleCount++;
					else
						ShuffleCount--;

					if (ShuffleCount >= lastFoundItems.Count) {
						ShuffleCount = 0; // reset back to initial
					}
					if (ShuffleCount < 0)
						ShuffleCount = lastFoundItems.Count - 1;
				}
			} catch (System.Exception ex) {
				NewMessage.Show (ex.ToString ());
			}
			LayoutDetails.Instance.SuspendTitleUpdate(false);
		}


		/// <summary>
		/// Loads the list of potential queries into the combo boxes. (Copy paste from NoteDataXML_Notelist.cs)
		/// </summary>
		public override void UpdateAfterLoad ()
		{
			//NewMessage.Show ("loading");
			System.Collections.Generic.List<string> queries = new List<string> ();
			//how and when to load this 
			// because we need the tables to be loaded we CANNOT load this data now
			if (LayoutDetails.Instance.TableLayout != null) {
				
				queries = LayoutDetails.Instance.TableLayout.GetListOfStringsFromSystemTable (LayoutDetails.SYSTEM_QUERIES, 1);
			} else {
				// first load The SYstem Table won't be ready so we grab it directly FROM US
				queries = Layout.GetListOfStringsFromSystemTable(LayoutDetails.SYSTEM_QUERIES, 1);
				//Layout.GetNoteOnSameLayout(LayoutDetails.SYSTEM_QUERIES, false);
			}
			queries.Sort ();
			CurrentFilterDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
			foreach (string s in queries) {
				CurrentFilterDropDown.Items.Add (s);
			}
			int lastQueryIndex =queries.IndexOf(CurrentFilterName);// queries.Find(s=>s==CurrentFilter);
			CurrentFilterDropDown.SelectedIndex = lastQueryIndex;
			CurrentFilterDropDown.SelectedIndexChanged+= HandleSelectedIndexLastQueryChanged;
			
			
		
		}
		/// <summary>
		/// Handles the selected index last query changed.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		void HandleSelectedIndexLastQueryChanged (object sender, EventArgs e)
		{
			
			SetSaveRequired(true);
			if ((sender as ComboBox).SelectedItem != null && (sender as ComboBox).SelectedItem.ToString () != Constants.BLANK) {
				//NewMessage.Show ("Set to " + (sender as ComboBox).SelectedItem.ToString ());
				CurrentFilterName = (sender as ComboBox).SelectedItem.ToString ();

				GenerateAppropriateList();

			} else {
				//NewMessage.Show ("Did not set");
			}
		}
	}



}
