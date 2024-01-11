using System.Threading;
using Controller;
using View;

namespace RaceSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Data.Initialize();
            Data.NextRace();
            
            ASCIIvisualizer.Initialize();
            
            Data.CurrentRace.Start();
            
            for (; ; )
            {
                Thread.Sleep(Data.raceTimerMs);
            }
        }
    }
}
