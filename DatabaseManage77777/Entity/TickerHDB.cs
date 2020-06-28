using System;

namespace DatabaseDAL.Entity
{
    public class TickerHDB
    {
        #region InnerClass
        public enum TickerHDBFields
        {
            Day,
            Open,
            Hight,
            Low,
            Close,
            Volumn            
        }
        #endregion

        #region Properties

        public DateTime Day { get; set; }
        public decimal Open { get; set; }
        public decimal Hight { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volumn { get; set; }



        #endregion
 
    }
}
