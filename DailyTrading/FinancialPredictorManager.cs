// ciumac.sergiu@gmail.com

using System;
using System.Collections.Generic;
using System.IO;


namespace DailyTrading
{
    /// <summary>
    /// Financial predictor manager
    /// </summary>
    public sealed class FinancialPredictorManager
    {
        #region Private Members

        /// <summary>
        /// Samples
        /// </summary>
        private readonly List<PredicInput> _samples = new List<PredicInput>();

        /// <summary>
        /// Input size
        /// </summary>
        private readonly int _inputSize;

        private readonly int _pointCount =5;
        private readonly int _predictDayplus = 3;
        /// <summary>
        /// Output size [% move]
        /// </summary>
        private readonly int _outputSize;

        #endregion

        #region Properties


        /// <summary>
        /// Max date for the training set
        /// </summary>
        public DateTime MaxDate { get; private set; }

        /// <summary>
        /// Min date for the training set
        /// </summary>
        public DateTime MinDate { get; private set; }

        public string Ticker { get; private set; }
        private string tickerIndex = "VNIndex";

        #endregion

        private const string DATE_HEADER = "Date";
        private const string ADJ_CLOSE_HEADER = "Close";
        private const string ADJ_VOLUMN_HEADER = "Volume";
        private const string ADJ_OPEN_HEADER = "Open";
        private const string ADJ_HIGH_HEADER = "High";
        private const string ADJ_LOW_HEADER = "Low";
        #region Constructors


        Dictionary<int, List<PData>> _dictionary = new Dictionary<int, List<PData>>();
        Dictionary<int, double> _dicMaxValue = new Dictionary<int, double>();
        Dictionary<int, double> _dicMinValue = new Dictionary<int, double>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inputSize">Input size</param>
        /// <param name="outputSize">Output size</param>
        public FinancialPredictorManager(int inputSize, int outputSize)
        {
            if (inputSize <= 0)
                throw new ArgumentException("inputSize cannot be less than 0");
            if (outputSize <= 0)
                throw new ArgumentException("outputSize cannot be less than 0");
            _inputSize = inputSize;
            _outputSize = outputSize;
            foreach (var index in Enum.GetValues(typeof(PredicInputIndexe)))
            {
                _dicMaxValue[(int)index] = double.MinValue;
                _dicMinValue[(int)index] = double.MaxValue;
            }
            MaxDate = DateTime.MaxValue;
            MinDate = DateTime.MinValue;
        }
        #endregion

        /// <summary>
        /// Get input data - S&P 500 Index, Prime Interest Rate, Dow index, Nasdaq index
        /// </summary>
        /// <param name="offset">Start index of input data</param>
        /// <param name="input">Array to be populated</param>
        /// <remarks>
        /// According to the <c>offset</c> parameter, first <c>_inputSize</c> values are drawn from the dataset 
        /// </remarks>
        public void GetInputData(int offset, double[] input)
        {
            int total = 0;
            int k = 0;
            // get OHLCVI *_pointCount
            for (int i = 0; i < _pointCount; i++)
            {
                PredicInput sample = _samples[offset + i];
                k = 0;

                foreach (PredicInputIndexe index in Enum.GetValues(typeof(PredicInputIndexe)))
                {

                    if (sample.IsSetValue((int)index))
                    {
                        input[i * total + k] = sample.GetValue((int)index);
                        k++;
                    }
                }
                if (total < 1)
                {
                    total = k;
                }
                //input[i*4]       = sample.Open;
                //input[i*4 + 1]   = sample.H;
                //input[i*4 + 2]   = sample.L;
                //input[i*4 + 3]   = sample.C;
            }
        }

        /// <summary>
        /// Get output data - S&P 500 Index, Prime Interest Rate, Dow index, Nasdaq index
        /// </summary>
        /// <param name="offset">Start index of output data</param>
        /// <param name="output">Output array to be populated</param>
        /// <remarks>
        /// The value of <c>offset + _inputSize</c> indexes value are drawn from the samples data set.
        /// E.g. Consider the <c>offset</c> parameter equal to 12581. Input parameters to the network will be
        /// values from [12581..12590]. The actual values will be equal to the parameters stored in the <code>12581 + _inputSize</code>
        /// place => 12591 index.
        /// </remarks>
        public void GetOutputData(int offset, double[] output)
        {
            PredicInput sample = _samples[offset + _pointCount+ _predictDayplus];
            output[0] = sample.GetValue((int)PredicInputIndexe.CloseIndex);
            //output[1] = sample.PrimeInterestRate;
            //output[2] = sample.Commodity;
            //output[3] = sample.VolumeCommo;

        }

        #region Get indexes

        public double GetMax(int index)
        {
            return _dicMaxValue[index];
        }

        public double GetMin(int index)
        {
            return _dicMinValue[index];
        }

        public bool IsSetIndex(int index)
        {
            return _dictionary.ContainsKey(index);
        }
        /* public double GetIndex(DateTime date, int index)
         {
             double currentAmount = 0;

             foreach (PData data in _dictionary[index])
             {
                 if (data.Date.CompareTo(date) >= 0)
                 {
                     return currentAmount;
                 }
                 currentAmount = data.Amount;
             }
             return currentAmount;
         }
         */
        public string GetDate(int offset)
        {
            PredicInput sample = _samples[offset];
            return sample.Date.ToShortDateString();
        }
        public DateTime GetDateTime(int offset)
        {
            PredicInput sample = _samples[offset];
            return sample.Date;
        }
        #endregion

        /// <summary>
        /// Get financial samples
        /// </summary>
        public IList<PredicInput> Samples
        {
            get { return _samples; }
        }

        
        public void Load(List<PredicInput> listdata, bool isnomalsize = true)
        {
            MaxDate = MaxDate.Subtract(new TimeSpan(_inputSize, 0, 0, 0)); /*Subtract 10 last days*/

            _samples.Clear();
            foreach (var data in listdata)
            {
                _samples.Add(data);

                foreach (PredicInputIndexe index in Enum.GetValues(typeof(PredicInputIndexe)))
                {
                    var amount = data.GetValue((int)index);
                    if (amount > _dicMaxValue[(int)index]) _dicMaxValue[(int)index] = amount;
                    if (amount < _dicMinValue[(int)index]) _dicMinValue[(int)index] = amount;
                }
                if (MaxDate.Date < data.Date) MaxDate = data.Date;
                if (MinDate.Date > data.Date) MinDate = data.Date;
            }           
            _samples.Sort();            /*Sort by date*/

            //increa max and deincrea min 5 point/
            foreach (PredicInputIndexe index in Enum.GetValues(typeof(PredicInputIndexe)))
            {
                
                  _dicMaxValue[(int)index] += 5;
                  _dicMinValue[(int)index] -= 5;
            }


            /*            using (StreamWriter writer = new StreamWriter("Datasamples.csv"))
                        {
                            for (int i = 0; i < _samples.Count; i++)
                            {
                                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                                builder.Append(_samples[i].Date.ToShortDateString());

                                foreach (PredicInputIndexe index in Enum.GetValues(typeof(PredicInputIndexe)))
                                {

                                    if (_samples[i].IsSetValue((int)index))
                                    {
                                        builder.Append(",");
                                        builder.Append(_samples[i].GetValue((int)index).ToString());                             
                                    }
                                }
                                writer.WriteLine(builder.ToString());                    
                            }
                        }
                        */
            if (isnomalsize)
                NormalizeData();
        }

       

        /// <summary>
        /// Get financial samples size
        /// </summary>
        public int Size
        {
            get { return _samples.Count; }
        }
 

        /// <summary>
        /// Normalize input data
        /// </summary>
        public void NormalizeData()
        {
            foreach (PredicInput t in _samples)
            {
                foreach (PredicInputIndexe index in Enum.GetValues(typeof(PredicInputIndexe)))
                {
                    if (t.IsSetValue((int)index))
                    {
                        var value = t.GetValue((int)index);
                        value = (value - _dicMinValue[(int)index]) / (_dicMaxValue[(int)index] - _dicMinValue[(int)index]);
                        t.SetValue((int)index, value);
                    }
                }
            }
        }
    }
}
