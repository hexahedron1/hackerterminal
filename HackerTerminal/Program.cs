using System.Text.Json.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

if (!Directory.Exists($"/home/{Environment.UserName}/.local/share/hackerterminal"))
    Directory.CreateDirectory($"/home/{Environment.UserName}/.local/share/hackerterminal");
if (!File.Exists($"/home/{Environment.UserName}/.local/share/hackerterminal/messages.json")) {
    Console.Write("Downloading messages.json... ");
    try {
        using var client = new HttpClient();
        using var s =
            client.GetStreamAsync(
                "https://raw.githubusercontent.com/hexahedron1/hackerterminal/refs/heads/master/HackerTerminal/messages.json");
        using var fs = new FileStream($"/home/{Environment.UserName}/.local/share/hackerterminal/messages.json",
            FileMode.OpenOrCreate);
        s.Result.CopyTo(fs);
    } catch (Exception e) {
        Console.WriteLine("FAIL");
        Console.WriteLine(e);
        Console.WriteLine("Can't hack today :(");
        return;
    }
    Console.WriteLine("Done");
}
Console.WriteLine("Reading messages.json");
string json = File.ReadAllText($"/home/{Environment.UserName}/.local/share/hackerterminal/messages.json");

JObject obj = (JObject)JsonConvert.DeserializeObject(json)!;

string[] normal = obj["normal"] is null ? [] : (from x in obj["normal"]! select x.Value<string>()).ToArray();
string[] spinners = obj["spinners"] is null ? [] : (from x in obj["spinners"]! select x.Value<string>()).ToArray();



Console.WriteLine("Press Q to exit.");
Console.ForegroundColor = ConsoleColor.Green;

Random rand = new Random();

while (true) {
    if (Console.KeyAvailable) {
        var key = Console.ReadKey(true);
        if (key.KeyChar == 'q') break;
    }
    switch (rand.Next(4)) {
        case 0:
            Console.WriteLine(PickRandom(normal, rand));
            break;
        case 3:
            var spin = new Spinner(PickRandom(spinners, rand));
            var spins = rand.Next(8, 32);
            for (var i = 0; i < spins; i++) {
                spin.Spin();
                Thread.Sleep(100);
            }
            spin.Finish();
            break;
    }
}
Console.ResetColor();

T PickRandom<T>(IEnumerable<T> collection, Random? r = null) {
    r ??= new Random();
    var enumerable = collection as T[] ?? collection.ToArray();
    return enumerable.ElementAt(r.Next(enumerable.Count()));
}

class Spinner {
    public int State { get; set; }
    private string Chars = "-\\|/";
    private int Y;
    private string Label;
    public Spinner(string label) {
        Y = Console.CursorTop;
        Label = label;
    }

    public void Spin() {
        State++;
        if (State == 4) State = 0;
        Console.SetCursorPosition(0, Y);
        Console.Write($"{Label} {Chars[State]}");
    }

    public void Finish() {
        Console.SetCursorPosition(0, Y);
        Console.WriteLine($"{Label} +");
    }
}