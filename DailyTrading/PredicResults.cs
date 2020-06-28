
// ciumac.sergiu@gmail.com
using System;

namespace DailyTrading
{
    /// <summary>
    /// Prediction results
    /// </summary>
    public class PredicResults
    {
        #region Properties
        /// <summary>
        /// Date of the prediction
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Actual percentage move
        /// </summary>
        public double ActualClose {get; set; }
   
        /// <summary>
        /// Predicted percentage move
        /// </summary>
        public double PredictedClose {get; set; }

        /// <summary>
        /// Error between predicted and actual values
        /// </summary>
        public double Error { get; set; }

        #endregion
    }
}
