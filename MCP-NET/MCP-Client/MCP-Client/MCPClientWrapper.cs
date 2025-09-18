using System.Diagnostics;
using MCPSharp;

namespace MCP_Client
{
    public class MCPClientWrapper : IDisposable
    {
        public MCPClient Client { get; }
        private Process? _process;

        public MCPClientWrapper(string name, string version, string server)
        {
            Client = new MCPClient(name, version, server);

            // Start process yourself to track it (since MCPClient does it internally, you might have to find the process)
            // Alternative: Find running process by exePath or process name
            _process = FindProcessByPath(server);
        }

        private Process? FindProcessByPath(string exePath)
        {
            var processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(exePath));
            foreach (var p in processes)
            {
                try
                {
                    if (p.MainModule?.FileName.Equals(exePath, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return p;
                    }
                }
                catch
                {
                    // Access denied on some processes - ignore
                }
            }
            return null;
        }

        public void Dispose()
        {
            if (_process is { HasExited: false })
            {
                try
                {
                    _process.Kill(true);
                    _process.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to kill MCP Server process: {ex.Message}");
                }
            }
        }
    }

}
