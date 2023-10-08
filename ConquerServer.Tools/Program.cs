using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

static void CreateDirectory(string path)
{
    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
}

static void ConvertCqNpcSql()
{
    Console.WriteLine("Converting cq_npc.sql ...");

    string[] columNames = new string[]
    {
        "id", "ownerid", "playerid",
        "name", "type", "lookface",
        "idxserver", "mapid", "cellx", "celly",
        "task0", "task1", "task2", "task3",
        "task4", "task5", "task6", "task7",
        "data0", "data1", "data2", "data3", "datastr",
        "linkid",
        "life", "maxlife",
        "base",
        "sort",
        "itemid"
    };

    string baseFolder = @"C:\Users\Vince\Desktop\ConquerOnline2\ConquerServer\bin\Debug\net6.0\database\raw\";
    string[] cq_npc = File.ReadAllLines(Path.Join(baseFolder, "cq_npc.sql"));

    string? valueLine = cq_npc.FirstOrDefault(line => line.StartsWith("INSERT INTO `cq_npc` VALUES "));
    if (valueLine == null)
    {
        Console.WriteLine("Could not find value line in cq_npc.sql");
        return;
    }

    valueLine = valueLine.Substring(valueLine.IndexOf('('));
    valueLine = valueLine.Substring(0, valueLine.LastIndexOf(')')+1);

    string[] data = valueLine
        .Split(",(")
        .Select(line => line.Trim(new char[] { '(', ')', ',' }))
        .ToArray();

    var npcs = new List<Dictionary<string, string>>();

    foreach (var line in data)
    {
        var parts = line.Split(',');
        var npc = new Dictionary<string, string>();
        for (int i = 0; i < parts.Length; i++)
        {
            npc.Add(columNames[i], parts[i].Trim('\''));
        }
        npcs.Add(npc);
    }

    int converted = 0;
    string npcFolder = Path.Join(baseFolder, "cq_npc");
    CreateDirectory(npcFolder);

    foreach (var g in npcs.GroupBy(n => n["mapid"]))
    {
        // g.Key is the mapid
        string mapFolder = Path.Join(npcFolder, g.Key);
        CreateDirectory(mapFolder);

        foreach (var npc in g)
        {
            // convert to json
            var jsonOutput = JsonSerializer.Serialize(npc, npc.GetType(), new JsonSerializerOptions()
            {
                 WriteIndented = true,
            });

            string path = Path.Join(mapFolder, $"{npc["id"]}.json");
            File.WriteAllText(path, jsonOutput);
            Console.WriteLine("Created {0}", path);

            converted++;
        }
    }

    Console.WriteLine("Converted {0} npcs", converted);

}


ConvertCqNpcSql();
