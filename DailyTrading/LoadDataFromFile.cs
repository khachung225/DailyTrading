using DatabaseDAL.DAO;
using DatabaseDAL.Entity;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyTrading
{
    public partial class LoadDataFromFile : Form
    {
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        public LoadDataFromFile()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {

                // PostgeSQL-style connection string
                string connstring = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};",
                    "127.0.0.1", "5433", "lemon",
                    "admin", "DailyTrading");

                var dao = new TickerDAO(connstring, "HDB");
                var ticker = new TickerBase { Day = new DateTime(2019,08,23), Open =45.6, Hight =45.6, Low = 45.6, Close =45.6,Volume=4444 };

                //dao.Insert(ticker);
                var data = dao.SelectALL("HDB");
                if (data.Count() > 0)
                    Console.WriteLine("lay dl:" + data.Count());

            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                MessageBox.Show(msg.ToString());
                throw;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                var listdata = new List<TickerBase>();
            var fileName = textBox2.Text;
            var tickerbase = textBox1.Text;
                var lines = File.ReadAllLines(fileName);
            foreach(var line in lines)
            {
                if (line.Length < 2) continue;
                //tach value/
                var listvalues = line.Split(',');
                if (listvalues[0].Contains("DATE")) continue;

                var ticker = new TickerBase();
                ticker.Day = DateTime.ParseExact(listvalues[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                ticker.Close = float.Parse(listvalues[1]);
                ticker.Open = float.Parse(listvalues[3]);
                ticker.Hight = float.Parse(listvalues[4]);
                ticker.Low = float.Parse(listvalues[5]);
                ticker.Volume = float.Parse(listvalues[6]);

                listdata.Add(ticker);
            }
            string connstring = String.Format("Server={0};Port={1};" +
                   "User Id={2};Password={3};Database={4};",
                   "127.0.0.1", "5433", "lemon",
                   "admin", "DailyTrading");

                var dao = new TickerDAO(connstring, tickerbase);
                var listTickerLoad = new List<TickerBase>();
                //check db da tao chua
                var isexistticker = dao.IsTableExisted(tickerbase);
                if(isexistticker)
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
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                MessageBox.Show(msg.ToString());
                throw;
            }
        }
    }
}
