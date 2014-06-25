using System;
using System.Collections.Generic;
using Layout;
using System.Linq;
using CoreUtilities;

namespace ADD_Facts
{
	public class advisorQueryJustFilter : advisorQueryMakerBase
	{
		public advisorQueryJustFilter (string _extraDetails)  : base (_extraDetails)
		{

		}
		public override System.Collections.Generic.List<Layout.MasterOfLayouts.NameAndGuid> BuildQuery ()
		{
			List<MasterOfLayouts.NameAndGuid> lastFoundItems = new List<MasterOfLayouts.NameAndGuid>();
			
			string namesearch = ""; // we ignore namesearch. Just using the FILTER. // extraDetails; // will bring into space sep strings and create a query of ORS "";
			
			string filter = extraDetails;
			
			
			
			
			List<string> CommonWords = MefAddIns.mef_Addin_Lookup_Word.GetCommonWords ();
			
			
			
		
			
			bool FullTextSearch = false;
			try {
				lastFoundItems = MasterOfLayouts.GetListOfLayouts (filter, "", FullTextSearch, null, namesearch);
			} catch (System.Exception) {
				//	NewMessage.Show (ex.ToString ());
			}
			
			return lastFoundItems;
		}
	}
}

