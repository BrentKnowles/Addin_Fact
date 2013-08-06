using System;
using CoreUtilities;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using CoreUtilities.Links;
using System.Data;
using Layout;



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
		
	}
}

