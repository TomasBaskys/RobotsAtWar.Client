using System;
using System.Collections.Generic;
using System.Threading;
using ClientSideApplication.Enums;

namespace ClientSideApplication
{
    public class WarriorBrain
    {
        private bool _bothUsersOnline = false;
        private readonly WarriorClient _warriorClient;
        private int _currentActionNumber;
        private readonly List<Command> _strategy;
        public static Guid RoomGuid;
        private bool _ihaveLost;
        public static WarriorState MyInfo = new WarriorState() { Life = 10, State = States.DoingNothing };
        public WarriorBrain(List<Command> strategy)
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
                    RoomGuid = Guid.Parse(Console.ReadLine());
                    _warriorClient.JoinGame(RoomGuid);
                }
                else
                {
                    Console.WriteLine("Wrong letter, please try again.");
                }
            }
            while (!_bothUsersOnline)
            {
                _bothUsersOnline = _warriorClient.AreBothUsersOnline();
                Thread.Sleep(100);
            }

            Thread checkGetAttackedThread = new Thread(_warriorClient.GetMyInfo);
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

        private void ExecuteCommand(Command command)
        {
            switch (command.Action)
            {
                case ActionName.Attack:
                    _warriorClient.Attack(command.Strength);
                    break;
                case ActionName.Defend:
                    _warriorClient.Defend(command.Time);
                    break;
                case ActionName.Rest:
                    _warriorClient.Rest(command.Time);
                    break;
                case ActionName.Check:
                    _warriorClient.Check();
                    break;
            }
        }
    }
}
