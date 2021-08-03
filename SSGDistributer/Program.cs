using System;

namespace SSGDistributer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 80;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--port", StringComparison.InvariantCultureIgnoreCase))
                    port = Int32.Parse(args[i + 1]);
            }

            Global.Load();

            APIServer apiServer = new APIServer(port);
            apiServer.AddAction("/tasks/get", Handlers.GetTask.ProcessContext);
            apiServer.Listen();
        }
    }
}
