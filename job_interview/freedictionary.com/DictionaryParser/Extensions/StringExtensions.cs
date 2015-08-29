using System;

namespace DictionaryParser.Extensions
{
	/// <summary>
	/// Extensions for the <see cref="String"/> type.
	/// </summary>
	internal static class StringExtensions
	{
		/// <summary>
		/// Slash string constant.
		/// </summary>
		private const String Slash = "/";

		/// <summary>
		/// HTML space string constant.
		/// </summary>
		private const String HtmlSpace = "&nbsp;";

		/// <summary>
		/// Remove any occurrence of <paramref name="stringToRemove"/> before <paramref name="character"/> in the <paramref name="word"/>.
		/// </summary>
		/// <param name="word">Word to process.</param>
		/// <param name="stringToRemove">String to remove.</param>
		/// <param name="character">Character to search in the <paramref name="word"/>.</param>
		/// <returns>String without occurence of <paramref name="stringToRemove"/> before specified <paramref name="character"/>.</returns>
		internal static String RemoveBefore(this String word, String stringToRemove, String character)
		{
			var index = word.IndexOf(character, StringComparison.Ordinal);
			var stringToRemoveLength = stringToRemove.Length;
			while (index != -1)
			{
				if (index >= stringToRemoveLength && word.Substring(index - stringToRemoveLength, stringToRemoveLength) == stringToRemove)
				{
					word = word.Remove(index - stringToRemoveLength, stringToRemoveLength);
					index -= stringToRemoveLength;
				}
				else
				{
					index = word.IndexOf(character, index + 1, StringComparison.Ordinal);
				}
			}

			return word;
		}

		/// <summary>
		/// Remove any occurrence of <paramref name="stringToRemove"/> after <paramref name="character"/> in the <paramref name="word"/>.
		/// </summary>
		/// <param name="word">Word to process.</param>
		/// <param name="stringToRemove">String to remove.</param>
		/// <param name="character">Character to search in the <paramref name="word"/>.</param>
		/// <returns>String without occurrence of <paramref name="stringToRemove"/> after specified <paramref name="character"/>.</returns>
		internal static String RemoveAfter(this String word, String stringToRemove, String character)
		{
			var index = word.IndexOf(character, StringComparison.Ordinal);
			var stringToRemoveLength = stringToRemove.Length;
			while (index != -1)
			{
				if (word.Length > index + stringToRemoveLength && word.Substring(index + 1, stringToRemoveLength) == stringToRemove)
				{
					word = word.Remove(index + 1, stringToRemoveLength);
				}
				else
				{
					index = word.IndexOf(character, index + 1, StringComparison.Ordinal);
				}
			}

			return word;
		}

		/// <summary>
		/// Remove any double occurrence of <paramref name="stringToRemove"/> and leaves only one.
		/// </summary>
		/// <param name="word">Word to process.</param>
		/// <param name="stringToRemove">String to search in the <paramref name="word"/>.</param>
		/// <returns>String without any double occurrence of the <paramref name="stringToRemove"/>.</returns>
		internal static String RemoveDouble(this String word, String stringToRemove)
		{
			var doubleString = stringToRemove + stringToRemove;
			while (true)
			{
				if (word.IndexOf(doubleString, StringComparison.Ordinal) == -1)
					return word;

				word = word.Replace(doubleString, stringToRemove);
			}
		}

		/// <summary>
		/// Removes all characters that represents whitespace. Also remove double occurrences of the space.
		/// </summary>
		/// <param name="word">Word to process.</param>
		/// <returns>String without insignificant whitespace.</returns>
		internal static String RemoveWhitespace(this String word)
		{
			return word
				.Replace("\t", " ")
				.Replace("\n", " ")
				.Replace("\r", " ")
				.RemoveDouble(" ")
				.RemoveDouble(HtmlSpace);
		}

		/// <summary>
		/// Finds last character in the inner HTML of the specified tag.
		/// </summary>
		/// <param name="tag">Tag to process.</param>
		/// <returns>Last character of the inner HTML.</returns>
		internal static Char GetLastNonTagCharacter(this String tag)
		{
			for (var i = tag.Length - 1; i >= 0; i--)
			{
				if (tag[i] == '>')
				{
					i = tag.LastIndexOf('<', i);
				}
				else
				{
					return tag[i];
				}
			}

			return default(Char);
		}

		/// <summary>
		/// Formats tag to have only tag name without any other specific information.
		/// </summary>
		/// <param name="tag">Tag to process.</param>
		/// <returns>Tag name.</returns>
		internal static String ClearTag(this String tag)
		{
			return tag.Replace(Slash, String.Empty).Trim();
		}
	}
}
