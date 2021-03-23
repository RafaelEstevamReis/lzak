namespace HostApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            Core c = Core.GetInstance();
            c.Run();
        }
    }
}
