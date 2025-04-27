using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.Json;
var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;

var listener = new TcpListener(ip, port);
listener.Start();

while (true)
{
    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var bw = new BinaryWriter(stream);
    var br = new BinaryReader(stream);

    while (true)
    {
    var input = br.ReadString();
    Console.WriteLine(input);
    var command = JsonSerializer.Deserialize<Command>(input);

        switch (command!.Text)
        {
            case Command.ProcessList:
                var processes = Process.GetProcesses();
                var processNames = JsonSerializer.Serialize(processes.Select(p => p.ProcessName));
                bw.Write(processNames);
                break;
            case Command.Kill:
                try
                {
                    var processesToKill = Process.GetProcessesByName(command.Param);

                    if (processesToKill.Length == 0)
                    {
                        bw.Write($"No process with name '{command.Param}' found.");
                    }
                    else
                    {
                        foreach (var proc in processesToKill)
                        {
                            proc.Kill();
                        }
                        bw.Write($"Killed {processesToKill.Length} instance(s) of '{command.Param}'.");
                    }
                }
                catch (Exception ex)
                {
                    bw.Write($"Failed to kill process: {ex.Message}");
                }
                break;

            case Command.Run:
                var process = Process.Start(command.Param);
                bw.Write($"{process.ProcessName} is started");
                break;
            default:
                Console.WriteLine("Unknown command");
                break;
        }
    }
}
class Command
{
    public const string ProcessList = "PROCLIST";
    public const string Kill = "KILL";
    public const string Run = "RUN";

    public string? Text { get; set; }
    public string? Param { get; set; }
}
