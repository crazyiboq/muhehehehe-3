using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;


var ip = IPAddress.Parse("127.0.0.1");
var port = 27001;

var client  = new TcpClient();
client.Connect(ip, port);

var stream = client.GetStream();
var bw = new BinaryWriter(stream);
var br = new BinaryReader(stream);

Command command = null!;

string responce = null!;

string str = null!;

while (true)
{
    Console.WriteLine("Write a command name or HELP");
    str = Console.ReadLine()!.ToUpper();
    if (str == "HELP")
    {
        Console.WriteLine();
        Console.WriteLine("COMMANDS:");
        Console.WriteLine(Command.ProcessList);
        Console.WriteLine($"{Command.Run}  <process name>");
        Console.WriteLine($"{Command.Kill}  <process name>");
        Console.ReadLine();
        Console.Clear();
    }
    var input = str.Split(' ');
    switch (input[0])
    {
        case Command.ProcessList:
            command = new Command { Text = Command.ProcessList };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();
            var processlist = JsonSerializer.Deserialize<List<string>>(responce);
            processlist.ForEach(x => Console.WriteLine($"            {x}"));

            break;
        case Command.Run:
            command = new Command { Text = input[0], Param = input[1] };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();
            Console.WriteLine(responce);

            break;
        case Command.Kill:
            if (input.Length < 2)
            {
                Console.WriteLine("Please provide a process name to kill.");
                break;
            }
            command = new Command { Text = Command.Kill, Param = input[1] };
            bw.Write(JsonSerializer.Serialize(command));
            responce = br.ReadString();
            Console.WriteLine(responce);
            break;

    }
    Console.WriteLine("Press any key to continue");
    Console.ReadLine();
    Console.Clear();
}

class Command
{
    public const string ProcessList = "PROCLIST";
    public const string Kill = "KILL";
    public const string Run = "RUN";

    public string? Text { get; set; }
    public string? Param { get; set; }
}