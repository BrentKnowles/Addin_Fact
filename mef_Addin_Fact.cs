namespace MefAddIns
{
	
	using MefAddIns.Extensibility;
	using System.ComponentModel.Composition;
	using System;
	using System.Windows.Forms;
	using CoreUtilities;
	using ADD_Facts;
	using Layout;
	/*Design
	 * Creates a note type that is based on a table.
	 * The table populates the Checked List
	 * The table is in turn populated by a reference to a Note. Each "line" is a row in the table. Each row then becomes a check list item.
	 * "Linking" is determined by the wording of the text (that's the 'guid').
	 * 
	 * Reason for adding: Wanted a way to maintain a checklist of "how to write a novel" and add new things to it that could be fed into previously 
	 * created layouts that had links to the note.
	 * 
	 * So basically the text-note itself is linked to in each of these derived layouts and then a checklist is added, feeding from the linked note.
	 */
	[Export(typeof(mef_IBase))]
	public class Addin_Facts:PlugInBase, mef_IBase
	{
		
		
		public Addin_Facts ()
		{
			guid = "FactNote";
		}
		#region properties
		public string Author
		{
			get { return @"Brent Knowles"; }
		}
		public string Version
		{
			get { return @"1.0.0.0"; }
		}
		public string Description
		{
			get { return Loc.Instance.GetString ("A text note with some additional meta-data for organizing fact information on large projects."); }
		}
		public string Name
		{
			get { return @"Facts"; }
		}
#endregion
		
		public override bool DeregisterType ()
		{
			
			
			return true;
			//Layout.LayoutDetails.Instance.AddToList(typeof(NoteDataXML_Picture.NoteDataXML_Pictures), "Picture");
		}
		public override void RegisterType()
		{
			//NewMessage.Show ("Registering Picture");
			Layout.LayoutDetails.Instance.AddToList(typeof(NoteDataXML_Facts), Loc.Instance.GetString ("Fact"),Loc.Instance.GetString ("Organization"));
		}

		
		public void ActionWithParamForNoteTextActions (object param)
		{
			// not used for this addin
		}
		
		public void RespondToMenuOrHotkey<T> (T form) where T: System.Windows.Forms.Form, MEF_Interfaces.iAccess
		{
			// not used
			
		}
		void HandleFormClosing (object sender, FormClosingEventArgs e)
		{
			//	RemoveQuickLinks();
		}
		//		public override object ActiveForm ()
		//		{
		//			return storySales;
		//		}
		public PlugInAction CalledFrom { 
			get
			{
				PlugInAction action = new PlugInAction();
				action.MyMenuName = Loc.Instance.GetString("Facts");
				action.ParentMenuName = "";
				action.IsOnContextStrip = false;
				action.IsOnAMenu = false;
				action.IsNoteAction = false;
				action.QuickLinkShows = true;
				action.IsANote = true;
				action.ToolTip = Description;
				action.GUID = GUID;	
				
				
				return action;
			} 
		}
		
	}
}

