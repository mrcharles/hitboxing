using System;

namespace HitBoxing
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (HitBoxing game = new HitBoxing())
            {
                game.Run();
            }
        }
    }
#endif
}

