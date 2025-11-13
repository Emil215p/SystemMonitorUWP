namespace SystemMonitorUWP.Code
{
    public class Shared
    {
        private static readonly Shared _instance = new Shared();
        public static Shared Instance => _instance;

        //public int MyVar = 5;

        private Shared()
        {

        }
    }
}