using ConquerServer.Database.Models;
using ConquerServer.Client;
using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.Buffers;
using System.Xml.Linq;

namespace ConquerServer.Database
{
    public class Db
    {

        private static Dictionary<int, AuthClient> _authClients;
        private static int _characterCounter;
        private static int _itemCounter;
        private static string _serverHost;
        private static List<PortalModel> _portals;
        private static Dictionary<int, ItemTypeModel> _itemTypes;
        private static Dictionary<int, MagicTypeModel> _magicTypes;
        private static DbCore _core;

        private static Thread _worker;
        private static Queue<Action> _workQueue;

        private static string GetAccountFilePath(string username) => $"./database/Account/{username}.json";
        private static DbFile GetStatFile(Profession profession) => _core.SelectFile("Misc", $"{profession}.ini");
        private static string GetConfigFilePath() => $"./database/config.json";
        private static string GetPortalFilePath() => $"./database/Misc/Portals.txt";
        private static string GetItemTypeFilePath() => $"./database/Item/itemtype.txt";
        private static string GetMagicTypeFilePath() => $"./database/Misc/magictype.txt";

        static Db()
        {
            _core = new DbCore("./database/");
            _authClients = new Dictionary<int, AuthClient>();
            _portals = new List<PortalModel>();
            _itemTypes = new Dictionary<int, ItemTypeModel>();
            _magicTypes = new Dictionary<int, MagicTypeModel>();
            _workQueue = new Queue<Action>();
            _worker = new Thread(ThreadTaskProcess);
        }

        private static void ThreadTaskProcess()
        {
            for (; ;)
            {
                if (_workQueue.Count > 0)
                {
                    var work = _workQueue.Dequeue();
                    work();
                }
                Thread.Sleep(1);
            }
        }

        private static Task<T> CreateTask<T>(Func<T> work)
        {
            var promise = new TaskCompletionSource<T>();
            _workQueue.Enqueue(() =>
            {
                try
                {
                    var result = work();
                    promise.TrySetResult(result);

                }
                catch (Exception ex)
                {
                    promise.TrySetException(ex);
                }
            });
            return promise.Task;
        }

        private static Task CreateTask(Action work)
        {
            // wrap task inside of a Func<T> to call the other CreateTask method
            return CreateTask(() =>
            {
                work();
                return 0;
            });
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            options.Converters.Add(new Lookface());
            return options;
        }

        public static string Serialize<T>(T model)
        {
            string serialized = JsonSerializer.Serialize(model, GetJsonSerializerOptions());
            return serialized;
        }

        public static T Deserialize<T>(string json)
        {
            //takes the string(storing all of the text within a file) and reconstructs the object
            T? obj = JsonSerializer.Deserialize<T>(json, GetJsonSerializerOptions());
            if (obj == null)
                throw new ArgumentException("Argument provided when deserialized is a null object", nameof(json));
            return obj;
        }

        public static void Load()
        {
            // get file path
            string path = GetConfigFilePath();
            DatabaseConfig databaseConfig = new DatabaseConfig();

            // if the file exists, then read the file
            if (File.Exists(path))
            {
                string config = File.ReadAllText(path);
                databaseConfig = Deserialize<DatabaseConfig>(config);
            }
            else
            {
                // file doesn't exist, so create it with defaults
                File.WriteAllText(path, Serialize(databaseConfig));
            }


            // set internal variables
            _characterCounter = databaseConfig.CharacterCounter;
            _serverHost = databaseConfig.ServerHost;
            _itemCounter = databaseConfig.ItemCounter;


            LoadPortals();

            _worker.Start();

            Console.WriteLine("\tLoaded {0} portals", _portals.Count);

            int failedItems = LoadItems();
            Console.WriteLine("\tLoaded {0} items, {1} invalid", _itemTypes.Count, failedItems);

            int failedMagic = LoadMagic();
            Console.WriteLine("\tLoaded {0} magic, {1} invalid", _magicTypes.Count, failedMagic);

        }
        private static void LoadPortals()
        {
            //read portals.txt as lines
            string[] portalText = File.ReadAllLines(GetPortalFilePath());
            //split lines and assign values as needed
            foreach (string line in portalText)
            {
                _portals.Add(PortalModel.Parse(line));
            }
        }
        private static int LoadItems()
        {
            ItemTypeModel? itemType;
            string[] itemText = File.ReadAllLines(GetItemTypeFilePath());

            int f = 0;
            foreach (string line in itemText)
            {
                try
                {
                    itemType = ItemTypeModel.Parse(line);
                    _itemTypes.Add(itemType.TypeId, itemType);
                }
                catch
                {
                    f++;
                }
            }
            return f;
        }
        private static int LoadMagic()
        {
            MagicTypeModel? magicType;
            string[] magicText = File.ReadAllLines(GetMagicTypeFilePath());

            int failed = 0;
            foreach (string line in magicText)
            {
                try
                {
                    magicType = MagicTypeModel.Parse(line);
                    _magicTypes.Add(magicType.CompositeKey, magicType);
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(line);
                    Console.WriteLine(ex.ToString());
                    failed++;
                }
            }

            return failed;
        }

        public static void SaveConfig()
        {
            DatabaseConfig databaseConfig = new DatabaseConfig()
            {
                ServerHost = _serverHost,
                CharacterCounter = _characterCounter,
                ItemCounter = _itemCounter,
            };
            string serialize = Serialize<DatabaseConfig>(databaseConfig);
            File.WriteAllText(GetConfigFilePath(), serialize);
        }

        public static ItemTypeModel GetItemTypeByTypeId(int id)
        {
            ItemTypeModel model;


            if (!_itemTypes.TryGetValue(id, out model))
                throw new DbException($"Item {id} does not exist within the database");
            return model;
        }
        public static MagicTypeModel GetMagicType(int type, int level)
        {
            MagicTypeModel model;
            int key = MagicTypeModel.CreateCompositeKey(type, level);

            if (!_magicTypes.TryGetValue(key, out model))
                throw new DbException($"Magic {type}-{level} does not exist within the database");
            return model;
        }

        public Dictionary<int, AuthClient> Auth { get { return _authClients; } }
        public IReadOnlyList<PortalModel> Portals { get { return _portals; } }
        public IReadOnlyDictionary<int, ItemTypeModel> ItemTypes { get { return _itemTypes; } }
        public string ServerHost { get { return _serverHost; } }
        public int CharacterCounter
        {
            get { return _characterCounter; }
            private set
            {
                _characterCounter = value;
                SaveConfig();
            }
        }
        public int ItemCounter
        {
            get
            {
                return _itemCounter;
            }
            set
            {
                _itemCounter = value;
                SaveConfig();
            }
        }

        public GameClient Owner { get; private set; }

        public Db(GameClient? owner)
        {
            Owner = owner;
        }


        public bool HasCharacter(string? username = null)
        {
            // Action: check if file exists
            username = username ?? Owner.Username;
            if (File.Exists(GetAccountFilePath(username)))
            {
                return true;
            }
            return false;
        }

        public Task<CharacterModel> LoadCharacter(string? username = null)
        {
            return CreateTask(() =>
            {
                username = username ?? Owner.Username;
                if (!HasCharacter(username))
                    throw new DbException("Character does not exist.");

                string charText = File.ReadAllText(GetAccountFilePath(username));
                var regenChar = Deserialize<CharacterModel>(charText);
                return regenChar;
            });
        }

        public Task CreateCharacter(string username, string name, uint lookface, int job)
        {
            return CreateTask(() =>
            {
                //throw error is char already exists
                if (HasCharacter(username))
                    throw new DbException("This account already has a character");

                //when there is not a character
                //goal: create a file
                //need: character model to serialize
                //      data for character model received by game client
                //      BEFORE this method is called
                //      receive and store that data


                CharacterModel model = new CharacterModel()
                {
                    Name = name,
                    Lookface = (Lookface)lookface,
                    Job = job,
                    Id = CharacterCounter++,
                    Level = 1,
                    MapId = 1005,
                    X = 50,
                    Y = 50,
                };
                string serialize = Serialize(model);
                File.WriteAllText(GetAccountFilePath(username), serialize);
            });
        }

        public Item CreateItem(int typeId)
        {
            /*
             * goal: create an item to be sent to player via slashcommand
             * need: each item to have its OWN id and 
             *      appropriate typeid, pulling all attributes via ITEMTYPEMODEL
             */
            Item generatedItem = new Item()
            {
                Owner = this.Owner,
                Id = ItemCounter++,
                TypeId = typeId,
                Position = ItemPosition.Inventory,
                Color = 3
            };
            generatedItem.Durability = generatedItem.Attributes.Durabiliy;
            generatedItem.MaxDurability = generatedItem.Attributes.Durabiliy;
            return generatedItem;
        }

        public StatModel GetStats(Profession profession, int level)
        {
            //unsure what return type is

            //pull from .ini here? 
            //gotta determine which .ini by using profession 
            var statFile = GetStatFile(profession);

            //pull by searching...job and level
            string slevel = level.ToString();
            int str = statFile.ReadInt32(slevel, "Strength", 0);
            int vit = statFile.ReadInt32(slevel, "Vitality", 0);
            int agi = statFile.ReadInt32(slevel, "Agility", 0);
            int spi = statFile.ReadInt32(slevel, "Spirit", 0);

            //when unable to pull, values = 0 by default
            StatModel stats = new StatModel(str, vit, agi, spi);

            return stats;
        }

        public Task SaveCharacter(GameClient? client = null)
        {
            return CreateTask(() =>
            {
                client = client ?? Owner;

                CharacterModel charModel = new CharacterModel(client);
                string serialize = Serialize(charModel);
                File.WriteAllText(GetAccountFilePath(client.Username), serialize);
            });
        }
    }
}