namespace MyntUI
{
    class Program
    {
        static void Main(string[] args)
        {
            WebApplication.ProcessInit();
        }
    }

    public static class WebApplication
    {
        public static void ProcessInit()
        {
            Startup.RunWebHost();
        }
    }

}
