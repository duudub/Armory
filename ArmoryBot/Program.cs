using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DiscordSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;
using BattleDotNet;
using System.Net;

namespace ArmoryBot
{

    class Program
    {

        public static string className = "class";
        public static string discordToken = "";
        public static string bpublicKey = "";
        public static string bprivateKey = "";

        static void Main(string[] args)
        {
            CharData charData = new CharData();
            getTokens();




            DiscordClient client = new DiscordClient(discordToken, true, true);
            WoWClient wow = new WoWClient(publicKey: bpublicKey, privateKey: bprivateKey);
            Console.WriteLine("private: " + bprivateKey + "\n" + "public: " + bpublicKey);

            client.MessageReceived += (sender, e) =>
            {
                if (!e.Message.Author.ID.Equals("180563370636738561"))
                {
                    if (e.MessageText.Contains("!find"))
                    {

                        object charname = "";
                        object realmname = "";
                        try
                        {
                            charname = e.MessageText.Substring(6, e.MessageText.LastIndexOf("@") - 6);
                            realmname = e.MessageText.Substring(e.MessageText.LastIndexOf("@") + 1);
                        }
                        catch
                        {
                            e.Channel.SendMessage("invalid command");
                        }
                        WebRequest request = WebRequest.Create("https://us.api.battle.net/wow/character/" + realmname + "/" + charname + "?fields=items,guild&locale=en_US&apikey=be88k97qvzxjwq8q6frxmzccucwajudz");


                        WebResponse response = null;

                        try
                        {
                            response = request.GetResponse();
                        }
                        catch (WebException ex)
                        {
                            e.Channel.SendMessage("Oops! Looks like the realm name or character is wrong! If not the api may have bugged out for a sec try again in a bit.");

                            Console.WriteLine("Error: " + ex);
                        }


                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();


                        dynamic data = JObject.Parse(responseFromServer);
                        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                        string jc = JsonConvert.SerializeObject(json);
                        reader.Close();

                        className = "class";

                        string GetClassNames = data.@class;
                        getClass();
                        GetClass(GetClassNames.ToString());
                        dynamic o = JsonConvert.DeserializeObject(json);
                        string ItemLevel = (string)data["items"]["averageItemLevel"];




                        charData.name = data.name;
                        charData.level = data.level;
                        charData.achievementPoints = data.achievementPoints;
                        charData.classType = data.@class;
                        charData.className = GetClass(GetClassNames);
                        charData.avgItemLevel = ItemLevel;

                        string guildName = "No Guild";

                        try
                        {
                            guildName = (string)data["guild"]["name"];
                        }
                        catch
                        {
                            charData.guildName = "Not in a guild";
                        }

                        e.Channel.SendMessage("```Name: " + charData.name + "\nGuild: " + "<" + guildName + ">" + "\nLevel: " + charData.level + "\nAchievement Points: " + charData.achievementPoints + "\nClass: " + charData.className + "\nAverage Item Level: " + charData.avgItemLevel + "```");




                    }
                }

            };

            client.PrivateMessageReceived += (sender, e) =>
            {


            };

            try
            {
                client.SendLoginRequest();
                client.Connect();
            }
            catch (Exception e)
            {

                Console.WriteLine("Failed to connect" + e.Message);

            }

            client.Connected += (sender, e) =>
            {
                // Console.ForegroundColor = ConsoleColor.Cyan;
                List<DiscordSharp.Objects.DiscordServer> servers = client.GetServersList();
                Console.WriteLine("Connected to server as " + e.User.Username);
                client.UpdateCurrentGame("Working as intended!", true, "jorbo.github.io");
            };

            Console.ReadKey();
            Environment.Exit(0);


        }

        public class CharData
        {
            public string name { get; set; }
            public string level { get; set; }
            public string classType { get; set; }
            public string faction { get; set; }
            public string className { get; set; }
            public string achievementPoints { get; set; }
            public string avgItemLevel { get; set; }
            public string guildName { get; set; }
        }

        public static string GetClass(string classType)
        {

            CharData charData = new CharData();

            if (classType.Equals("1"))
            {
                className = "Warrior";
                return className;
            }
            else if (classType.Equals("2"))
            {
                className = "Paladin";
                return className;
            }
            else if (classType.Equals("3"))
            {
                className = "Hunter";
                return className;
            }
            else if (classType.Equals("4"))
            {
                className = "Rogue";
                return className;
            }
            else if (classType.Equals("5"))
            {
                className = "Priest";
                return className;
            }
            else if (classType.Equals("7"))
            {
                className = "Shaman";
                return className;
            }
            else if (classType.Equals("8"))
            {
                className = "Mage";
                return className;
            }
            else if (classType.Equals("9"))
            {
                className = "Warlock";
                return className;
            }
            else if (classType.Equals("10"))
            {
                className = "Monk";
                return className;
            }
            else if (classType.Equals("11"))
            {
                className = "Druid";
                return className;
            }
            else if (classType.Equals("6"))
            {
                className = "Death Knight";
                return className;
            }
            else
            {
                return "ERROR";
            }

        }

        public static string GetFaction(string faction)
        {
            CharData charData = new CharData();
            if (faction.Equals(0))
            {
                charData.faction = "Alliance";
                return faction;
            }
            else
            {
                charData.faction = "Horde";
                return faction;
            }

        }

        public static void getClass()
        {
            using (StringReader sr = new StringReader(className))
            {
                for (int i = 0; i < 6; i++)
                {
                    className = sr.ReadLine();

                }
            }

        }

        public static void getTokens()
        {
            using (StreamReader sr = new StreamReader(@"../../../tokens.ini"))
            {
                for (int i = 0; i < 6; i++)
                {
                    string line = sr.ReadLine();
                    if (i == 1)
                    {
                        discordToken = line.Substring(6);
                    }
                    if (i == 3)
                    {
                        bprivateKey = line.Substring(7);
                    }
                    if (i == 4)
                    {
                        bpublicKey = line.Substring(8);
                    }
                }
            }
        }
    }
}