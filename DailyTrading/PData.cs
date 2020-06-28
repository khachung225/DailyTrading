using System;

namespace DailyTrading
{
    public class PData : IComparable<PData>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amount">SP index</param>
        /// <param name="date">Date of the index</param>
        public PData(double amount, DateTime date)
        {
            Amount = amount;
            Date = date;
        }

        #region Properties

        public double Amount { get; set; }

        /// <summary>
        /// Corresponding date
        /// </summary>
        public DateTime Date { get; set; }

        #endregion

        /// <summary>
        /// Compare the indexes by date
        /// </summary>
        /// <param name="other">Other PData index</param>
        /// <returns>Comparison result</returns>
        public int CompareTo(PData other)
        {
            return Date.CompareTo(other.Date);
        }
    }

   
}
