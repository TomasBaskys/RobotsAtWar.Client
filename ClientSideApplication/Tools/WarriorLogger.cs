using System;
using ClientSideApplication.Enums;
using log4net;

namespace ClientSideApplication.Tools
{
    public class WarriorLogger
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WarriorLogger));

        public static void GameHost(Guid roomGuid)
        {
            Logger.Info("You are hosting a game room, it's guid is:" + roomGuid);
        }

        public static void JoinGame(JoinRoomOutcome outcome)
        {
            switch (outcome)
            {
                case JoinRoomOutcome.Success:
                    Logger.Info("You have joined game room");
                    break;
                case JoinRoomOutcome.NoSuchRoomExists:
                    Logger.Error("No room with given guid exists");
                    break;
                case JoinRoomOutcome.UserAlreadyInRoom:
                    Logger.Error("User with same username allready exists");
                    break;
            }
        }

        public static void WrongLetter()
        {
            Logger.Error("You have entered wrong letter, please try again ");
        }

        public static void BattleLost()
        {
            Logger.Info("You have lost the battle...\nConsole closing in...");
        }
        public static void AttackLogging(AttackOutcome outcome, Strength strength)
        {
            switch (outcome)
            {
                case AttackOutcome.Success:
                    Logger.Info("Your " + strength + " attack was " + outcome);
                    break;
                case AttackOutcome.Interrupted:
                    Logger.Info("Your " + strength + " attack was interrupted");
                    break;
                case AttackOutcome.WrongData:
                    WrongAttackData();
                    break;
                case AttackOutcome.Blocked:
                    Logger.Info("Your was blocked with a shield");
                    break;
                case AttackOutcome.BattleNotStarted:
                    BattleNotStarted();
                    break;
            }
            Logger.Info("Your " + strength + " attack was " + outcome);
        }
        public static void DefenceLogging(DefenseOutcome outcome)
        {
            switch (outcome)
            {
                case DefenseOutcome.Success:
                    Logger.Info("You have succesfully defended");
                    break;
                case DefenseOutcome.WrongData:
                    WrongDefendingData();
                    break;
                case DefenseOutcome.BattleNotStarted:
                    BattleNotStarted();
                    break;
            }
        }
        public static void CheckLogging(WarriorState outcome)
        {
            Logger.Info("You checked your enemy, its life is " + outcome.Life + ", state is " + outcome.State);
        }

        public static void WrongAttackData()
        {
            Logger.Error("You have entered wrong data for your attack");
        }

        public static void WrongRestData()
        {
            Logger.Error("You have entered wrong data when attempting to rest");
        }

        public static void WrongDefendingData()
        {
            Logger.Error("You have entered wrong data when defending");
        }

        public static void BattleNotStarted()
        {
            Logger.Error("Battle has not started yet!");

        }
        public static void RestLogging(RestOutcome outcome)
        {
            switch (outcome)
            {
                case RestOutcome.Success:
                    Logger.Info("You have succefully rested ");
                    break;
                case RestOutcome.Interrupted:
                    Logger.Info("You got interrupted while resting");
                    break;
                case RestOutcome.WrongData:
                    WrongRestData();
                    break;
                case RestOutcome.BattleNotStarted:
                    BattleNotStarted();
                    break;
            }
        }

        public static void WrongRoomGuid()
        {
            Logger.Info("The war room guid you have provided is incorrect!");
        }
        public static void UnableToConnect()
        {
            Logger.Error("Unable to connect to server!");
        }

        public static void CountDown(int number)
        {
            Logger.Info(number);
        }

        public static void Winner(bool winner)
        {
            if (winner)
            {
                Logger.Info("I have won the game!");
            }
            else Logger.Info("I have lost the game!");
            
        }
    }
}