using ClientSideApplication.Enums;


namespace ClientSideApplication
{
    public class WarriorState
    {

        private readonly object _lockState = new object();
        private readonly object _lockState2 = new object();
        private States _state;
        private int _life;

        public States State
        {
            get
            {
                lock (_lockState)
                {
                    return _state;
                }
            }
            set
            {
                lock (_lockState)
                {
                    _state = value;
                }
            }
        }

        public int Life
        {
            get
            {
                lock (_lockState2)
                {
                    return _life;
                }
            }

            set
            {
                lock (_lockState2)
                {
                    _life = value;
                }
            }
        }

        public WarriorState()
        {
            State = States.DoingNothing;
            Life = 100;
        }

    }
}