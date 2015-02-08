using System.IO;
using log4net.Config;

namespace ClientSideApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("..\\..\\App.config"));

            WarriorBrain warriorBrain = new WarriorBrain(Strategy.YourStrategy());
            warriorBrain.Start();
        }
    }
}
