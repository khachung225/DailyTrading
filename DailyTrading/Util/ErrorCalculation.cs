// ciumac.sergiu@gmail.com
using System;
using Encog.ML.Data;

namespace FinancialMarketPredictor.Utilities
{
    /// <summary>
    /// Class for error calculation
    /// </summary>
    public class ErrorCalculation
    {
        /// <summary>
        /// The current error level.
        /// </summary>
        private double _globalError;

        /// <summary>
        /// The size of a training set.
        /// </summary>
        private int _setSize;

        /// <summary>
        /// Returns the root mean square error for a complete training set.
        /// </summary>
        /// <returns>The current error for the neural network.</returns>
        public double CalculateRMS()
        {
            return Math.Sqrt(_globalError/(_setSize));
        }

        /// <summary>
        /// Reset the error accumulation to zero.
        /// </summary>
        public void Reset()
        {
            _globalError = _setSize = 0;
        }

        /// <summary>
        /// Called to update for each number that should be checked.
        /// </summary>
        /// <param name="actual">The actual number.</param>
        /// <param name="ideal">The ideal number.</param>
        public void UpdateError(double[] actual, double[] ideal)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                double delta = ideal[i] - actual[i];
                _globalError += delta * delta;
            }
            _setSize += ideal.Length;
        }
        public void UpdateError(double[] actual, IMLData ideal)
        {
            for (int i = 0; i < actual.Length; i++)
            {
                double delta = ideal[i] - actual[i];
                _globalError += delta * delta;
            }
            _setSize += ideal.Count;
        }

    }
    public class LeanOverFitting
    {
        private const int Maxfit = 10;
        private static int t = -1;
        private static void SetStep()
        {
            if (t == (Maxfit - 1))
                t = 0;
            else
                t++;
        }
        private static int GetCompair()
        {
            if (t == (Maxfit - 1))
                return 0;
            return t + 1;
        }
        public static void Init()
        {
            for (int i = 0; i < Maxfit; i++)
            {
                _data[i] = double.MaxValue;
            }
        }
        private static double[] _data = new double[Maxfit];

        public static bool IsOverfilling(double er)
        {
            SetStep();
            _data[t] = er;
            if (_data[t] > _data[GetCompair()])
                return true;
            return false;
        }
    }

    public class MyError
    {
        public double value { get; set; }
        public int index { get; set; }
    }
}
