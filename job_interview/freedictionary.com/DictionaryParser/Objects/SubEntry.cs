using System;
using System.Xml.Linq;

namespace DictionaryParser.Objects
{
	internal class SubEntry
	{
		internal XElement Entry { get; set; }
		internal String Headword { get; set; }
	}
}
