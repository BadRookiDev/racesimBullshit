using System;
using System.Threading;
using Controller;

namespace RaceSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Data.Initialize();
            Data.NextRace();
            Console.WriteLine(Data.CurrentRace.Track.Name);
            for (; ; )
            {
                Thread.Sleep(100);
            }
        }
    }
}
