using System;
using System.Collections.Generic;
using Layout;
using System.Linq;
using CoreUtilities;

namespace ADD_Facts


{
	public class advisorKeywords: advisorQueryMakerBase
	{
		private bool matchAllKeywords= false;
		public advisorKeywords (string _extraDetails, bool MatchAllKeywords)  : base (_extraDetails)
		{
			matchAllKeywords = MatchAllKeywords;
		}
		public override List<MasterOfLayouts.NameAndGuid> BuildQuery ()
		{
			List<MasterOfLayouts.NameAndGuid> lastFoundItems = new List<MasterOfLayouts.NameAndGuid>();
			
			string namesearch = extraDetails; // will bring into space sep strings and create a query of ORS "";
			
			string filter = "All";
			
			
			
			
			List<string> CommonWords = MefAddIns.mef_Addin_Lookup_Word.GetCommonWords ();
			
			
			
			string[] Keywords = extraDetails.Split (new char[1] {'|'}, StringSplitOptions.RemoveEmptyEntries);
			
			if (Keywords != null) {
				// iterate through each CommonWord and remove fromlist
				foreach (string word in Keywords) {
					{
						if (namesearch == extraDetails) {
							// we have not added anything yet
							namesearch = String.Format ("and keywords like '%{0}%'", word);
						} else {

							if (matchAllKeywords)
							{
								namesearch = String.Format ("{0} and keywords like '%{1}%'", namesearch, word);
							}
							else
							namesearch = String.Format ("{0} or keywords like '%{1}%'", namesearch, word);
							
							// tmp 
							//richText.Text = richText.Text + namesearch;
						}
						// we override the cardname because we have found at LEAST one match.
						
						// build NAMESEARCH
						// build a OR query using remaining TITLE WORDS
					}
				}
			}
			
			
			
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

