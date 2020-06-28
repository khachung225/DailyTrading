using System.Collections.Generic;
using System.Data;
using DatabaseDAL.Entity;
using Npgsql;

namespace DatabaseDAL.EntitySql
{

    
    /// <summary>
    /// Data access layer class for ClassInfo
    /// </summary>
    public class TickerHDBSql: System.IDisposable
    {

        private string Insertsql = "";
        private string Updatesql = "";
        private string Deletesql = "";
        private string Selectsql = "";

        #region Constructor

        /// <summary>
        /// Class constructor
        /// </summary>
        public TickerHDBSql()
        {
            Init();
        }

        #endregion

        #region Init

        /// <summary>
        /// initial class
        /// </summary>
        private void Init()
        {
            Insertsql = "dbo.[TickerHDB_Insert]";
            Updatesql = "dbo.[TickerHDB_Update]";
            Deletesql = "dbo.[TickerHDB_DeleteByPrimaryKey]";
            Selectsql = "dbo.[TickerHDB_SelectAll]";
        }

        #endregion
        #region Public override Methods

        /// <summary>
        /// Create a command for action insert
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public  NpgsqlCommand GetSqlCommandForInsert(TickerHDB baseEntity)
        {
            var sqlCommand = new NpgsqlCommand { CommandType = System.Data.CommandType.StoredProcedure,CommandText = Insertsql };

            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerHDB;
                if (businessObject != null)
                {

                    sqlCommand.Parameters.AddWithValue("@ClassInfoId", NpgsqlTypes.NpgsqlDbType.Bigint, 8, businessObject.Day);

                   // sqlCommand.ExecuteNonQuery();
                }
            }
            return sqlCommand;
        }
        /// <summary>
        /// Update Key after excutant insert to DB.
        /// </summary>
        /// <param name="businessObject">business object</param>
        /// <returns>true for successfully updated</returns>
        public  TickerHDB UpdateEntityId(TickerHDB baseEntity, NpgsqlCommand sqlCommand)
        {
            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerHDB;

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
        public NpgsqlCommand GetSqlCommandForUpdate(TickerHDB baseEntity)
        {
            var sqlCommand = new NpgsqlCommand { CommandType = CommandType.StoredProcedure, CommandText = Updatesql };
            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerHDB;

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
        public NpgsqlCommand GetSqlCommandForDelete(TickerHDB baseEntity)
        {
            var sqlCommand = new NpgsqlCommand { CommandType = CommandType.StoredProcedure };
            if (baseEntity != null)
            {
                var businessObject = baseEntity as TickerHDB;
                if (businessObject != null)
                {

                   

                }
            }
            return sqlCommand;
        }

        public NpgsqlCommand GetSqlCommandForGetAll()
        {
            var sqlCommand = new NpgsqlCommand { CommandType = CommandType.StoredProcedure, CommandText= Selectsql};
            
            return sqlCommand;
        }

        /// <summary>
        /// Populate business objects from the data reader
        /// </summary>
        /// <param name="dataReader">data reader</param>
        /// <returns>list of ClassInfo</returns>
        public List<TickerHDB> PopulateBusinessObjectFromReader(IDataReader dataReader)
        {
            var list = new List<TickerHDB>();

            while (dataReader.Read())
            {
                var businessObject = new TickerHDB();
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
        internal void PopulateBusinessObjectFromReader(TickerHDB businessObject, IDataReader dataReader)
        {           

           
                businessObject.Day = dataReader.GetDateTime(dataReader.GetOrdinal(TickerHDB.TickerHDBFields.Day.ToString()));
          
                    
            if (!dataReader.IsDBNull(dataReader.GetOrdinal(TickerHDB.TickerHDBFields.Open.ToString())))
            {
                businessObject.Open = dataReader.GetDecimal(dataReader.GetOrdinal(TickerHDB.TickerHDBFields.Open.ToString()));
            }

           
            businessObject.Hight = dataReader.GetDecimal(dataReader.GetOrdinal(TickerHDB.TickerHDBFields.Hight.ToString()));
            businessObject.Hight = dataReader.GetDecimal(dataReader.GetOrdinal(TickerHDB.TickerHDBFields.Hight.ToString()));
            businessObject.Hight = dataReader.GetDecimal(dataReader.GetOrdinal(TickerHDB.TickerHDBFields.Hight.ToString()));
            businessObject.Hight = dataReader.GetDecimal(dataReader.GetOrdinal(TickerHDB.TickerHDBFields.Hight.ToString()));
        }

        public void Dispose()
        {
             
        }



        #endregion

    }
}
