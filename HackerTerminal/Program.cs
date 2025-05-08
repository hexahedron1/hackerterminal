if (!Directory.Exists($"/home/{Environment.UserName}/.local/share/hackerterminal"))
    Directory.CreateDirectory($"/home/{Environment.UserName}/.local/share/hackerterminal");
if (!File.Exists($"/home/{Environment.UserName}/.local/share/hackerterminal/messages.json")) {
    using var client = new HttpClient();
    using var s = client.GetStreamAsync("https://raw.githubusercontent.com/hexahedron1/hackerterminal/refs/heads/master/HackerTerminal/messages.json");
    using var fs = new FileStream($"/home/{Environment.UserName}/.local/share/hackerterminal/messages.json", FileMode.OpenOrCreate);
    s.Result.CopyTo(fs);
}