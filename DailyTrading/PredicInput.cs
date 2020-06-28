// ciumac.sergiu@gmail.com
using System;
using System.Collections.Generic;

namespace DailyTrading
{
    public enum PredicInputIndexe
    {
        OpenIndex,
        HighIndex,
        LowIndex,
        CloseIndex,
        VolumeIndex,
        VNIndex
    }

    /// <summary>
    /// All four financial indexes are stitched in this class
    /// </summary>
    public class PredicInput : IComparable<PredicInput>
    {
        public double DefauValue = -1.0;

      
        /// <summary>
        /// Date with corresponding S&P500 Adj Close and Prime Interest PrimeInterestRate
        /// </summary>
        public DateTime Date { get; set; }

        private Dictionary<int,double> _data = new Dictionary<int, double>(); 

        public bool IsSetValue(int index)
        {
            if (_data.ContainsKey(index))
                return true;
            return false;
        }
        public double GetValue(int index)
        {
            if (_data.ContainsKey(index))
                return _data[index];
            return DefauValue;
        }
        public void SetValue(int index, double value)
        {
            _data[index] = value;
        }
        #region IComparable<FinancialIndexes> Members
        /// <summary>
        /// Compare by date
        /// </summary>
        /// <param name="other">Other financial pair</param>
        /// <returns></returns>
        public int CompareTo(PredicInput other)
        {
            return Date.Date.CompareTo(other.Date.Date);
        }

        #endregion
    }
}
