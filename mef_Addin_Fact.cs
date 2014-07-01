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
	 *
	 *
	 *THIS CLASS Is Incomplete as of 14/06/2014
	 * -- The idea was that it would "parse" the Text [defined as what?] and looked for the [[token]] matches and create
	 *    hyperlinks to those matches.
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

		/// <summary>
		/// Gets the dependencyguid.
		/// 16/06/2014 -- added this dependency because we will reuse the lookup form.
		/// </summary>
		/// <value>
		/// The dependencyguid.
		/// </value>
		public override string dependencyguid {
			get {
				return "lookupword";
			}
		}
	
		public override string dependencymainapplicationversion {
			get {
				return "1.5.2.0";
			}
		}
		/*
		 * 1.2.0.1 - setting up Fact note to actually parse for facts
		 * 1.1.0.0 - adding the "brainstormer"
		*/
		public string Version
		{
			get { return @"1.2.0.1"; }
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
			Layout.LayoutDetails.Instance.AddToList(typeof(NoteDataXML_Advisor), Loc.Instance.GetString ("Advisor"),Loc.Instance.GetString ("Organization"));
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

