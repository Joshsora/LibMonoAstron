using System;
using System.Collections.Generic;

namespace Astron
{
	namespace DClass
	{
		public class KeywordList
		{
			protected List<string> keywords;
			protected List<string> keywordsByName;

			// Constructors
			public KeywordList()
			{
				keywords = new List<string>();
				keywordsByName = new List<string>();
			}

			public KeywordList(KeywordList copy)
			{
				keywords = copy.keywords;
				keywordsByName = copy.keywordsByName;
			}

			// Keywords
			public bool HasKeyword(string name)
			{
				return keywordsByName.Contains(name);
			}

			public string GetKeyword(int n)
			{
				return keywords [n];
			}

			public bool HasMatchingKeywords(KeywordList other)
			{
				return keywordsByName == other.keywordsByName;
			}

			public void CopyKeywords(KeywordList other)
			{
				this.keywords = other.keywords;
				this.keywordsByName = other.keywordsByName;
			}

			public bool AddKeyword(string keyword)
			{
				if (keywordsByName.Contains(keyword))
				{
					return false;
				}

				keywordsByName.Add(keyword);
				keywords.Add(keyword);
				return true;
			}

			public uint GetKeywordCount()
			{
				return (uint)keywords.Count;
			}

			// Hash
			public virtual void GenerateHash(HashGenerator hashgen)
			{
				hashgen.AddInt(keywords.Count);
				for (int i = 0; i < keywords.Count; i++)
				{
					hashgen.AddString(keywords [i]);
				}
			}
		}
	}
}
