using System;
using TextIndexing.Library;

namespace TextIndexing.Service
{
	internal static class ErrorExtensions
	{
		public static String ToMessage(this Error error)
		{
			switch (error)
			{
				case Error.Success:
					return "{\"result\":\"Success\"}";

				case Error.InvalidName:
					return "{\"error\":\"Invalid name to watch specified\"}";

				case Error.InvalidQuery:
					return "{\"error\":\"Invalid query to search word specified\"}";

				case Error.DoesNotExist:
					return "{\"error\":\"Specified object does not exist\"}";

				case Error.AlreadyIndexed:
					return "{\"error\":\"Specified object already indexed\"}";

				case Error.FailedToIndex:
					return "{\"error\":\"Failed to index file\"}";

				case Error.FailedToWatch:
					return "{\"error\":\"Failed to watch file\"}";

				default:
					return "{\"error\":\"Unknown error occured\"}";
			}
		}
	}
}
