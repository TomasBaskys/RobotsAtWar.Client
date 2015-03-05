using System;
using System.Collections.Generic;
using System.Threading;
using ClientSideApplication.Enums;
using ClientSideApplication.Tools;
using log4net.Repository.Hierarchy;

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
            string action;
            Console.WriteLine("Do you want to join a game or host it?\n \n'J' to Join, 'H' to Host ");
            while (true)
            {
                action = Console.ReadLine();
                if (action != null && action.ToLower() == "h")
                {
                    RoomGuid = _warriorClient.HostGame();
                    WarriorLogger.GameHost(RoomGuid);
                    break;
                }
                if (action != null && action.ToLower() == "j")
                {
                    
                    string guid = Console.ReadLine();

                    if (Guid.TryParse(guid, out RoomGuid))
                    {
                        JoinRoomOutcome joinOutcome = _warriorClient.JoinGame(RoomGuid);
                        WarriorLogger.JoinGame(joinOutcome);
                        break;
                    }
                    WarriorLogger.WrongRoomGuid();
                    
                }
                else
                {
                    WarriorLogger.WrongLetter();
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
                if (_warriorClient.IsBattleOver())
                {
                    _ihaveLost = _warriorClient.CheckForWinner();
                    break;
                }
                ExecuteNextCommand();
            }
            if (_ihaveLost)
            {
                WarriorLogger.BattleLost();
                for (int i = 5; i > 0; i--)
                {
                    WarriorLogger.CountDown(i);
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
                    AttackOutcome attackOutcome = _warriorClient.Attack(command.Strength);
                    WarriorLogger.AttackLogging(attackOutcome, command.Strength);
                    break;
                case ActionName.Defend:
                    DefenseOutcome defenseOutcome = _warriorClient.Defend(command.Time);
                    WarriorLogger.DefenceLogging(defenseOutcome);
                    break;
                case ActionName.Rest:
                    RestOutcome restOutcome = _warriorClient.Rest(command.Time);
                    WarriorLogger.RestLogging(restOutcome);
                    break;
                case ActionName.Check:
                    WarriorState warriorState = _warriorClient.Check();
                    WarriorLogger.CheckLogging(warriorState);
                    break;
            }
        }
    }
}
