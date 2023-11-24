using System.Text.Json;

string baseFolder = @"C:\Users\Vince\Desktop\ConquerOnline2\ConquerServer\bin\Debug\net6.0\database\raw\";

string CreateDirectory(string path)
{
    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
    return path;
}

List<Dictionary<string, string>>? ConvertCq(string[] columnNames, string fileName)
{
    Console.WriteLine("Converting {0} ...", fileName);
    string[] cqSql = File.ReadAllLines(Path.Join(baseFolder, fileName));

    string? valueLine = cqSql.FirstOrDefault(line => line.StartsWith("INSERT INTO"));
    if (valueLine == null)
    {
        Console.WriteLine("Could not find INSERT INTO line.");
        return null;
    }

    valueLine = valueLine.Substring(valueLine.IndexOf('('));
    valueLine = valueLine.Substring(0, valueLine.LastIndexOf(')') + 1);

    string[] data = valueLine
        .Split(",(")
        .Select(line => line.Trim(new char[] { '(', ')', ',' }))
        .ToArray();

    var map = new List<Dictionary<string, string>>();
    foreach (var line in data)
    {
        var parts = line.Split(',');
        var npc = new Dictionary<string, string>();
        for (int i = 0; i < parts.Length; i++)
        {
            npc.Add(columnNames[i], parts[i].Trim('\''));
        }
        map.Add(npc);
    }

    return map;
}

void ConvertCqMagic()
{
    string[] columnNames = new string[]
    {
        "id", "type", "sort", "name", "crime", "ground", "multi",
        "target", "level", "use_mp", "power", "intone_speed", "perent",
        "step_secs", "range", "distance", "status",
        "need_prof", "need_exp", "need_time", "need_level",
        "use_xp", "weapon_subtype", "active_times", "auto_active",
        "floor_attr", "autolearn", "learn_level", "drop_weapon",
        "use_ep", "weapon_hit", "use_item", "next_magic", "delay_ms",
        "use_item_num", "status_data0", "status_data1", "status_data2",
        "attr_type", "attr_power", "target_num", "next_ast_prof", "next_ast_prof_rank",
        "width", "data", "dur_time", "atk_interval", "coldtime", "req_uplevtime"
    };

    var magics = ConvertCq(columnNames, "cq_magictype.sql");

    string magicFolder = CreateDirectory(Path.Join(baseFolder, "cq_magic"));
    int converted = 0;

    foreach (var magic in magics)
    {
        Save(magic, $"{magic["type"]}-{magic["level"]}", magicFolder); 
        converted++;
    }

    Console.WriteLine("Converted {0} magics", converted);
}

void ConvertCqNpcSql()
{
    string[] columnNames = new string[]
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

    var npcs = ConvertCq(columnNames, "cq_npc.sql");
    if (npcs == null) return;

    int converted = 0;
    string npcFolder = CreateDirectory(Path.Join(baseFolder, "cq_npc"));

    foreach (var g in npcs.GroupBy(n => n["mapid"]))
    {
        // g.Key is the mapid
        string mapFolder = Path.Join(npcFolder, g.Key);
        CreateDirectory(mapFolder);

        foreach (var npc in g)
        {
            // convert to json
            Console.WriteLine("Created {0}", Save(npc, npc["id"], mapFolder));
            converted++;
        }
    }

    Console.WriteLine("Converted {0} npcs", converted);

}

string Save(Dictionary<string, string> map, string file, string baseFolder)
{
    var jsonOutput = JsonSerializer.Serialize(map, map.GetType(), new JsonSerializerOptions()
    {
        WriteIndented = true,
    });

    string path = Path.Join(baseFolder, $"{file}.json");
    File.WriteAllText(path, jsonOutput);
    return path;
}


ConvertCqMagic();
