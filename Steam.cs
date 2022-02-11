using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace SteamChecker
{
    internal class Steam
    {
        private readonly string SteamBanAPI = "https://api.steampowered.com/ISteamUser/GetPlayerBans/v1/?key=12A1D1DE83F9932934EDD6DF2BA00463&steamids=";
        private readonly string SteamLevelAPI = "http://api.steampowered.com/IPlayerService/GetSteamLevel/v1/?key=12A1D1DE83F9932934EDD6DF2BA00463&steamid=";
        private readonly string SteamNameAPI = "https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=12A1D1DE83F9932934EDD6DF2BA00463&steamids=";
        private readonly string SteamGameAPI = "https://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=12A1D1DE83F9932934EDD6DF2BA00463&steamid=";
        internal string level { get; set; }
        internal string name { get; set; }
        internal string gameCount { get; set; }
        internal string steam64ID_ { get; set; }
        internal int vacBanCount { get; set; }
        internal int gameBanCount { get; set; }
        internal bool communityBan { get; set; }
        internal bool tradeBan { get; set; }
        internal bool error { get; set; }
        internal bool online { get; set; }

        internal Steam(string steam64ID)
        {
            using (WebClient client = new WebClient()) //Check it's Steam46ID or normal ID
            {
                try
                {
                    string test = client.DownloadString(SteamLevelAPI + steam64ID);
                    client.Dispose();
                }
                catch (System.Net.WebException)
                {
                    string tempText_ = "";

                    string tempText = client.DownloadString("https://steamcommunity.com/id/" + steam64ID);

                    tempText = tempText.Substring(tempText.IndexOf("\"steamid\":\"") + "\"steamid\":".Length + 1);

                    foreach (char letter in tempText)
                    {
                        if (letter != '\"')
                        {
                            tempText_ = tempText_ + letter;
                        }
                        else
                        {
                            tempText = tempText_;
                            tempText_ = "";
                            break;
                        }
                    }

                    steam64ID = tempText;

                    try
                    {
                        string test = client.DownloadString(SteamLevelAPI + steam64ID);
                        client.Dispose();
                    }
                    catch
                    {
                        error = true;
                        return;
                    }
                }
            }

            using (WebClient client = new WebClient())
            {
                string banData = client.DownloadString(SteamBanAPI + steam64ID);
                string levelData = client.DownloadString(SteamLevelAPI + steam64ID);
                string nameData = client.DownloadString(SteamNameAPI + steam64ID);
                string gameData = client.DownloadString(SteamGameAPI + steam64ID);
                string tempText = "";

                if (banData.Substring(banData.IndexOf("\"CommunityBanned\":") + "\"CommunityBanned\":".Length, 5) == "false")
                {
                    communityBan = false;
                }
                else
                {
                    communityBan = true;
                }

                if (banData.Substring(banData.IndexOf("\"EconomyBan\":") + "\"EconomyBan\":".Length, 6) == "\"none\"")
                {
                    tradeBan = false;
                }
                else
                {
                    communityBan = true;
                }

                vacBanCount = Convert.ToInt32(banData.Substring(banData.IndexOf("\"NumberOfVACBans\":") + "\"NumberOfVACBans\":".Length, 1));

                gameBanCount = Convert.ToInt32(banData.Substring(banData.IndexOf("\"NumberOfGameBans\":") + "\"NumberOfGameBans\":".Length, 1));

                level = levelData.Substring(levelData.IndexOf("\"player_level\":") + "\"player_level\":".Length);

                foreach (char letter in level.ToString())
                {
                    if (letter != '}')
                    {
                        tempText = tempText + letter;
                    }
                    else
                    {
                        level = tempText;
                        tempText = "";
                        break;
                    }
                }

                name = nameData.Substring(nameData.IndexOf("\"personaname\":\"") + "\"personaname\":\"".Length);

                foreach (char letter in name)
                {
                    if (letter != '\"')
                    {
                        tempText = tempText + letter;
                    }
                    else
                    {
                        name = tempText;
                        tempText = "";
                        break;
                    }
                }

                gameCount = gameData.Substring(gameData.IndexOf("\"game_count\":") + "\"game_count\":".Length);

                foreach (char letter in gameCount)
                {
                    if (letter != ',')
                    {
                        tempText = tempText + letter;
                    }
                    else
                    {
                        gameCount = tempText;
                        tempText = "";
                        break;
                    }
                }

                steam64ID_ = steam64ID;

                error = false;

                try
                {
                    Task<bool> task = Twitch.IsOnline(name);
                    task.Wait();
                    online = task.Result;
                }
                catch (Exception)
                {
                    online = false;
                }

                client.Dispose();
                ;
            }
        }
    }

    internal static class Engine
    {
        internal static void ShowInformation(Steam account)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            if (account.error == true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error->{account.steam64ID_}]");
                return;
            }

            if (account.communityBan == true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Community Banned->{account.steam64ID_}]");
                return;
            }

            Console.WriteLine($"[Account Name : {account.name}]");

            Console.WriteLine($"[SteamID64: { account.steam64ID_}]");

            Console.WriteLine($"[Level: { account.level}]");

            Console.WriteLine($"[Game Count : {account.gameCount}]");

            if (account.gameBanCount >= 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Game Ban Count : {account.gameBanCount}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Game Ban Count : {account.gameBanCount}]");
                Console.ForegroundColor = ConsoleColor.Green;
            }

            if (account.vacBanCount >= 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Vac Ban Count : {account.vacBanCount}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Vac Ban Count : {account.vacBanCount}]");
                Console.ForegroundColor = ConsoleColor.Green;
            }

            if (account.tradeBan == true)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"[Trade Banned : {account.tradeBan}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Trade Banned : {account.tradeBan}]");
                Console.ForegroundColor = ConsoleColor.Green;
            }

            if (account.online == true)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"[Live On Twitch : {account.online}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"[Live On Twitch : {account.online}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }


    }

}