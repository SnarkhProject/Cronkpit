using System;

namespace Cronkpit
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CronkPit game = new CronkPit())
            {
                game.Run();
            }
        }
    }
#endif
}

