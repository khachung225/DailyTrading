using DatabaseDAL.Entity;
using DatabaseDAL.EntitySql;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseDAL.DAO
{
    public class TickerHDBDAO
    {
        private TickerHDBSql entitySql = new TickerHDBSql();
        private string connectionstring = "";
         
        public TickerHDBDAO (string connecstring)
        {
            connectionstring = connecstring;
        }
        public int Insert (TickerHDB entity)
        {
            try
            {
                var cmd = entitySql.GetSqlCommandForInsert(entity);
                    cmd.Connection = new Npgsql.NpgsqlConnection(connectionstring);
             
                    cmd.ExecuteNonQuery();
                
            }catch
            {
                return 0;
            }
            return 1;
        }

        public List<TickerHDB> SelectALL()
        {
            try
            {
                var cmd = entitySql.GetSqlCommandForGetAll();
                cmd.Connection = new Npgsql.NpgsqlConnection(connectionstring);
                var reader = cmd.ExecuteReader();
                return entitySql.PopulateBusinessObjectFromReader(reader);

            }
            catch
            {
                 
            }
            return new List<TickerHDB>(); 
        }
    }
}
