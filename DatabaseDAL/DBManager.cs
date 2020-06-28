using DatabaseDAL.DAO;
using DatabaseDAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseDAL
{
   public class DBManager
    {
       private  string connstring = String.Format("Server={0};Port={1};" +
                  "User Id={2};Password={3};Database={4};",
                  "127.0.0.1", "5433", "lemon",
                  "admin", "DailyTrading");

        public bool InsertTicker(string tickerbase, List<TickerBase> listdata)
        {
            try
            {
                var dao = new TickerDAO(connstring, tickerbase);
                var listTickerLoad = new List<TickerBase>();
                //check db da tao chua
                var isexistticker = dao.IsTableExisted(tickerbase);
                if (isexistticker)
                {
                    listTickerLoad.AddRange(dao.SelectALL(tickerbase));
                }
                else
                {
                    //taoj moi tabke
                    dao.CreateTable(tickerbase);
                    dao.SetOwner(tickerbase);
                }
                //get all data check trung

                //insert to db
                foreach (var entity in listdata)
                {
                    if (listTickerLoad.Contains(entity))
                        continue;
                    dao.Insert(entity);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());                
            }
            return true;
        }
        public List<TickerBase> GetAllTicker(string tickerbase)
        {
            var listTickerLoad = new List<TickerBase>();
            //check db da tao chua
            var dao = new TickerDAO(connstring, tickerbase);
            var isexistticker = dao.IsTableExisted(tickerbase);
            if (isexistticker)
            {
                listTickerLoad.AddRange(dao.SelectALL(tickerbase));
            }
            return listTickerLoad;
        }
    }
}
