namespace ClientSideApplication
{
    public class Commands
    {
        public readonly WarriorBrain.Actions Action;
        public readonly int Time;
        public readonly WarriorBrain.Strength Strength;

        public Commands(WarriorBrain.Actions action, int time)
        {
            Action = action;
            Time = time;
        }

        public Commands(WarriorBrain.Actions action, WarriorBrain.Strength strength)
        {
            Action = action;
            Strength = strength;
        }
    }
}