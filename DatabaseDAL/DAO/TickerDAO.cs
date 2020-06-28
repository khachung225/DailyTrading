using DatabaseDAL.Entity;
using DatabaseDAL.EntitySql;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseDAL.DAO
{
    public class TickerDAO: IDisposable
    {
        private TickerSql entitySql;
        private string connectionstring = "";
        
        public TickerDAO(string connecstring, string tickerbase)
        {
            connectionstring = connecstring;
            entitySql = new TickerSql(tickerbase);
        }

        public void Dispose()
        {
            entitySql.Dispose();
        }

        public int Insert (TickerBase entity)
        {
            var conn = new Npgsql.NpgsqlConnection(connectionstring);
            try
            {
               
                conn.Open();
                var cmd = entitySql.GetSqlCommandForInsert(entity);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
               
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Console.WriteLine(msg.ToString());
              
            }
            conn.Close();
            return 1;
        }

        public List<TickerBase> SelectALL(string ticher)
        {
            var conn = new Npgsql.NpgsqlConnection(connectionstring);
            try
            {
                conn.Open();
                var cmd = entitySql.GetSqlCommandForGetAll(ticher);
                cmd.Connection = conn;
                var reader = cmd.ExecuteReader();
                return entitySql.PopulateBusinessObjectFromReader(reader);

            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Console.WriteLine(msg.ToString());
            }
            conn.Close();
            return new List<TickerBase>(); 
        }
        public int CreateTable(string ticher)
        {
            var conn = new Npgsql.NpgsqlConnection(connectionstring);
            try
            {
                conn.Open();
                var cmd = entitySql.GetSqlCommandForCreateDB(ticher);
                cmd.Connection = conn;

                cmd.ExecuteNonQuery();
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Console.WriteLine(msg.ToString());
            }
            conn.Close();
            return 1;
        }
        public int SetOwner(string ticher)
        {
            var conn = new Npgsql.NpgsqlConnection(connectionstring);
            try
            {
                conn.Open();
                var cmd = entitySql.GetSqlCommandForSetOWner(ticher);
                cmd.Connection = conn;

                cmd.ExecuteNonQuery();
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Console.WriteLine(msg.ToString());
            }
            conn.Close();
            return 1;
        }
        public bool IsTableExisted(string ticher)
        {
            var conn = new Npgsql.NpgsqlConnection(connectionstring);
            try
            {
                conn.Open();
                var cmd = entitySql.GetSqlCommandForExistTicker(ticher);
                cmd.Connection = conn;

                var reader = cmd.ExecuteReader();
                return entitySql.GetValueCheckTableExisted(reader);
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Console.WriteLine(msg.ToString());
            }
            conn.Close();
            return false;
        }

        private int DropTable(string ticher)
        {
            var conn = new Npgsql.NpgsqlConnection(connectionstring);
            try
            {
                conn.Open();
                var cmd = entitySql.GetSqlCommandForDropDB(ticher);
                cmd.Connection = conn;

                cmd.ExecuteNonQuery();
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                Console.WriteLine(msg.ToString());
            }
            conn.Close();
            return 1;
        }
    }
}
