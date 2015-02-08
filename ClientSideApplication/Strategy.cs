using System;
using System.Collections.Generic;
using System.Linq;
using ClientSideApplication.Tools;

namespace ClientSideApplication
{
    public class Strategy
    {
        public static List<Commands> YourStrategy()
        {
            string[] actions = ConfigSettings.ReadSetting("Action").Split(',').Select(s => s.Trim()).ToArray();
            string[] actionLevel = ConfigSettings.ReadSetting("ActionLevel").Split(',').Select(s => s.Trim()).ToArray();

            List<Commands> myStrategy = new List<Commands>();

            for (int i = 0; i < actions.Length; i++)
            {
                var action = (WarriorBrain.Actions) Enum.Parse(typeof (WarriorBrain.Actions), actions[i]);
                if (action == WarriorBrain.Actions.Attack)
                {
                    var strength = (WarriorBrain.Strength) Enum.Parse(typeof (WarriorBrain.Strength), actionLevel[i]);
                    myStrategy.Add(new Commands(action, strength));
                }
                else
                {
                    myStrategy.Add(new Commands(action, Int32.Parse(actionLevel[i])));
                }
            }
            return myStrategy;
        }
    }
}