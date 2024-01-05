using System;
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
            for (; ; )
            {
                Thread.Sleep(100);
            }
        }
    }
}
