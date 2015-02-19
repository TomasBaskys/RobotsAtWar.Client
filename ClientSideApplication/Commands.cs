using ClientSideApplication.Enums;

namespace ClientSideApplication
{
    public class Command
    {
        public ActionName Action;
        public int Time;
        public Strength Strength;


        //For def and rest only
        public Command(ActionName action, int time)
        {
            switch (action)
            {
                case ActionName.Attack:
                    return;
                case ActionName.Check:
                    return;
            }
            Action = action;
            Time = time;
        }

        //For attack only
        public Command(ActionName action, Strength str)
        {
            if (action != ActionName.Attack)
            {
                return;
            }
            Action = action;
            Strength = str;
        }
        //For check only
        public Command(ActionName action)
        {
            if (action != ActionName.Check)
            {
                return;
            }
            this.Action = action;
        }
    }
}