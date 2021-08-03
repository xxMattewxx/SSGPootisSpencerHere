using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SSGDistributer
{
	public class APIServer
	{
		private Dictionary<string, Action<HttpListenerContext, StreamWriter, StreamReader>> handlers = new Dictionary<string, Action<HttpListenerContext, StreamWriter, StreamReader>>();
		private HttpListener listener;

		public APIServer(int port)
		{
			listener = new HttpListener();
			listener.Prefixes.Add("http://+:" + port + "/");
			Console.WriteLine("Listening on port {0}", port);
		}

		public void Listen()
		{
			listener.Start();
			while (listener.IsListening)
			{
				var context = listener.GetContext();

				Task.Run(() => {
					try
					{
                        using StreamWriter writer = new StreamWriter(context.Response.OutputStream);
                        using StreamReader reader = new StreamReader(context.Request.InputStream);

                        ProcessContext(context, writer, reader);
                    }
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
				});
			}
		}

		private void ProcessContext(HttpListenerContext context, StreamWriter writer, StreamReader reader)
		{
			Uri url = context.Request.Url;
			string handler = url.AbsolutePath;

			if (!handlers.ContainsKey(handler))
			{
				writer.Write("Invalid handler.");
				return;
			}

			handlers[handler].Invoke(context, writer, reader);
		}

		public void AddAction(string handler, Action<HttpListenerContext, StreamWriter, StreamReader> action)
		{
			handlers.Add(handler, action);
		}
	}
}
