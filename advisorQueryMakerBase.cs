using System;
using System.Collections.Generic;
using Layout;
using System.Linq;
using CoreUtilities;

namespace ADD_Facts
{
	/// <summary>
	/// Advisor query maker base.
	/// 
	/// A wrapper class for the advisors, in case I want to make children with different behavior.
	/// </summary>
	public class advisorQueryMakerBase
	{
		protected string extraDetails = Constants.BLANK;
		public advisorQueryMakerBase (string _extraDetails)
		{
			extraDetails = _extraDetails;

		
			//.2.	"also a crash when closing -- still happenign 18/06/2014 9:18pm
		}

		public virtual List<MasterOfLayouts.NameAndGuid> BuildQuery ()
		{
			List<MasterOfLayouts.NameAndGuid> lastFoundItems = new List<MasterOfLayouts.NameAndGuid>();

			string namesearch = extraDetails; // will bring into space sep strings and create a query of ORS "";

			string filter = "All";
			
			
			
			
			List<string> CommonWords = MefAddIns.mef_Addin_Lookup_Word.GetCommonWords ();
			
			
			
			string[] CommonWordsFoundinCardName = namesearch.Split (new char[1] {' '}, StringSplitOptions.RemoveEmptyEntries);
			
			if (CommonWordsFoundinCardName != null) {
				// iterate through each CommonWord and remove fromlist
				foreach (string word in CommonWordsFoundinCardName) {
					bool q = CommonWords.Any (w => word.Contains (w));
					if (q) {
						// skip, we have a common word match
					} else {
						if (namesearch == extraDetails) {
							// we have not added anything yet
							namesearch = String.Format ("and name like '%{0}%'", word);
						} else {
							namesearch = String.Format ("{0} or name like '%{1}%'", namesearch, word);
							
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

