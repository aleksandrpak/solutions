using System;
using System.IO;
using System.Net;
using System.Text;
using Grapevine;
using Grapevine.Server;
using TextIndexing.Library;

namespace TextIndexing.Service
{
	public sealed class LibraryResource : RESTResource
	{
		private enum WatchType
		{
			File,
			Directory
		}

		[RESTRoute(Method = HttpMethod.POST, PathInfo = "/api/v1/get_files")]
		public void GetFiles(HttpListenerContext context)
		{
			var payload = GetPayload(context);
			if (String.IsNullOrWhiteSpace(payload))
			{
				SendTextResponse(context, Error.InvalidQuery.ToMessage());
				return;
			}

			var result = Index.Instance.GetFiles(payload);
			var builder = new StringBuilder("{\"result\":[");
			
			// TODO: Implement caching of results?
			foreach (var file in result)
				builder.AppendFormat("\"{0}\",", file);

			if (builder[builder.Length - 1] == ',') // Remove last comma
				builder.Remove(builder.Length - 1, 1);

			builder.Append("]}");

			SendTextResponse(context, builder.ToString());
		}

		[RESTRoute(Method = HttpMethod.PUT, PathInfo = "/api/v1/watch_file")]
		public void WatchFile(HttpListenerContext context)
		{
			Watch(context, WatchType.File);
		}

		[RESTRoute(Method = HttpMethod.PUT, PathInfo = "/api/v1/watch_directory")]
		public void WatchDirectory(HttpListenerContext context)
		{
			Watch(context, WatchType.Directory);
		}

		private void Watch(HttpListenerContext context, WatchType watchType)
		{
			var name = GetPayload(context);
			if (String.IsNullOrWhiteSpace(name))
			{
				SendTextResponse(context, Error.InvalidName.ToMessage());
				return;
			}

			Error result;
			switch (watchType)
			{
				case WatchType.File:
					result = Watcher.WatchFile(name);
					break;

				case WatchType.Directory:
					result = Watcher.WatchDirectory(name);
					break;

				default:
					result = Error.FailedToWatch;
					break;
			}

			SendTextResponse(context, result.ToMessage());
		}

		private static String GetPayload(HttpListenerContext context)
		{
			if (!context.Request.HasEntityBody || context.Request.ContentType != "text/plain")
				return null;

			using (var reader = new StreamReader(context.Request.InputStream))
				return reader.ReadToEnd();
		}
	}
}
