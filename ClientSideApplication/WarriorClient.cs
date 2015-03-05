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
        private readonly HttpClient _client;
        private readonly Guid _myGuid;

        public WarriorClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(ConfigSettings.ReadSetting("Url"));
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

           if(!Guid.TryParse(ConfigSettings.ReadSetting("MyGUID"), out _myGuid))
           {
               _myGuid = Guid.NewGuid();
           }
        }



        public AttackOutcome Attack(Strength strength)
        {
            try
            {
                string info = strength.ToString();

                HttpResponseMessage responseMessage = _client.PostAsJsonAsync("Attack/" + WarriorBrain.RoomGuid + "/" + _myGuid + "/" + info, "a").Result;
                responseMessage.EnsureSuccessStatusCode();

                string content = responseMessage.Content.ReadAsAsync<string>().Result;
                return (AttackOutcome)Enum.Parse(typeof(AttackOutcome), content);
            }
            catch (Exception exception)
            {
                WarriorLogger.UnableToConnect();
            }
            return AttackOutcome.WrongData;
        }

        public DefenseOutcome Defend(int time)
        {
            try
            {
                string info = time.ToString();

                HttpResponseMessage responseMessage = _client.PostAsJsonAsync("Defend/" + WarriorBrain.RoomGuid + "/" + _myGuid + "/" + info, "aaa").Result;
                responseMessage.EnsureSuccessStatusCode();

                string content = responseMessage.Content.ReadAsAsync<string>().Result;
                return (DefenseOutcome)Enum.Parse(typeof(DefenseOutcome), content);
            }
            catch (Exception exception)
            {
                WarriorLogger.UnableToConnect();
            }
            return DefenseOutcome.WrongData;
        }

        public RestOutcome Rest(int time)
        {
            try
            {
                string info = time.ToString();

                HttpResponseMessage responseMessage = _client.PostAsJsonAsync("Rest/" + WarriorBrain.RoomGuid + "/" + _myGuid + "/" + info, "aaa").Result;
                responseMessage.EnsureSuccessStatusCode();

                string content = responseMessage.Content.ReadAsAsync<string>().Result;
                return (RestOutcome)Enum.Parse(typeof(AttackOutcome), content);
            }
            catch (Exception exception)
            {
                WarriorLogger.UnableToConnect();
            }
            return RestOutcome.WrongData;
        }

        public WarriorState Check()
        {
            try
            {
                HttpResponseMessage responseMessage = _client.PostAsJsonAsync("Check/" + WarriorBrain.RoomGuid + "/" + _myGuid, "aaa").Result;
                responseMessage.EnsureSuccessStatusCode();

                WarriorState enemyState = responseMessage.Content.ReadAsAsync<WarriorState>().Result;

                return enemyState;
            }
            catch (Exception exception)
            {
                WarriorLogger.UnableToConnect();
            }
            return null;
        }



        public Guid HostGame()
        {
            while (true)
            {
                try
                {
                    HttpResponseMessage responseMessage = _client.PostAsJsonAsync("HostGame/" + _myGuid, "aaa").Result;
                    responseMessage.EnsureSuccessStatusCode();

                    string content = responseMessage.Content.ReadAsAsync<string>().Result;
                    //content = content.Substring(1, content.Length - 2);

                    return Guid.Parse(content);
                }
                catch (Exception e )
                {
                    WarriorLogger.UnableToConnect();
                    Console.WriteLine(e);
                }
            }
        }

        public JoinRoomOutcome JoinGame(Guid roomGuid)
        {
            while (true)
            {
                try
                {
                    HttpResponseMessage responseMessage = _client.PostAsJsonAsync("JoinGame/" + roomGuid + "/" + _myGuid, "aaa").Result; 
                    responseMessage.EnsureSuccessStatusCode();

                    string content = responseMessage.Content.ReadAsAsync<string>().Result;
                    return (JoinRoomOutcome)Enum.Parse(typeof (JoinRoomOutcome), content);
                }
                catch (Exception)
                {
                    WarriorLogger.UnableToConnect();
                }
            }
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
                WarriorLogger.UnableToConnect();
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
                    WarriorLogger.UnableToConnect();
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
                WarriorLogger.Winner(content == "WINNER");
                return content == "NOT WINNER";
            }
            catch (Exception exception)
            {
                WarriorLogger.UnableToConnect();
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
                WarriorLogger.UnableToConnect();
            }
            return false;
        }

    }
}