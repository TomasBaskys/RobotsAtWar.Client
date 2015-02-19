using System;
using System.Collections.Generic;
using System.Threading;
using log4net;

namespace ClientSideApplication
{
    public class WarriorBrain
    {
        public enum Actions { Attack, Defend, Rest, Check }
        public enum Strength { Weak, Medium, Strong, None }

        private static readonly ILog Logger = LogManager.GetLogger(typeof(WarriorClient));

        private bool BothUsersOnline = false;
        private readonly WarriorClient _warriorClient;
        private int _currentActionNumber;
        private DateTime _battleTime = new DateTime(2000, 01, 01);
        private readonly List<Commands> _strategy;
        public static string RoomGuid;
        private bool _ihaveLost;

        public WarriorBrain(List<Commands> strategy)
        {
            _warriorClient = new WarriorClient();
            _strategy = strategy;
        }


        public void Start()
        {
            string action = "";
            Console.WriteLine("Do you want to join a game or host it?\n \n'J' to Join, 'H' to Host ");
            while (action != null && (action.ToLower() != "j" && action.ToLower() != "h"))
            {
                action = Console.ReadLine();
                if (action != null && action.ToLower() == "h")
                {
                    RoomGuid = _warriorClient.HostGame();
                }
                else if (action != null && action.ToLower() == "j")
                {
                    Console.WriteLine("Please insert guid of WarRoom that you wish to join");
                    RoomGuid = Console.ReadLine();
                    _warriorClient.JoinGame(RoomGuid);
                }
                else
                {
                    Console.WriteLine("Wrong letter, please try again.");
                }
            }
            while (!BothUsersOnline)
            {
                BothUsersOnline = _warriorClient.AreBothUsersOnline();
                Thread.Sleep(100);
            }

            Thread checkGetAttackedThread = new Thread(_warriorClient.CheckGetAttacked);
            checkGetAttackedThread.Start();

            Fight();
        }


        private void Fight()
        {
            while (true)
            {
                if (WarriorClient.IsBattleOver())
                {
                    _ihaveLost = WarriorClient.CheckForWinner();
                    break;
                }
                ExecuteNextCommand();
            }
            if (_ihaveLost)
            {
                Console.WriteLine("I have lost the battle...\nConsole closing in...");
                for (int i = 5; i > 0; i--)
                {
                    Console.WriteLine(i);
                    Thread.Sleep(1000);
                }
                Environment.Exit(0);
            }
        }

        private void ExecuteNextCommand()
        {
            ExecuteCommand(_strategy[_currentActionNumber % _strategy.Count]);
            _currentActionNumber++;
        }

        private void ExecuteCommand(Commands command)
        {
            switch (command.Action)
            {
                case Actions.Attack:
                    _warriorClient.Attack(command.Strength);
                    break;
                case Actions.Defend:
                    _warriorClient.Defend(command.Time);
                    break;
                case Actions.Rest:
                    _warriorClient.Rest(command.Time);
                    break;
                case Actions.Check:
                    _warriorClient.Check();
                    break;
            }
        }
    }
}
