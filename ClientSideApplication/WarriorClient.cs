using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Script.Serialization;
using ClientSideApplication.Tools;
using log4net;
using RestSharp;

namespace ClientSideApplication
{
    public class WarriorClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WarriorClient));

        private static string DoAction(string action, WarriorBrain.Strength strength, int time)
        {
            var client = new RestClient(ConfigSettings.ReadSetting("Url"));
            var request = new RestRequest(action + "/{RoomGUID}/{MyGUID}/{strength}", Method.POST);

            string info;
            if (strength.Equals(WarriorBrain.Strength.None)) info = time.ToString();
            else info = strength.ToString();

            request.AddUrlSegment("RoomGUID", WarriorBrain.RoomGuid);
            request.AddUrlSegment("MyGUID", ConfigSettings.ReadSetting("MyGUID"));
            request.AddUrlSegment("strength", info);
            try
            {
                IRestResponse response = client.Execute(request);

                var content = response.Content;

                return content;
            }
            catch (Exception)
            {
                Logger.Info("Unable to connect to server!");
            }
            return "0";
        }

        public string HostGame()
        {
            while (true)
            {
                try
                {
                    var client = new RestClient(ConfigSettings.ReadSetting("Url"));
                    var request = new RestRequest("HostGame/{MyGUID}", Method.POST);

                    request.AddUrlSegment("MyGUID", ConfigSettings.ReadSetting("MyGUID"));

                    IRestResponse response = client.Execute(request);

                    var content = response.Content;

                    content = content.Substring(1, content.Length - 2);
                    Logger.Info("Game room Guid is:\n" + content);
                    return content;

                }
                catch (Exception)
                {
                    Logger.Info("Unable to Host game, server is unavailable");
                }
            }
        }

        public void JoinGame(string roomGuid)
        {
            while (true)
            {
                try
                {
                    var client = new RestClient(ConfigSettings.ReadSetting("Url"));
                    var request = new RestRequest("JoinGame/{RoomGUID}/{MyGUID}", Method.POST);

                    request.AddUrlSegment("MyGUID", ConfigSettings.ReadSetting("MyGUID"));
                    request.AddUrlSegment("RoomGUID", roomGuid);

                    IRestResponse response = client.Execute(request);

                    var content = response.Content;

                    content = content.Substring(1, content.Length - 2);

                    Logger.Info(content);
                    return;
                }
                catch (Exception)
                {
                    Logger.Info("Unable to Join game, server is unavailable");

                }
            }
        }

        public void Attack(WarriorBrain.Strength strength)
        {
            Logger.Info("You are trying to deal " + strength + " atack!");
            int damage = Int32.Parse(DoAction("attack", strength, 0));

            switch (damage)
            {
                case -3:
                    Logger.Info("Other player has not joined yet");
                    break;
                case -2:
                    Logger.Info("You were trying to attack, but you were interrupted");
                    break;
                case -1:
                    Logger.Info("Invalid time!");
                    break;
                case 0:
                    Logger.Info("Enemy didn`t lose any health because he was defending!");
                    break;
                default:
                    Logger.InfoFormat("Enemy was damaged for {0} hitpoints", damage);
                    break;
            }
        }

        public void Check()
        {
            Logger.Info("You are checking the State of your enemy");

            string state = DoAction("check", WarriorBrain.Strength.None, 0);

            Logger.InfoFormat("The State of your enemy is: {0}", state);
            Logger.Info("My current life is: " + WarriorBrain.MyInfo.Life + " State is: " + WarriorBrain.MyInfo.State);
        }

        public void Defend(int time)
        {
            Logger.Info("You are defending for " + time + "s.");

            int respond = Int32.Parse(DoAction("defend", WarriorBrain.Strength.None, time));

            if (respond == -1)
                Logger.Info("Invalid time!");
            if (respond == -3)
                Logger.Info("Other user not ready!");
        }

        public void Rest(int time)
        {
            Logger.Info("You are resting for " + time + "s");

            int healPoints = Int32.Parse(DoAction("rest", WarriorBrain.Strength.None, time));

            switch (healPoints)
            {
                case -2:
                    Logger.Info("Other user has not joined yet!");
                    break;
                case -1:
                    Logger.Info("Invalid time!");
                    break;
                case 0:
                    Logger.Info("You were trying to rest, but you were interrupted");
                    break;
                default:
                    Logger.Info("You have been healed by " + healPoints + " points");
                    break;
            }
        }

        public static bool IsBattleOver()
        {
            var request = (HttpWebRequest)WebRequest.Create(ConfigSettings.ReadSetting("Url") + "IsAlive/" + WarriorBrain.RoomGuid);

            var response = (HttpWebResponse)request.GetResponse();

            // ReSharper disable once AssignNullToNotNullAttribute
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString.Equals("true");
        }

        public void GetMyInfo()
        {
            while (true)
            {
                var client = new RestClient(ConfigSettings.ReadSetting("Url"));
                var request = new RestRequest("MyStats/{RoomGuid}/{MyGUID}", Method.GET);

                request.AddUrlSegment("RoomGuid", WarriorBrain.RoomGuid);
                request.AddUrlSegment("MyGUID", ConfigSettings.ReadSetting("MyGUID"));
                try
                {
                    IRestResponse response = client.Execute(request);

                    var content = response.Content;

                    WarriorBrain.MyInfo = new JavaScriptSerializer().Deserialize<WarriorState>(content);

                    Thread.Sleep(25);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public static bool CheckForWinner()
        {
            var request = (HttpWebRequest)WebRequest.Create(ConfigSettings.ReadSetting("Url") + "Winner/" + WarriorBrain.RoomGuid + "/" + ConfigSettings.ReadSetting("MyGUID"));

            var response = (HttpWebResponse)request.GetResponse();

            // ReSharper disable once AssignNullToNotNullAttribute
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            responseString = responseString.Substring(1, responseString.Length - 2);

            Logger.Info(responseString);
            return responseString == "NOT WINNER";
        }

        public bool AreBothUsersOnline()
        {
            try
            {
                var client = new RestClient(ConfigSettings.ReadSetting("Url"));
                var request = new RestRequest("UsersReady/{RoomGuid}", Method.POST);

                request.AddUrlSegment("RoomGuid", WarriorBrain.RoomGuid);

                IRestResponse response = client.Execute(request);

                var content = response.Content;

                if(content == "false")
                    return false;
                return true;

            }
            catch (Exception)
            {
                Logger.Info("Unable to Host game, server is unavailable");
            }
            return false;
        }
    }
}