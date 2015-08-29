using System;
using System.Globalization;

namespace DictionaryParser.Extensions
{
	/// <summary>
	/// Extensions for the <see cref="Char"/> type.
	/// </summary>
	internal static class CharExtensions
	{
		/// <summary>
		/// Replaces non-ASCII characters with HTML compatible codes.
		/// </summary>
		/// <param name="character">Character to examine.</param>
		/// <returns>Character if it is ASCII character; otherwise, HTML code.</returns>
		internal static String ReplaceUnicodeWithTag(this Char character)
		{
			return character <= 127 ? character.ToString(CultureInfo.InvariantCulture) : String.Format("&#{0};", (Int32)character);
		}
	}
}
