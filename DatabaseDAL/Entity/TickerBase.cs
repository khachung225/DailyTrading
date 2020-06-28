using System;

namespace DatabaseDAL.Entity
{
    public class TickerBase
    {
        #region InnerClass
        public enum TickerBaseFields
        {
            Day,
            Open,
            Hight,
            Low,
            Close,
            Volume
        }
        #endregion

        #region Properties

        public DateTime Day { get; set; }
        public Double Open { get; set; }
        public Double Hight { get; set; }

        public Double Low { get; set; }

        public Double Close { get; set; }

        public Double Volume { get; set; }



        #endregion
 
        public bool IsHasData()
        { return Day != null && Close > 0; }
    }
}
