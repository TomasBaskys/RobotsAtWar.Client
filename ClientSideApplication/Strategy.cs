using System;
using System.Collections.Generic;
using System.Linq;
using ClientSideApplication.Enums;
using ClientSideApplication.Tools;

namespace ClientSideApplication
{
    public class Strategy
    {
        public static List<Command> YourStrategy()
        {
            string[] actions = ConfigSettings.ReadSetting("Action").Split(',').Select(s => s.Trim()).ToArray();
            string[] actionLevel = ConfigSettings.ReadSetting("ActionLevel").Split(',').Select(s => s.Trim()).ToArray();
            Strength strength = Strength.None;
            ActionName action = ActionName.Check;
            int time;
            List<Command> myStrategy = new List<Command>();

            for (int i = 0; i < actions.Length; i++)
            {
                
                switch (actions[i])
                {
                    case "Attack":
                        action = ActionName.Attack;
                        break;
                    case "Defend":
                        action = ActionName.Defend;
                        break;
                    case "Rest":
                        action = ActionName.Rest;
                        break;
                    case "Check":
                        action = ActionName.Check;
                        break;
                }
                if (action == ActionName.Rest || action == ActionName.Defend)
                {
                    time = Int32.Parse(actionLevel[i]);
                    myStrategy.Add(new Command(action,time));
                }
                else if (action == ActionName.Attack)
                {
                    try
                    {
                        strength = (Strength)Enum.Parse(typeof(Strength), actionLevel[i]);
                        myStrategy.Add(new Command(action,strength));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Wrong strength value in strategy");
                    }
                }
                else
                {
                    myStrategy.Add(new Command(action));
                }
            }
            return myStrategy;
        }
    }
}