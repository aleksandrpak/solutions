using System;
using System.Xml.Linq;

namespace DictionaryParser.Extensions
{
	/// <summary>
	/// Extensions for the <see cref="XElement"/> type.
	/// </summary>
	internal static class XElementExtensions
	{
		/// <summary>
		/// Returns only the inner XML of the current <paramref name="element"/>.
		/// </summary>
		/// <param name="element">Element to process.</param>
		/// <returns>Inner XML.</returns>
		internal static String GetInnerXml(this XElement element)
		{
			using (var reader = element.CreateReader())
			{
				reader.MoveToContent();
				return reader.ReadInnerXml();
			}
		}
	}
}
