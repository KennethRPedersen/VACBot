using System.Threading.Tasks;

namespace Discord.Bot
{
    class Program
    {
        public static Task Main(string[] args)
            => Startup.RunAsync(args);
    }
}
