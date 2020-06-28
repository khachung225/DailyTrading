using System.Collections.Generic;
using System.Data;
using DatabaseDAL.Entity;
using Npgsql;

namespace DatabaseDAL.EntitySql
{

    
    /// <summary>
    /// Data access layer class for ClassInfo
    /// </summary>
    public class TickerSql: System.IDisposable
    {

        private string Insertsql = "";
        private string Updatesql = "";
        private string Deletesql = "";
        private string Selectsql = "";
        private string CreateDB = "";

        #region Constructor

        /// <summary>
        /// Class constructor
        /// </summary>
        public TickerSql(string tickerbase)
        {
            Init(tickerbase);
        }

        #endregion

        #region Init

        /// <summary>
        /// initial class
        /// </summary>
        private void Init(string tickerbase)
        {
            Insertsql = "INSERT INTO public.\"Ticker" + tickerbase + "\"( \"Day\", \"Open\", \"Hight\", \"Low\", \"Close\", \"Volume\") VALUES(:Day, :Open, :Hight, :Low, :Close, :Volume); ";////dbo.[TickerHDB_Insert]
            Updatesql = "dbo.[TickerHDB_Update]";
            Deletesql = "dbo.[TickerHDB_DeleteByPrimaryKey]";
            Selectsql = "SELECT \"Day\", \"Open\", \"Hight\", \"Low\", \"Close\", \"Volume\" FROM public.\"Ticker" + tickerbase + "\"";
            CreateDB = "";
        }

        #endregion
        #region Public override Methods

        public NpgsqlCommand GetSqlCommandForCreateDB(string tickerbase)
        {
            var command = "CREATE TABLE public.\"Ticker"+ tickerbase + "\" ( \"Day\" date NOT NULL, \"Open\" real NOT NULL, \"Hight\" real NOT NULL, \"Low\" real NOT NULL, \"Close\" real NOT NULL, \"Volume\" real NOT NULL, CONSTRAINT \"Day" + tickerbase.Trim() + "_pkey\" PRIMARY KEY(\"Day\")) WITH( OIDS = FALSE) TABLESPACE pg_default; ";
            var sqlCommand = new NpgsqlCommand { CommandType = System.Data.CommandType.Text, CommandText = command };

           
            return sqlCommand;
        }

        public NpgsqlCommand GetSqlCommandForDropDB(string tickerbase)
        {
            var commandtext = "DROP TABLE public.\"Ticker" + tickerbase + "\";";
            var sqlCommand = new NpgsqlCommand { CommandType = System.Data.CommandType.Text, CommandText = commandtext };

            return sqlCommand;
        }
        public NpgsqlCommand GetSqlCommandForSetOWner(string tickerbase)
        {

            var commandtext = "ALTER TABLE public.\"Ticker"+ tickerbase + "\" OWNER to postgres; ";
            var sqlCommand = new NpgsqlCommand { CommandType = System.Data.CommandType.Text, CommandText = commandtext };

           
            return sqlCommand;
        }

        public NpgsqlCommand GetSqlCommandForExistTicker(string tickerbase)
        {

            var commandtext = "SELECT EXISTS ( SELECT 1 FROM   pg_tables  WHERE tablename = 'Ticker" + tickerbase + "' ) as Isexisted;";
            var sqlCommand = new NpgsqlCommand { CommandType = System.Data.CommandType.Text, CommandText = commandtext };


            return sqlCommand;
        }

        /// <summary>
        /// Create a command for action insert
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public  NpgsqlCommand GetSqlCommandForInsert(TickerBase baseEntity)
        {
            var sqlCommand = new NpgsqlCommand { CommandType = System.Data.CommandType.Text,CommandText = Insertsql };

            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerBase;
                if (businessObject != null)
                {

                    sqlCommand.Parameters.AddWithValue("Day", NpgsqlTypes.NpgsqlDbType.Date, businessObject.Day);
                    sqlCommand.Parameters.AddWithValue("Open", NpgsqlTypes.NpgsqlDbType.Real, businessObject.Open);
                    sqlCommand.Parameters.AddWithValue("Hight", NpgsqlTypes.NpgsqlDbType.Real,  businessObject.Hight);
                    sqlCommand.Parameters.AddWithValue("Low", NpgsqlTypes.NpgsqlDbType.Real,  businessObject.Low);
                    sqlCommand.Parameters.AddWithValue("Close", NpgsqlTypes.NpgsqlDbType.Real, businessObject.Close);
                    sqlCommand.Parameters.AddWithValue("Volume", NpgsqlTypes.NpgsqlDbType.Real, businessObject.Volume);
                }
            }
            return sqlCommand;
        }
        /// <summary>
        /// Update Key after excutant insert to DB.
        /// </summary>
        /// <param name="businessObject">business object</param>
        /// <returns>true for successfully updated</returns>
        public  TickerBase UpdateEntityId(TickerBase baseEntity, NpgsqlCommand sqlCommand)
        {
            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerBase;

                if (businessObject != null)
                {

                }
                return businessObject;
            }
            return null;
        }

        

        /// <summary>
        /// Create a command for action Update
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public NpgsqlCommand GetSqlCommandForUpdate(TickerBase baseEntity)
        {
            var sqlCommand = new NpgsqlCommand { CommandType = CommandType.StoredProcedure, CommandText = Updatesql };
            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerBase;

                if (businessObject != null)
                {

                   

                }
            }
            return sqlCommand;
        }

        /// <summary>
        /// Create a command for action Delete
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public NpgsqlCommand GetSqlCommandForDelete(TickerBase baseEntity)
        {
            var sqlCommand = new NpgsqlCommand { CommandType = CommandType.StoredProcedure };
            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerBase;
                if (businessObject != null)
                {

                   

                }
            }
            return sqlCommand;
        }

        public NpgsqlCommand GetSqlCommandForGetAll(string ticher)
        {
            var selectsql = "SELECT \"Day\", \"Open\", \"Hight\", \"Low\", \"Close\", \"Volume\" FROM public.\"Ticker" + ticher + "\"";
            var sqlCommand = new NpgsqlCommand { CommandType = CommandType.Text, CommandText= selectsql };
            
            return sqlCommand;
        }

        /// <summary>
        /// Populate business objects from the data reader
        /// </summary>
        /// <param name="dataReader">data reader</param>
        /// <returns>list of ClassInfo</returns>
        public List<TickerBase> PopulateBusinessObjectFromReader(IDataReader dataReader)
        {
            var list = new List<TickerBase>();

            while (dataReader.Read())
            {
                var businessObject = new TickerBase();
                PopulateBusinessObjectFromReader(businessObject, dataReader);
                list.Add(businessObject);
            }

            return list;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Populate business object from data reader
        /// </summary>
        /// <param name="businessObject">business object</param>
        /// <param name="dataReader">data reader</param>
        internal void PopulateBusinessObjectFromReader(TickerBase businessObject, IDataReader dataReader)
        {           

           
                businessObject.Day = dataReader.GetDateTime(dataReader.GetOrdinal(TickerBase.TickerBaseFields.Day.ToString()));
          
                    
            if (!dataReader.IsDBNull(dataReader.GetOrdinal(TickerBase.TickerBaseFields.Open.ToString())))
            {
                businessObject.Open = dataReader.GetFloat(dataReader.GetOrdinal(TickerBase.TickerBaseFields.Open.ToString()));
            }

           
            businessObject.Hight = dataReader.GetFloat(dataReader.GetOrdinal(TickerBase.TickerBaseFields.Hight.ToString()));
            businessObject.Low = dataReader.GetFloat(dataReader.GetOrdinal(TickerBase.TickerBaseFields.Low.ToString()));
            businessObject.Close = dataReader.GetFloat(dataReader.GetOrdinal(TickerBase.TickerBaseFields.Close.ToString()));
            businessObject.Volume = dataReader.GetFloat(dataReader.GetOrdinal(TickerBase.TickerBaseFields.Volume.ToString()));
        }
        public bool GetValueCheckTableExisted(IDataReader dataReader)
        {
            var ischeck = false;
            while (dataReader.Read())
            {

                ischeck = dataReader.GetBoolean(dataReader.GetOrdinal("Isexisted"));
            }

            return ischeck;
        }

        public void Dispose()
        {
             
        }



        #endregion

    }
}
