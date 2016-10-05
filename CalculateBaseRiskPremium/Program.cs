using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateBaseRiskPremium
{
    class Program
    {
        static void Main(string[] args)
        {
            double[,] RiskPremium = new double[100, 101];

            double[] stacks = new double[100];
            for (int i = 1; i < 100; i++) stacks[i] = 1;
            stacks[0] = 2;

            int maxiter = 1000000;

            double[,] equitiessum = new double[100, 100];

            object locker = new object();

            Parallel.For(0, maxiter,
                () => new double[100, 100],
                (iter, state, localsum) =>
                {

                    double[] results = new double[100];

                    double[] stackpower = new double[100];

                    for (int i = 0; i < 100; i++)
                    {
                        stackpower[i] = 1 / stacks[i];
                        double res = Utils.NextDouble();
                        results[i] = Math.Pow(res, stackpower[i]);
                    }

                    for (int stackcount = 2; stackcount < 100; stackcount++)
                    {

                        int[] origstacknumber = new int[stackcount];

                        for (int i = 0; i < stackcount; i++)
                        {
                            origstacknumber[i] = i;
                        }

                        double[] resultscopy = new double[stackcount];
                        for (int i = 0; i < stackcount; i++)
                        {
                            resultscopy[i] = results[i];
                        }

                        Array.Sort(resultscopy, origstacknumber);
                        Array.Reverse(origstacknumber);

                        for (int tickets = 2; tickets < stackcount + 1; tickets++) localsum[tickets, stackcount] += Array.IndexOf(origstacknumber, 0) < tickets ? 1 : 0;
                    }

                    return localsum;

                },
            (localsum) =>
            {
                lock (locker)
                {
                    for (int i = 0; i < 100; i++) for (int j = 0; j < 100; j++)
                            equitiessum[i, j] += localsum[i, j];
                }
            });

            for (int stackcount = 2; stackcount < 100; stackcount++)
                for (int tickets = 2; tickets < stackcount + 1; tickets++)
                    RiskPremium[tickets, stackcount + 1] = ((double)tickets / (stackcount + 1)) / (equitiessum[tickets, stackcount] / maxiter) - 0.5;

            string fileName = "riskpremium.dat";
            if (File.Exists(fileName))
            {
                Console.WriteLine(fileName + " already exists!");
                return;
            }

            using (FileStream fs = new FileStream(fileName, FileMode.CreateNew))
            {
                using (BinaryWriter w = new BinaryWriter(fs))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        for (int j = 0; j < 101; j++)
                        {
                            w.Write(RiskPremium[i, j]);
                        }
                    }
                }
            }
        }

    }

}