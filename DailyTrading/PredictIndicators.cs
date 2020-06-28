// ciumac.sergiu@gmail.com
//#define LOG_DATASET
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;
//using Encog.Neural.Activation;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using FinancialMarketPredictor.Utilities;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using DailyTrading;

namespace DailyTrading
{
    /// <summary>
    /// Training algorithm
    /// </summary>
    public enum TrainingAlgorithm
    {
        /// <summary>
        /// Backpropagation learning
        /// </summary>
        Resilient,

        /// <summary>
        /// Simulated annealing
        /// </summary>
        Annealing,

        /// <summary>
        /// Evolutionary learning
        /// </summary>
        Evolutionary,
        /// <summary>
        /// Fuzzy logic Leanning
        /// </summary>
        ANFIS
    }
    /// <summary>
    /// Training status delegate
    /// </summary>
    /// <param name="iteration">Epoch number</param>
    /// <param name="error">Error</param>
    /// <param name="algorithm">Training algorithm</param>
    public delegate void TrainingStatus(int iteration, double error, TrainingAlgorithm algorithm);

    /// <summary>
    /// Class for prediction
    /// </summary>
    public sealed class PredictIndicators
    {
        #region Constants
        /// <summary>
        /// Indexes to consider
        /// </summary>
        /// <remarks>
        /// Dow index, Prime interest rate, Nasdaq index, S&P500 index
        /// </remarks>
        private const int INDEXES_TO_CONSIDER = 6; //so dau vao

        /// <summary>
        /// Input Tuples. Each tuple consist of a pair: <c>S&P500</c> index and prime interest rate PIR
        /// </summary>
        /// <remarks>
        /// The total amount of input synapses equals <c>InputTupples * IndexesToConsider</c>
        /// </remarks>
        private const int INPUT_TUPLES = 5; // ban ghi can du doan.

        /// <summary>
        /// The size of network's output
        /// </summary>
        private const int OUTPUT_SIZE = 1; // dau ra

        /// <summary>
        /// Maximal error
        /// </summary>
        private const double MAX_ERROR = 0.000299;


        #endregion

        #region Private Members

        /// <summary>
        /// Network to be trained
        /// </summary>
        private BasicNetwork _network;

        /// <summary>
        /// Input data S&P, Prime Interest Rate, Nasdaq, Dow indexes
        /// </summary>
        private double[][] _input;

        /// <summary>
        /// Desired output
        /// </summary>
        private double[][] _ideal;

        /// <summary>
        /// Financial market predictor
        /// </summary>
        private FinancialPredictorManager _manager;

        /// <summary>
        /// Training tread
        /// </summary>
        private Thread _trainThread;

      
        /// <summary>
        /// Size of the training data
        /// </summary>
        private int _trainingSize = 10000;

        #endregion

        /// <summary>
        /// Gets the information about the predictor
        /// </summary>
        public bool Loaded { get; private set; }

        /// <summary>
        /// Hidden layers
        /// </summary>
        public int HiddenUnits1 { get; private set; }

        /// <summary>
        /// Hidden units
        /// </summary>
        public int HiddenUnits2 { get; private set; }

        /// <summary>
        /// Maximum date for training and prediction
        /// </summary>
        public DateTime MaxIndexDate
        {
            get
            {
                return _manager == null ? DateTime.MinValue : _manager.MaxDate;
            }
        }

        /// <summary>
        /// Minimum date for training and prediction
        /// </summary>
        public DateTime MinIndexDate
        {
            get
            {
                return _manager == null ? DateTime.MaxValue : _manager.MinDate;
            }
        }

     
        #region Constructors

        public PredictIndicators(List<PredicInput> listdata, int hiddenLayers1, int hiddenLayers2)
        {

            

            CreateNetwork(hiddenLayers1, hiddenLayers2);                                                       /*Create new network*/
            _manager = new FinancialPredictorManager(INPUT_TUPLES, OUTPUT_SIZE);     /*Create new financial predictor manager*/
            _manager.Load(listdata);     /*Load S&P 500 and prime interest rates*/
            Loaded = true;
            HiddenUnits1 = hiddenLayers1;
            HiddenUnits2 = hiddenLayers2;
        }
        
        #endregion

       
        /// <summary>
        /// Create a new network
        /// </summary>
        private void CreateNetwork(int hiddenLayers1, int hiddenLayers2)
        {
            _network = new BasicNetwork();
            _network.AddLayer(new BasicLayer(INPUT_TUPLES * INDEXES_TO_CONSIDER));                             /*Input 30*/
           // for (int i = 0; i < hiddenLayers; i++)
            _network.AddLayer(new BasicLayer(new ActivationTANH(), true, hiddenLayers1));                 /*Hidden layer*/
            _network.AddLayer(new BasicLayer(new ActivationTANH(), true, hiddenLayers2));                 /*Hidden layer*/
           
            _network.AddLayer(new BasicLayer(new ActivationTANH(), true, OUTPUT_SIZE));                      /*Output of the network*/
            _network.Structure.FinalizeStructure();                                                         /*Finalize network structure*/
            _network.Reset();                                                                               /*Randomize*/
            //_network.Structure.HiddentLayer = hiddenLayers;
            //_network.Structure.HiddentUnit = hiddenUnits;
        }
 
        /// <summary>
        /// Create Training sets for the neural network to be trained
        /// </summary>
        /// <param name="trainFrom">Initial date, from which to gather indexes</param>
        /// <param name="trainTo">Final date, to which to gather indexes</param>
        public void CreateTrainingSets(DateTime trainFrom, DateTime trainTo)
        {
            // find where we are starting from
            int startIndex = 0;
            int endIndex = -1;
            foreach (var sample in _manager.Samples)
            {
                if (sample.Date.CompareTo(trainFrom.Date) < 0)
                    startIndex++;
                if (sample.Date.CompareTo(trainTo.Date) < 0)
                    endIndex++;
            }
            // create a sample factor across the training area
            _trainingSize = endIndex - startIndex;
            _input = new double[_trainingSize][];
            _ideal = new double[_trainingSize][];

            // grab the actual training data from that point
            for (int i = startIndex; i < endIndex; i++)
            {
                _input[i - startIndex] = new double[INPUT_TUPLES * INDEXES_TO_CONSIDER];
                _ideal[i - startIndex] = new double[OUTPUT_SIZE];
                _manager.GetInputData(i, _input[i - startIndex]);
                _manager.GetOutputData(i, _ideal[i - startIndex]);
            }
#if LOG_DATASET
            using (StreamWriter writer = new StreamWriter("dataset.csv"), ideal = new StreamWriter("ideal.csv"))
            {
                for (int i = 0; i < _input.Length; i++)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.Append(_manager.GetDate(i));
                    builder.Append(",");
                    for (int j = 0; j < _input[0].Length; j++)
                    {
                        builder.Append(_input[i][j]);
                        if (j != _input[0].Length - 1)
                            builder.Append(",");
                    }
                    writer.WriteLine(builder.ToString());

                    StringBuilder idealData = new StringBuilder();
                    for (int j = 0; j < _ideal[0].Length; j++)
                    {
                        idealData.Append(_ideal[i][j]);
                        if (j != _ideal[0].Length - 1)
                            idealData.Append(",");
                    }
                    ideal.WriteLine(idealData.ToString());
                }
            }
#endif

        }

        /// <summary>
        /// Train the network using Backpropagation and SimulatedAnnealing methods
        /// </summary>
        /// <param name="trainTo">Train until a specific date</param>
        /// <param name="status">Callback function invoked on each _epoch</param>
        /// <param name="trainFrom">Initial date, from which to gather training data</param>
        public void TrainNetworkAsync(DateTime trainFrom, DateTime trainTo, TrainingStatus status)
        {
            Action<DateTime, DateTime, TrainingStatus> action = TrainNetwork;
            action.BeginInvoke(trainFrom, trainTo, status, action.EndInvoke, action);
        }

        
       private List<MyError> listErr = new List<MyError>();

       public delegate void TimeTrainningSetEventHandler(string timeset, TimeSpan timeSpan, bool ishoitu, int counter);
       public event TimeTrainningSetEventHandler timeTrainningSet;
        private DateTime _startTime;
        private void setTimeEnd(bool ishoitu,int counter)
        {
            if (timeTrainningSet != null)
            {
                var time = DateTime.Now;
                var t = time - _startTime;
                timeTrainningSet(string.Format("{0} Minutes {1} Seconds", t.Minutes, t.Seconds), t, ishoitu, counter);
            }
        }
        /// <summary>
        // Train network
        /// </summary>
        /// <param name="status">Delegate to be invoked</param>
        /// <param name="trainFrom">Train from</param>
        /// <param name="trainTo">Train to</param>
        private void TrainNetwork(DateTime trainFrom, DateTime trainTo, TrainingStatus status)
        {
            if(_input == null || _ideal == null)
                CreateTrainingSets(trainFrom, trainTo);         /*Create training sets, according to input parameters*/

            _trainThread = Thread.CurrentThread;
            int epoch = 1;
            //LeanOverFitting.Init();
            _startTime = DateTime.Now;
           listErr.Clear();
            bool istoitu = true;
            ITrain train = null;
            try
            {
               
                var trainSet = new BasicNeuralDataSet(_input, _ideal);
                train = new ResilientPropagation(_network, trainSet);
                double error;
                do
                {
                   // train.AddStrategy();
                    train.Iteration();
                    error = train.Error;
                    if (status != null)
                        status.Invoke(epoch, error, TrainingAlgorithm.Resilient);
                    
                   // if (LeanOverFitting.IsOverfilling(error))
                    //    AbortTraining();
                    listErr.Add(new MyError{index = epoch,value = error});

                    if (epoch > 1500)
                    {
                    //    istoitu = false;
                      //  break;
                    }
                    epoch++;
                } while (error > MAX_ERROR);
            }
            catch (ThreadAbortException) {
                /*Training aborted*/
                if (_trainThread != null) _trainThread.Abort();
                _trainThread = null; }
            finally
            {
                setTimeEnd(istoitu, epoch);
                if (train != null) train.FinishTraining();
            }
            if (_trainThread != null) _trainThread.Abort();
            _trainThread = null;
        }
 
        public void ShowError()
        {
            if (listErr.Count > 0)
            {
                var p = new PError();
                p.DrawGraph(listErr);
                p.Show();
            }
        }

        /// <summary>
        /// Abort training
        /// </summary>
        public void AbortTraining()
        {
            if (_trainThread != null) _trainThread.Abort();
            setTimeEnd(false,0);
        }



        /// <summary>
        /// Export neural network
        /// </summary>
        /// <param name="path"></param>
        [System.Security.Permissions.FileIOPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void ExportNeuralNetwork(string path)
        {
            if (_network == null)
                throw new NullReferenceException("Network reference is set to null. Nothing to export.");
            Encog.Util.SerializeObject.Save(path, _network);
        }

        /// <summary>
        /// Load neural network
        /// </summary>
        /// <param name="path">Path to previously serialized object</param>
        public void LoadNeuralNetwork(string path)
        {
            _network = (BasicNetwork)Encog.Util.SerializeObject.Load(path);
            //HiddenLayers = _network.Structure.HiddentLayer /*1 input, 1 output*/;
            //HiddenUnits = _network.Structure.HiddentUnit;
        }

        /// <summary>
        /// Predict the results
        /// </summary>
        /// <returns>List with the prediction results</returns>
        public List<PredicResults> Predict(DateTime predictFrom, DateTime predictTo)
        {
            var results = new List<PredicResults>();
            double[] present = new double[INPUT_TUPLES * INDEXES_TO_CONSIDER];
            double[] actualOutput = new double[OUTPUT_SIZE];
            int index = 0;
            foreach (var sample in _manager.Samples)
            {
                if (sample.Date.Date.CompareTo(predictFrom.Date) > 0 && sample.Date.Date.CompareTo(predictTo.Date) <= 0)
                {
                    var result = new PredicResults();
                    _manager.GetInputData(index - INPUT_TUPLES, present);
                   // _manager.GetOutputData(index - INPUT_TUPLES, actualOutput);
                    var data = new BasicNeuralData(present);
                    var predict = _network.Compute(data);
                    //result.ActualClose = actualOutput[0] * (_manager.GetMax((int)PredicInputIndexe.CloseIndex) - _manager.GetMin((int)PredicInputIndexe.CloseIndex)) + _manager.GetMin((int)PredicInputIndexe.CloseIndex);
                    result.PredictedClose = predict[0] * (_manager.GetMax((int)PredicInputIndexe.CloseIndex) - _manager.GetMin((int)PredicInputIndexe.CloseIndex)) + _manager.GetMin((int)PredicInputIndexe.CloseIndex);
                    //result.ActualPir = actualOutput[1] * (_manager.MaxPrimeRate - _manager.MinPrimeRate) + _manager.MinPrimeRate;
                    //result.PredictedPir = predict[1] * (_manager.MaxPrimeRate - _manager.MinPrimeRate) + _manager.MinPrimeRate;
                    //result.ActualDow = actualOutput[2] * (_manager.MaxDow - _manager.MinDow) + _manager.MinDow;
                    //result.PredictedDow = predict[2] * (_manager.MaxDow - _manager.MinDow) + _manager.MinDow;
                    //result.ActualNasdaq = actualOutput[3] * (_manager.MaxNasdaq - _manager.MinNasdaq) + _manager.MinNasdaq;
                    //result.PredictedNasdaq = predict[3] * (_manager.MaxNasdaq - _manager.MinNasdaq) + _manager.MinNasdaq;
                    result.Date = sample.Date;
                    ErrorCalculation error = new ErrorCalculation();
                    //error.UpdateError(actualOutput, predict);
                    //result.Error = error.CalculateRMS();
                    //result.Error = result.ActualClose - result.PredictedClose; 
                    result.Error = 0;
                    result.ActualClose = 0;
                    results.Add(result);
                }
                index++;
            }
            return results;
        }
        public PredicResults Predict()
        {
            
            double[] present = new double[INPUT_TUPLES * INDEXES_TO_CONSIDER];
            double[] actualOutput = new double[OUTPUT_SIZE];
            int index = 0;
            index = _manager.Samples.Count;
                    var result = new PredicResults();
                    _manager.GetInputData(index - INPUT_TUPLES, present);
                    //_manager.GetOutputData(index - INPUT_TUPLES, actualOutput);
                    var data = new BasicNeuralData(present);
                    var predict = _network.Compute(data);
                    //result.ActualClose = actualOutput[0] * (_manager.GetMax((int)PredicInputIndexe.CloseIndex) - _manager.GetMin((int)PredicInputIndexe.CloseIndex)) + _manager.GetMin((int)PredicInputIndexe.CloseIndex);
                    result.PredictedClose = predict[0] * (_manager.GetMax((int)PredicInputIndexe.CloseIndex) - _manager.GetMin((int)PredicInputIndexe.CloseIndex)) + _manager.GetMin((int)PredicInputIndexe.CloseIndex);
                    //result.ActualPir = actualOutput[1] * (_manager.MaxPrimeRate - _manager.MinPrimeRate) + _manager.MinPrimeRate;
                    //result.PredictedPir = predict[1] * (_manager.MaxPrimeRate - _manager.MinPrimeRate) + _manager.MinPrimeRate;
                    //result.ActualDow = actualOutput[2] * (_manager.MaxDow - _manager.MinDow) + _manager.MinDow;
                    //result.PredictedDow = predict[2] * (_manager.MaxDow - _manager.MinDow) + _manager.MinDow;
                    //result.ActualNasdaq = actualOutput[3] * (_manager.MaxNasdaq - _manager.MinNasdaq) + _manager.MinNasdaq;
                    //result.PredictedNasdaq = predict[3] * (_manager.MaxNasdaq - _manager.MinNasdaq) + _manager.MinNasdaq;
                    result.Date = _manager.GetDateTime(index-1).AddDays(3);
                    //ErrorCalculation error = new ErrorCalculation();
                    //error.UpdateError(actualOutput, predict);
                    //result.Error = error.CalculateRMS();
                    result.Error = 0;
                    
               
            return result;
        }
    }
}
