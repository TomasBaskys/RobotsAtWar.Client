using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using ClientSideApplication.Enums;
using ClientSideApplication.Tools;
using log4net;

namespace ClientSideApplication
{
    public class WarriorClient
    {
        private static ILog Logger;
        private readonly HttpClient _client;
        private readonly string _myGuid;

        public WarriorClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(ConfigSettings.ReadSetting("Url"));
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Logger = LogManager.GetLogger(typeof(WarriorClient));
            _myGuid = ConfigSettings.ReadSetting("MyGUID");
        }

        private Response DoAction(string action, Strength strength, int time)
        {
            try
            {
                string info;
                if (strength.Equals(Strength.None))
                    info = time.ToString();
                else
                    info = strength.ToString();

                HttpResponseMessage responseMessage = _client.PostAsJsonAsync(action + "/" + WarriorBrain.RoomGuid + "/" + _myGuid + "/" + info, "aaa").Result;
                responseMessage.EnsureSuccessStatusCode();

                string content = responseMessage.Content.ReadAsAsync<string>().Result;
                return StringToResponse(content);
            }
            catch (Exception exception)
            {
                Logger.Info("Error DoAction! " + exception.Message);
            }
            return Response.WrongData;
        }

        private Response StringToResponse(string value)
        {
            switch (value)
            {
                case "0":
                    return Response.Success;
                case "1":
                    return Response.Interrupted;
                case "2":
                    return Response.WrongData;
                case "3":
                    return Response.BattleNotStarted;
                case "4":
                    return Response.DefendingHeavyShield;
                case "5":
                    return Response.DefendingNormalShield;
                case "6":
                    return Response.DefendingLightShield;
            }
            return Response.WrongData;
        }

        public string HostGame()
        {
            while (true)
            {
                try
                {
                    HttpResponseMessage responseMessage = _client.PostAsJsonAsync("HostGame/" + _myGuid, "aaa").Result;
                    responseMessage.EnsureSuccessStatusCode();

                    string content = responseMessage.Content.ReadAsAsync<string>().Result;
                    //content = content.Substring(1, content.Length - 2);

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
                    HttpResponseMessage responseMessage = _client.PostAsJsonAsync("JoinGame/" + roomGuid + "/" + _myGuid, "aaa").Result; 
                    responseMessage.EnsureSuccessStatusCode();

                    string content = responseMessage.Content.ReadAsAsync<string>().Result;
                    Logger.Info(content);
                    return;
                }
                catch (Exception)
                {
                    Logger.Info("Unable to Join game, server is unavailable");
                }
            }
        }

        public Response Attack(Strength strength)
        {
            Logger.Info("You are trying to deal " + strength + " atack!");
            Response attackResponse = DoAction("attack", strength, 0);

            switch (attackResponse)
            {
                case Response.BattleNotStarted:
                    Logger.Info("Other player has not joined yet");
                    break;
                case Response.Interrupted:
                    Logger.Info("You were trying to attack, but you were interrupted");
                    break;
                case Response.WrongData:
                    Logger.Info("Invalid time!");
                    break;
                case Response.DefendingHeavyShield:
                    Logger.Info("Enemy didn`t lose any health because he was defending with heavy shield!");
                    break;
                case Response.DefendingNormalShield:
                    Logger.Info("Enemy didn`t lose any health because he was defending with normal shield!");
                    break;
                case Response.DefendingLightShield:
                    Logger.Info("Enemy didn`t lose any health because he was defending with light shield!");
                    break;
                case Response.Success:
                    Logger.InfoFormat("I have succesfully dealt {0} attack", strength);
                    break;
            }
            return attackResponse;
        }

        public WarriorState Check()
        {
            Logger.Info("You are checking the State of your enemy");

            try
            {
                HttpResponseMessage responseMessage = _client.PostAsJsonAsync("Check/" + WarriorBrain.RoomGuid + "/" + _myGuid, "aaa").Result;
                responseMessage.EnsureSuccessStatusCode();

                WarriorState enemyState = responseMessage.Content.ReadAsAsync<WarriorState>().Result;
                Logger.Info("My current life is: " + WarriorBrain.MyInfo.Life + " State is: " + WarriorBrain.MyInfo.State);
                Logger.Info("Enemy current life is: " + enemyState.Life + " State is: " + enemyState.State);

                return enemyState;
            }
            catch (Exception exception)
            {
                Logger.Info("Error Check! " + exception.Message);
            }
            return null;
        }

        public Response Defend(int time)
        {
            Logger.Info("You are defending for " + time + "s.");

            Response response = DoAction("defend", Strength.None, time);

            switch (response)
            {
                case Response.WrongData:
                    Logger.Info("Invalid time!");
                    break;
                case Response.BattleNotStarted:
                    Logger.Info("Other user not ready!");
                    break;
            }
            return response;
        }

        public Response Rest(int time)
        {
            Logger.Info("You are resting for " + time + "s");

            Response response = DoAction("rest", Strength.None, time);

            switch (response)
            {
                case Response.BattleNotStarted:
                    Logger.Info("Other user has not joined yet!");
                    break;
                case Response.WrongData:
                    Logger.Info("Invalid time!");
                    break;
                case Response.Interrupted:
                    Logger.Info("You were trying to rest, but you were interrupted");
                    break;
                case Response.Success:
                    Logger.Info("You have successfully healed");
                    break;
            }
            return response;
        }

        public bool IsBattleOver()
        {
            try
            {
                HttpResponseMessage responseMessage = _client.GetAsync("IsAlive/" + WarriorBrain.RoomGuid).Result;
                responseMessage.EnsureSuccessStatusCode();

                bool content = responseMessage.Content.ReadAsAsync<bool>().Result;
                //return content.Equals("true");
                return content;
            }
            catch (Exception exception)
            {
                Logger.Info("Error IsBattleOver! " + exception.Message);
            }
            return false;
        }

        public void GetMyInfo()
        {
            while (true)
            {
                try
                {
                    HttpResponseMessage responseMessage = _client.GetAsync("MyStats/" + WarriorBrain.RoomGuid + "/" + _myGuid).Result;
                    responseMessage.EnsureSuccessStatusCode();

                    WarriorBrain.MyInfo = responseMessage.Content.ReadAsAsync<WarriorState>().Result;
                    Thread.Sleep(25);
                }
                catch (Exception exception)
                {
                    Logger.Info("Error GetMyInfo! " + exception.Message);
                }
            }
        }

        public bool CheckForWinner()
        {
            try
            {
                HttpResponseMessage responseMessage = _client.GetAsync("Winner/" + WarriorBrain.RoomGuid + "/" + _myGuid).Result;
                responseMessage.EnsureSuccessStatusCode();

                string content = responseMessage.Content.ReadAsAsync<string>().Result;
                Logger.Info(content);
                return content == "NOT WINNER";
            }
            catch (Exception exception)
            {
                Logger.Info("Error CheckForWinner! " + exception.Message);
            }
            return false;
        }

        public bool AreBothUsersOnline()
        {
            try
            {
                HttpResponseMessage responseMessage = _client.PostAsJsonAsync("UsersReady/" + WarriorBrain.RoomGuid, "aaa").Result;
                responseMessage.EnsureSuccessStatusCode();

                bool content = responseMessage.Content.ReadAsAsync<bool>().Result;

                if (content)
                    return true;
            }
            catch (Exception)
            {
                Logger.Info("Unable to Host game, server is unavailable");
            }
            return false;
        }

        /*
        private Response DoAction(string action, Strength strength, int time)
        {
            var client = new RestClient(ConfigSettings.ReadSetting("Url"));
            var request = new RestRequest(action + "/{RoomGUID}/{MyGUID}/{strength}", Method.POST);

            string info;
            if (strength.Equals(Strength.None)) info = time.ToString();
            else info = strength.ToString();

            request.AddUrlSegment("RoomGUID", WarriorBrain.RoomGuid);
            request.AddUrlSegment("MyGUID", ConfigSettings.ReadSetting("MyGUID"));
            request.AddUrlSegment("strength", info);
            try
            {
                IRestResponse response = client.Execute(request);

                var content = response.Content;

                return StringToResponse(content);
            }
            catch (Exception)
            {
                Logger.Info("Unable to connect to server!");
            }
            return Response.WrongData;
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

        public WarriorState Check()
        {
            Logger.Info("You are checking the State of your enemy");

            var client = new RestClient(ConfigSettings.ReadSetting("Url"));
            var request = new RestRequest("check" + "/{RoomGUID}/{MyGUID}", Method.POST);

            request.AddUrlSegment("RoomGUID", WarriorBrain.RoomGuid);
            request.AddUrlSegment("MyGUID", ConfigSettings.ReadSetting("MyGUID"));
            try
            {
                IRestResponse response = client.Execute(request);

                var content = response.Content;

                WarriorState enemyInfo = new JavaScriptSerializer().Deserialize<WarriorState>(content);
                Logger.Info("My current life is: " + WarriorBrain.MyInfo.Life + " State is: " + WarriorBrain.MyInfo.State);
                Logger.Info("Enemy current life is: " + enemyInfo.Life + " State is: " + enemyInfo.State);

                return enemyInfo;
            }
            catch (Exception)
            {
                Logger.Info("Unable to connect to server!");
            }
            return null;
        }

        public bool IsBattleOver()
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

        public bool CheckForWinner()
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
        */
    }
}