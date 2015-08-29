using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DictionaryParser.Extensions;
using DictionaryParser.Objects;

namespace DictionaryParser.Tools
{
	internal class Parser
	{
		#region Constants

		#region Text

		private const String TextSymbol = "Symbol for";
		private const String TextAbbreviation = "Abbreviation for";

		#endregion

		#region Html

		private const String HtmlSpace = "&nbsp;";
		private const String HtmlNormalSpan = "<span style='font-weight:normal;font-style:normal;font-variant:normal;'>";
		private const String HtmlEndSpan = "</span>";
		private const String HtmlStartItalics = "<i>";
		private const String HtmlEndItalics = "</i>";
		private const String HtmlStartBold = "<b>";
		private const String HtmlEndBold = "</b>";
		private const String HtmlStartAnchor = "<a href='{0}'>";
		private const String HtmlEndAnchor = "</a>";
		private const String HtmlListDiv = "<div class='ds-list'>";
		private const String HtmlSequenceFirst = "{0}.</b> ";
		private const String HtmlSequenceOther = "</div><div class='ds-list'><b>{0}.</b> ";

		#endregion

		#region Tags

		private const String TagEntryList = "ENTRYLIST";
		private const String TagEntry = "ENTRY";
		private const String TagLem = "LEM";
		private const String TagSen = "SEN";
		private const String TagLor = "LOR";

		private const String TagSapec = "SAPEC";
		private const String TagVor = "VOR";
		private const String TagVorBio = "VOR LEM=\"BIO\"";
		private const String TagVorl = "VORL";
		private const String TagLemBio = "LEM CLS=BIO";
		private const String TagHp = "HP";
		private const String TagSp = "SP";
		private const String TagIor = "IOR";
		private const String TagUse = "USE";
		private const String TagPuse = "PUSE";
		private const String TagStxt = "STXT";
		private const String TagRef = "REF";
		private const String TagSee = "SEE";
		private const String TagGsee = "GSEE";
		private const String TagSeeAl = "SEEAL";
		private const String TagSeeUn = "SEEUN";
		private const String TagPron = "PRON";

		private const String TagSub = "SUB";
		private const String TagSuper = "SUPER";

		private const String TagAbb = "ABB";
		private const String TagSubspec = "SUBSPEC";
		private const String TagItal = "ITAL";
		private const String TagGenus = "GENUS";
		private const String TagGenspec = "GENSPEC";

		#endregion

		#endregion

		#region Fields

		/// <summary>
		/// HTML replaces for the start tags in XML.
		/// </summary>
		private static readonly Dictionary<String, String> StartTagReplace;

		/// <summary>
		/// HTML replaces for the end tags in XML.
		/// </summary>
		private static readonly Dictionary<String, String> EndTagReplace;

		/// <summary>
		/// Tags to ignore in the headword XML.
		/// </summary>
		private static readonly List<String> HeadwordIgnoreTags;

		/// <summary>
		/// Tag to remove including their inner XML in the headword XML.
		/// </summary>
		private static readonly List<String> HeadwordRemoveTags;

		/// <summary>
		/// Tags that represents links in the article.
		/// </summary>
		private static readonly Dictionary<String, RelationType> ReferenceTags;

		/// <summary>
		/// Flow of process information of the parser work.
		/// </summary>
		private readonly Subject<String> _parseInformation;

		#endregion

		#region Constructors

		static Parser()
		{
			HeadwordIgnoreTags = new List<String>
				{
					TagSub,
					TagSuper,
					TagItal,
					TagHp,
					TagSp,
					"STRS",
					TagGenus,
					TagGenspec,
					"MHP",
					"SMCAP",
					"INSUB",
					TagRef
				};

			HeadwordRemoveTags = new List<String>
				{
					TagSubspec,
					TagAbb,
					"RSEQ",
					"CIT",
					"OFFALT"
				};

			ReferenceTags = new Dictionary<String, RelationType>
				{
					{TagSeeAl, RelationType.SeeAlso},
					{"SYXT", RelationType.Synonym},
					{"SYNX", RelationType.Synonym},
					{"SYNX OFFALT=\"Y\"", RelationType.Synonym},
					{TagRef, RelationType.Synonym},
					{TagSee, RelationType.Synonym},
					{"GSEE", RelationType.Synonym},
					{"GSEE TY=\"1\"", RelationType.Synonym},
					{TagSeeUn, RelationType.Synonym}
				};

			StartTagReplace = new Dictionary<String, String>();
			EndTagReplace = new Dictionary<String, String>();

			var replacements = File.ReadAllLines(Path.GetFullPath(@"XmlData/Replacements.txt"));

			const Char tab = '\t';
			foreach (var replacement in replacements)
			{
				if (!replacement.Contains(tab))
				{
					StartTagReplace.Add(replacement, String.Empty);
					EndTagReplace.Add(replacement, String.Empty);
					continue;
				}

				var values = replacement.Split(tab);

				StartTagReplace.Add(values[0], values[1]);
				EndTagReplace.Add(values[0], values.Length >= 3 ? values[2] : String.Empty);
			}
		}

		public Parser()
		{
			_parseInformation = new Subject<String>();
		}

		#endregion

		#region Word Format Methods

		/// <summary>
		/// Removes parentheses in the <paramref name="word"/> including it's content.
		/// </summary>
		/// <param name="word">Word to process.</param>
		/// <returns>String without parentheses.</returns>
		private static String RemoveParentheses(String word)
		{
			var index = word.IndexOf('(');
			while (index >= 0)
			{
				var endIndex = word.IndexOf(')', index);
				if (endIndex == -1)
					break;

				word = word.Remove(index, endIndex - index + 1);
				index = word.IndexOf('(');
			}

			return word.Trim();
		}

		/// <summary>
		/// Indicates whether it is single word and it has only letters and numbers.
		/// </summary>
		/// <param name="headword">Headword to process.</param>
		/// <returns>True if there are only letters and numbers and other characters; otherwise, false.</returns>
		private static Boolean IsSingleWord(String headword)
		{
			const String singleWordRegex = "[a-zA-Z0-9]+";
			return Regex.IsMatch(headword, singleWordRegex);
		}

		/// <summary>
		/// Indicates whether <paramref name="nextHeadword"/> is subentry of the specified <paramref name="headword"/>.
		/// </summary>
		/// <param name="headword">Current headword.</param>
		/// <param name="nextHeadword">Next headword.</param>
		/// <returns>True if <paramref name="nextHeadword"/> contains one and only one occurrence of the <paramref name="headword"/>; otherwise, false.</returns>
		private static Boolean IsSeparateWord(String headword, String nextHeadword)
		{
			var nextWithoutHeadword = nextHeadword.Replace(headword, String.Empty);

			return
				nextWithoutHeadword.Length == nextHeadword.Length - headword.Length &&
				(nextWithoutHeadword.Trim() != nextWithoutHeadword || nextWithoutHeadword.Contains("  "));
		}

		/// <summary>
		/// Gets tag inside word starting from <paramref name="startIndex"/> and writes end of the tag in the <paramref name="endIndex"/>.
		/// </summary>
		/// <param name="word">Word to process.</param>
		/// <param name="startIndex">Start index of the tag.</param>
		/// <param name="endIndex">Found end index of the tag.</param>
		/// <returns>Tag name.</returns>
		private static String GetTag(String word, Int32 startIndex, out Int32 endIndex)
		{
			endIndex = word.IndexOf('>', startIndex);
			return word.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
		}

		#endregion

		#region Reference Methods

		/// <summary>
		/// Adds <see cref="RelationType.Synonym"/> reference containing chemical element name to the headword if it contains one.
		/// </summary>
		/// <param name="headword">Headword to process.</param>
		/// <param name="references">Collection of references where to add new one.</param>
		private static void AddReferenceToChemicalElement(String headword, IDictionary<String, RelationType> references)
		{
			const String regexString = "^(?<Numbers>\\d+)[A-Z][a-z]\\s";
			if (!Regex.IsMatch(headword, regexString))
				return;

			var match = Regex.Match(headword, regexString);
			var numbers = match.Groups["Numbers"].Value;

			if (references.ContainsKey(numbers))
				return;

			references.Add(headword.Remove(0, numbers.Length), RelationType.Synonym);
		}

		/// <summary>
		/// Gets final headword to reference using <paramref name="entries"/> and <paramref name="subEntries"/>.
		/// Using this method because <paramref name="referenceHeadword"/> can point to subentry and it depends on the main entry.
		/// So in the we have add reference to the main entry. This why we can change value of the <paramref name="referenceHeadword"/>.
		/// </summary>
		/// <param name="entries">Collection of all entries available.</param>
		/// <param name="subEntries">Collection of subentries that can point to the headword.</param>
		/// <param name="referenceHeadword">Reference headword to look for.</param>
		/// <returns>Headword to reference.</returns>
		private static String GetHeadwordReference(Dictionary<XElement, Headword> entries, IDictionary<String, SubEntry> subEntries, String referenceHeadword)
		{
			if (entries.Any(i => i.Value.HeadwordValue == referenceHeadword))
				return referenceHeadword;

			if (!subEntries.ContainsKey(referenceHeadword))
				return null;

			while (subEntries.ContainsKey(referenceHeadword))
			{
				referenceHeadword = subEntries[referenceHeadword].Headword;
			}

			return referenceHeadword;
		}

		/// <summary>
		/// Extracts all plain references entries and removes them from the entries collection.
		/// </summary>
		/// <param name="entries">Collection of the entries.</param>
		/// <param name="subEntries">Collection of the subentries.</param>
		/// <returns>Collection of all references.</returns>
		private IDictionary<String, Reference> ExtractReferences(Dictionary<XElement, Headword> entries, IDictionary<String, SubEntry> subEntries)
		{
			var references = new Dictionary<String, Reference>();
			var entriesToDelete = new List<XElement>();

			foreach (var entry in entries.Keys)
			{
				if (ReferenceTags.All(i => !entry.GetInnerXml().Contains(i.Key)))
					continue;

				var senseInformation = entry.Element(TagSen);
				var lemma = entry.Element(TagLem);

				if (senseInformation == null && lemma == null)
					continue;

				var referenceInformation = (senseInformation != null ?
					senseInformation.Elements() :
					lemma.ElementsAfterSelf()).ToArray();

				var tag = referenceInformation[0];
				var tagName = tag.Name.LocalName;
				var text = tag.Value.Trim();

				if (tagName == TagStxt && tag.Elements(TagRef).Count() == 1 &&
					(text.StartsWith(TextSymbol) || text.StartsWith(TextAbbreviation)))
				{
					tagName = TagRef;
					tag = tag.Element(TagRef);
				}

				if (referenceInformation.Length > 1 || !ReferenceTags.ContainsKey(tagName))
					continue;

				var referenceType = ReferenceTags[tagName];

				// Need to have article to have see also relation.
				if (referenceType == RelationType.SeeAlso)
					continue;

				var words = tag
					.GetInnerXml()
					.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(i => i.Trim());

				foreach (var referenceHeadword in words.Select(word => GetHeadwordReference(
																				entries,
																				subEntries,
																				GetHeadword(word).HeadwordValue)))
				{
					if (referenceHeadword == null)
					{
						if (senseInformation == null ||
							!senseInformation.GetInnerXml().Contains(TextAbbreviation) &&
							!entriesToDelete.Contains(entry))
						{
							entriesToDelete.Add(entry);
						}

						continue;
					}

					if (!entriesToDelete.Contains(entry))
						entriesToDelete.Add(entry);

					if (!references.ContainsKey(entries[entry].HeadwordValue))
					{
						references.Add(entries[entry].HeadwordValue, new Reference
						{
							Headword = referenceHeadword,
							Type = referenceType
						});
					}
				}
			}

			foreach (var entry in entriesToDelete)
			{
				entries.Remove(entry);
			}

			return references;
		}

		/// <summary>
		/// Extracts all subentries and removes them from the entries collection.
		/// </summary>
		/// <param name="entries">Collection of the entries.</param>
		/// <returns>Collection of all subentries.</returns>
		private Dictionary<String, SubEntry> ExtractSubEntries(IDictionary<XElement, Headword> entries)
		{
			var subEntries = new Dictionary<String, SubEntry>();
			var enumerator = entries.GetEnumerator();

			enumerator.MoveNext();
			var headword = enumerator.Current.Value.HeadwordValue;
			while (true)
			{
				if (!enumerator.MoveNext())
					break;

				var nextHeadword = enumerator.Current.Value.HeadwordValue;
				var nextEntry = enumerator.Current.Key;

				while (IsSingleWord(headword) && IsSeparateWord(headword, nextHeadword))
				{
					if (subEntries.ContainsKey(nextHeadword) && subEntries[nextHeadword].Headword != headword)
					{
						_parseInformation.OnError(new Exception(
							String.Format("{0} points to two different words: 1. {1} and {2}", nextHeadword, subEntries[nextHeadword], headword)));
					}

					subEntries[nextHeadword] = new SubEntry
					{
						Entry = nextEntry,
						Headword = headword
					};

					if (!enumerator.MoveNext())
						break;

					nextHeadword = enumerator.Current.Value.HeadwordValue;
					nextEntry = enumerator.Current.Key;
				}

				headword = nextHeadword;
			}

			foreach (var subEntry in subEntries)
			{
				entries.Remove(subEntry.Value.Entry);
			}

			return subEntries;
		}

		#endregion

		#region Entry Methods

		/// <summary>
		/// Extracts headword from the lexeme XML.
		/// </summary>
		/// <param name="lexeme">The lexeme XML.</param>
		/// <returns>Headword found in the XML.</returns>
		private Headword GetHeadword(String lexeme)
		{
			lexeme = lexeme.Trim()
				.Replace("&amp;", "&");

			var headword = new StringBuilder();
			var headwordWithTags = new StringBuilder();

			var previousCharacter = (Char)0;
			for (var i = 0; i < lexeme.Length; i++)
			{
				var character = lexeme[i];
				if (character == '\n')
					continue;

				if (character == ' ' && character == previousCharacter)
					continue;

				if (character == '<')
				{
					Int32 tagEnd;
					var tag = GetTag(lexeme, i, out tagEnd).ClearTag();

					if (HeadwordRemoveTags.Contains(tag))
					{
						while (true)
						{
							tagEnd = lexeme.IndexOf('>', tagEnd + 1);
							if (tagEnd == -1)
							{
								_parseInformation.OnError(new Exception(String.Format("No close tag for '{0}' in lexeme '{1}'", tag, lexeme)));
							}

							var tagStart = lexeme.LastIndexOf('<', tagEnd);
							var closeTag = GetTag(lexeme, tagStart, out tagEnd).ClearTag();

							if (closeTag == tag)
								break;
						}
					}
					else if (!HeadwordIgnoreTags.Contains(tag))
					{
						_parseInformation.OnError(
							new Exception(String.Format("Unknown tag ({0}) in lexeme '{1}'", tag, lexeme)));
					}
					else if (tag == TagSub || tag == TagSuper)
					{
						headwordWithTags.Append(lexeme.Substring(i, tagEnd - i + 1).Replace(TagSuper, "SUP"));
					}

					i = tagEnd;
					continue;
				}

				headword.Append(character);
				headwordWithTags.Append(character);
				previousCharacter = character;
			}

			var modifiedHeadword = RemoveParentheses(UnicodeStrings.ReplaceSymbols(headword.ToString().Trim())
				.Replace("[", String.Empty)
				.Replace("]", String.Empty));

			var isWithTags = headwordWithTags.Length != headword.Length;
			var isChanged = String.Compare(modifiedHeadword.Trim(), headword.ToString().Trim(), StringComparison.OrdinalIgnoreCase) != 0;

			return new Headword
				{
					HeadwordValue = modifiedHeadword,
					OriginalHeadword = isWithTags || isChanged ? headwordWithTags.ToString() : null
				};
		}

		/// <summary>
		/// Appends string to the article string builder but first add sequence HTML if required.
		/// Maybe used in future to add some other processing of appended string before appending.
		/// </summary>
		/// <param name="article">Builder of the article.</param>
		/// <param name="append">String to append.</param>
		/// <param name="isSequence">Indicates whether appending string is included in sequence.</param>
		/// <param name="sequenceNumber">Number of current sequence entry.</param>
		private static void Append(StringBuilder article, String append, ref Boolean isSequence, ref Int32 sequenceNumber)
		{
			if (isSequence)
			{
				article.AppendFormat(sequenceNumber == 1 ? HtmlSequenceFirst : HtmlSequenceOther, sequenceNumber++);
				isSequence = false;
			}

			article.Append(append);
		}

		/// <summary>
		/// Adds subentry to the <paramref name="article"/>.
		/// </summary>
		/// <param name="mainHeadword">Headword of the article.</param>
		/// <param name="article">Article which subentry belongs to.</param>
		/// <param name="innerXml">XML used to build article.</param>
		/// <param name="aliases">Collection of all aliases that reference this article.</param>
		/// <param name="entries">Collection of all entries.</param>
		/// <param name="subEntries">Collection of all subentries.</param>
		/// <returns>HTML string that contains article and current subentry.</returns>
		private String AddSubEntry(String mainHeadword, String article, String innerXml, Dictionary<String, RelationType> aliases,
			Dictionary<XElement, Headword> entries, IDictionary<String, SubEntry> subEntries)
		{
			return String.Format("{0}<br />{1}", article, GetArticle(mainHeadword, innerXml, aliases, entries, subEntries, true));
		}

		/// <summary>
		/// Builds article HTML.
		/// </summary>
		/// <param name="mainHeadword">Headword of the article.</param>
		/// <param name="innerXml">XML used to build article.</param>
		/// <param name="aliases">Collection of all aliases that reference this article.</param>
		/// <param name="entries">Collection of all entries.</param>
		/// <param name="subEntries">Collection of all subentries.</param>
		/// <param name="isSubEntry">Indicates whether this article is subentry or not.</param>
		/// <returns>HTML string that contains article.</returns>
		private String GetArticle(String mainHeadword, String innerXml, IDictionary<String, RelationType> aliases,
			Dictionary<XElement, Headword> entries, IDictionary<String, SubEntry> subEntries, Boolean isSubEntry = false)
		{
			var article = new StringBuilder();
			var parentTags = new List<String>();
			var lastCloseTag = String.Empty;
			var isLink = false;
		    var isSynonyms = false;
			var isPreviousLink = false;
		    var seeEnd = 0;
		    var seeReplace = String.Empty;
			var sequenceNumber = 1;
			var isSequence = false;
			var linkQueue = new Queue<String>();

			for (var i = 0; i < innerXml.Length; i++)
			{
				if (innerXml[i] == '<')
				{
					Int32 tagEndIndex;
					var tag = GetTag(innerXml, i, out tagEndIndex);

					if (tag.StartsWith("/"))
					{
						tag = tag.ClearTag();

						var parentTag = parentTags.FirstOrDefault(j => j.StartsWith(tag));
						if (parentTag != null)
							tag = parentTag;

						parentTags.Remove(tag);

						var replace = EndTagReplace[tag];

						switch (tag)
						{
							case TagVorl:
								if (parentTags.Contains(TagLemBio))
									replace = ")";
								break;

							case TagLor:
								if (isSubEntry)
									replace = HtmlEndBold;
								break;
						}

						if ((parentTags.Contains(TagUse) || parentTags.Contains(TagPuse)) &&
							(replace.Contains(HtmlStartItalics) || replace.Contains(HtmlEndItalics)))
						{
							replace = replace
								.Replace(HtmlStartItalics, HtmlNormalSpan)
								.Replace(HtmlEndItalics, HtmlEndSpan);
						}

						if (ReferenceTags.ContainsKey(tag) && isLink)
						{
							replace = String.Format("{0}{1}", isPreviousLink ? HtmlEndAnchor : String.Empty, replace);
							isLink = false;
							isPreviousLink = false;
						    isSynonyms = false;
						}

						var lastNonTagCharacter = article.ToString().GetLastNonTagCharacter();
						if (lastNonTagCharacter == '.' && replace.EndsWith("."))
						{
							replace = replace.Remove(replace.Length - 1);
						}

						if (lastNonTagCharacter == '.' && replace.EndsWith(";"))
						{
							article.Remove(article.Length - 1, 1);
						}

						if (lastCloseTag == TagPron)
						{
							if (replace.Contains('.'))
								replace = replace.Replace(".", String.Empty);

							if (replace.Contains(','))
								replace = replace.Replace(",", String.Empty);
						}

						Append(article, replace, ref isSequence, ref sequenceNumber);
						lastCloseTag = tag;
					}
					else
					{
						if (!tag.EndsWith("/"))
							parentTags.Add(tag);

						tag = tag.ClearTag();
						var replace = StartTagReplace[tag];

						switch (tag)
						{
							case TagVor:
							case TagVorBio:
								if (lastCloseTag == TagVor || lastCloseTag == TagVorBio)
									replace = String.Format(",{0}{1}", HtmlSpace, replace);
								break;

							case TagIor:
								if (lastCloseTag == TagIor)
									replace = String.Format(",{0}{1}", HtmlSpace, replace);
								break;

							case TagHp:
							case TagSp:
								if (parentTags.Contains(TagSen))
									replace = String.Empty;
								break;

							case TagVorl:
								if (parentTags.Contains(TagLemBio))
									replace = String.Format("{0}(", HtmlSpace);
								break;

							case TagLor:
								if (isSubEntry)
									replace = HtmlStartBold;
								break;

							case TagSen:
								if (innerXml.Contains(TagSapec))
									replace = HtmlListDiv + HtmlStartBold;
								break;

							case TagSapec:
								isSequence = true;
								var sapecCloseTag = String.Format("</{0}>", TagSapec);
								i = innerXml.IndexOf(sapecCloseTag, i, StringComparison.Ordinal) + sapecCloseTag.Length - 1;
								continue;
						}

						if ((parentTags.Contains(TagUse) || parentTags.Contains(TagPuse)) &&
							(replace.Contains(HtmlStartItalics) || replace.Contains(HtmlEndItalics)))
						{
							replace = replace
								.Replace(HtmlStartItalics, HtmlNormalSpan)
								.Replace(HtmlEndItalics, HtmlEndSpan);
						}

						if (ReferenceTags.ContainsKey(tag))
						{
							var linkTagEnd = String.Format("</{0}>", tag.Contains(' ') ? tag.Split(' ')[0] : tag);
							var linkTagEndIndex = innerXml.IndexOf(linkTagEnd, tagEndIndex, StringComparison.Ordinal);
							var linkXml = innerXml.Substring(tagEndIndex + 1, linkTagEndIndex - tagEndIndex - 1);
							var words = linkXml.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(k => k.Trim()).ToArray();
						    
                            isSynonyms = !(tag == TagSee || tag == TagSeeAl || tag == TagSeeUn || tag == TagGsee);
                            seeEnd = linkTagEndIndex + linkTagEnd.Length - 1;
						    seeReplace = EndTagReplace[tag];

							var isFirst = true;

							foreach (var headword in words.Select(word => GetHeadwordReference(entries, subEntries, GetHeadword(word).HeadwordValue)))
							{
								if (headword == null || headword == mainHeadword)
								{
									if (isFirst)
									{
										isFirst = false;
									    if (!isSynonyms && words.Length > 1)
                                            i = innerXml.IndexOf(',', i + 1) - 1;
									}
									else
									{
										linkQueue.Enqueue(String.Format("{0} ", isPreviousLink ? HtmlEndAnchor : String.Empty));
									}

									isPreviousLink = false;
									continue;
								}

								aliases[headword] = aliases.ContainsKey(headword) && aliases[headword] != ReferenceTags[tag]
														? (RelationType)Math.Max((Int32)aliases[headword], (Int32)ReferenceTags[tag])
														: ReferenceTags[tag];
								if (isFirst)
								{
									replace += String.Format(HtmlStartAnchor, Uri.EscapeDataString(headword.Replace(' ', '+')));
									isFirst = false;
								}
								else
								{
									linkQueue.Enqueue(String.Format("{0}, {1}",
										isPreviousLink ? HtmlEndAnchor : String.Empty,
										String.Format(HtmlStartAnchor, Uri.EscapeDataString(headword.Replace(' ', '+')))));
								}
								
								isLink = true;
								isPreviousLink = true;
							}

							if (!isLink && !isSynonyms)
							{
								// Remove See section
								isSequence = false;
                                i = seeEnd;
								continue;
							}
						}

						Append(article, replace, ref isSequence, ref sequenceNumber);
					}

					i = tagEndIndex;
					continue;
				}

				var append = innerXml[i].ReplaceUnicodeWithTag();

			    if (isLink && append == "," && linkQueue.Count > 0)
			    {
			        append = linkQueue.Dequeue();

			        if (append.EndsWith(">"))
			            i++;
			        else if (!isSynonyms)
			        {
			            if (linkQueue.Count > 0)
			                i = innerXml.IndexOf(',', i + 1) - 1;
			            else
			            {
			                if (article[article.Length - 1] == ' ')
			                    article.Remove(article.Length - 1, 1);

			                append = append.Remove(append.Length - 1) + seeReplace;
			                i = seeEnd;
                            isLink = false;
                            isPreviousLink = false;
                            isSynonyms = false;
			            }
			        }
			    }

			    Append(article, append, ref isSequence, ref sequenceNumber);
			}

			var articleText = article.ToString()
				.Replace("&star;", "&#9733;")
				.RemoveWhitespace()
				.RemoveBefore(" ", ",").RemoveBefore(HtmlSpace, ",")
				.RemoveBefore(" ", ".").RemoveBefore(HtmlSpace, ".")
				.RemoveBefore(" ", "!").RemoveBefore(HtmlSpace, "!")
				.RemoveBefore(" ", "?").RemoveBefore(HtmlSpace, "?")
				.RemoveBefore(" ", ":").RemoveBefore(HtmlSpace, ":")
				.RemoveBefore(" ", ";").RemoveBefore(HtmlSpace, ";")
				.RemoveBefore(".", ";").RemoveBefore(",", ";")
				.RemoveBefore(",", ".").RemoveAfter("&#x2027;", "-")
				.RemoveWhitespace()
				.RemoveDouble(",")
				.RemoveDouble(".")
				.RemoveDouble(HtmlSpace);

			return articleText;
		}

		#endregion

		#region Parse Methods

		public IEnumerable<KeyValuePair<Entry, List<EntryAlias>>> Parse(String filename)
		{
			// Removing all new lines in the file.
			const String emptySpaceRegex = "\\r\\n\\s+";
			var textContents = File.ReadAllText(filename);
			textContents = Regex.Replace(textContents, emptySpaceRegex, String.Empty);

			XDocument document;
			using (var reader = new StringReader(textContents))
			{
				document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
			}

			// Looking for the root of list.
			var listElement = document.Element(TagEntryList);
			if (listElement == null)
			{
				yield break;
			}

			// Extracting all entries from the list.
			var entries = listElement.Elements(TagEntry)
				.ToDictionary(i => i, i =>
					{
						var lemma = i.Element(TagLem);
						return lemma != null ? GetHeadword(lemma.Element(TagLor).GetInnerXml()) : null;
					});

			// Raising error if there is entry with empty headword.
			if (entries.ContainsValue(null))
				_parseInformation.OnError(new Exception("File contains entries with empty lemma."));

			// Extracting subentries and references.
			// Leaves in entry collections only entries that will have articles.
			var subEntries = ExtractSubEntries(entries);
			var references = ExtractReferences(entries, subEntries);

			foreach (var entry in entries)
			{
				var headword = entry.Value.HeadwordValue;
				var aliases = references
					.Where(i => i.Value.Headword == headword)
					.ToDictionary(alias => alias.Key, alias => alias.Value.Type);

				AddReferenceToChemicalElement(headword, aliases);

				var article = GetArticle(headword, entry.Key.GetInnerXml(), aliases, entries, subEntries);

				article = subEntries
					.Where(i => i.Value.Headword == headword)
					.Aggregate(article, (current, subEntry) => AddSubEntry(headword, current, subEntry.Value.Entry.GetInnerXml(), aliases, entries, subEntries));

				var entryObject = new Entry
					{
						Headword = headword,
						OriginalHeadword = entry.Value.OriginalHeadword,
						Article = article
					};

				yield return new KeyValuePair<Entry, List<EntryAlias>>(
					entryObject,
					aliases.Select(alias => new EntryAlias
						{
							Alias = alias.Key,
							RelationType = alias.Value
						}).ToList());
			}
		}

		#endregion

		#region Events

		public IObservable<String> ParseInformation
		{
			get { return _parseInformation; }
		}

		#endregion
	}
}