using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

if (!Directory.Exists($"/home/{Environment.UserName}/.local/share/hackerterminal"))
    Directory.CreateDirectory($"/home/{Environment.UserName}/.local/share/hackerterminal");
#if RELEASE
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
#else
Console.WriteLine("Reading messages.json");
string json = File.ReadAllText($"test.json");
#endif

bool sleep = false;

JObject obj = (JObject)JsonConvert.DeserializeObject(json)!;

string[] normal = obj["normal"] is null ? [] : (from x in obj["normal"]! select x.Value<string>()).ToArray();
Progressbar[] pbars = new Progressbar[obj["progressbars"]?.Count() ?? 0];
int ii = 0;
foreach (var objekt in obj["progressbars"] ?? new JArray()) {
    pbars[ii] = new Progressbar(objekt["label"]?.Value<string>() ?? "null", objekt["number"]?.Value<bool>() ?? false, objekt["max"]?.Value<int>() ?? 100, objekt["wait"]?.Value<int>() ?? 0);
    ii++;
}
HackTask[] tasks = new HackTask[obj["tasks"]?.Count() ?? 0];
ii = 0;
foreach (var owl in obj["tasks"] ?? new JArray()) {
    if (owl is JObject objekt) tasks[ii] = new HackTask(objekt["label"]?.Value<string>() ??  "null") { FailChance = objekt["failchance"]?.Value<int>() ?? 10};
    else if (owl is JValue value) tasks[ii] = new HackTask(value.Value<string>() ?? "null");
    ii++;
}
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
            string? text = PickRandom(normal, rand);
            if (text is null) break;
            Console.WriteLine(text);
            break;
        case 1:
            Console.CursorVisible = false;
            var oldbar = PickRandom(pbars, rand);
            if (oldbar is null) break;
            var bar = new Progressbar(oldbar.Label, oldbar.Number, oldbar.Max, oldbar.Wait);
            bar.Sleep();
            bool working;
            do {
                working = bar.Work();
                Thread.Sleep(2);
            } while (working);
            Console.WriteLine();
            Console.CursorVisible = true;
            break;
        case 2:
            var task = PickRandom(tasks, rand);
            if (task is null) break;
            Console.Write($"{task.Label}... ");
            if (sleep)
                Thread.Sleep(rand.Next(100, 1200));
            Console.WriteLine(rand.Next(100) < task.FailChance ? "FAIL" : "Done");
            break;
        case 3:
            string? label = PickRandom(spinners, rand);
            if (label is null) break;
            var spin = new Spinner(label);
            var spins = rand.Next(8, 32);
            for (var i = 0; i < spins; i++) {
                spin.Spin();
                if (sleep)
                    Thread.Sleep(100);
            }
            spin.Finish();
            break;
    }
}
Console.ResetColor();

T? PickRandom<T>(IEnumerable<T> collection, Random? r = null) {
    r ??= new Random();
    var enumerable = collection as T[] ?? collection.ToArray();
    if (enumerable.Length == 0) return default(T);
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

class HackTask {
    public string Label { get; set; }
    public int FailChance { get; set; } = 10;

    public HackTask(string label) {
        Label = label;
    }
}

class Progressbar : ICloneable {
    public string Label { get; set; }
    private int Progress;
    public int Max { get; set; }
    public bool Number { get; set; }
    public int Wait { get; set; }
    private int Y;
    public Progressbar(string label, bool number, int max, int wait) {
        Label = label;
        Number = number;
        Max = max;
        Wait = wait;
        Y = Console.CursorTop;
    }
    // Returns whether the bar is still not full
    public bool Work() {
        Progress++;
        Console.SetCursorPosition(0, Y);
        Console.Write(Label);
        Console.SetCursorPosition(Console.WindowWidth/2, Y);
        int perc = Progress * 100 / Max;
        Console.Write(new string('█', perc/10).PadRight(10, '░'));
        Console.Write(" | " + (Number ? $"{Progress}/{Max}" : $"{perc}%"));
        return Progress < Max;
    }

    public void Sleep() {
        if (Wait == 0) return;
        for (int i = 0; i < Wait / 50; i++) {
            Console.SetCursorPosition(0, Y);
            Console.Write(Label);
            Console.SetCursorPosition(Console.WindowWidth/2, Y);
            Console.Write("░░░░░░░░░░ | ...");
            Console.SetCursorPosition(Console.WindowWidth/2 + i%10, Y);
            Console.Write("█");
            Thread.Sleep(50);
        }
    }
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}