using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Windows.Forms;
using DatabaseDAL.Entity;
using DatabaseDAL;

namespace GetTickerPrice
{
    public partial class LoadDataFromWeb : Form
    {
        public LoadDataFromWeb()
        {
            InitializeComponent();

            webBrowser1.ScriptErrorsSuppressed = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            webBrowser1.AllowNavigation = true;
            webBrowser1.Navigate(textBox1.Text);             
        }
        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.Host.Equals("")) return;

            try
            {
                var browser = sender as WebBrowser;
                HtmlElementCollection textel = browser.Document.Body.GetElementsByTagName("table");
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(browser.DocumentText);
                var node = doc.DocumentNode.Descendants("table");
                var listData = new List<TickerBase>();
                var ticker = GetTicker(e.Url.ToString());
                foreach (var mynode in node)
                {
                    if (mynode.Attributes["class"] != null && mynode.Attributes["class"].Value == "dataTable")
                    {
                        foreach (var mytr in mynode.ChildNodes)
                        {
                            if (mytr.Name == "tr")
                            {
                                int i = 0;
                                var mydata = new TickerBase();

                                foreach (var mytd in mytr.ChildNodes)
                                {
                                    if (mytd.Name == "td")
                                    {

                                        var data = mytd.InnerText.Trim();
                                        //ngay
                                        if (i == 0)
                                        {
                                            mydata.Day = DateTime.ParseExact(data, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        }
                                        //close
                                        if (i == 2)
                                        { mydata.Open = Double.Parse(data); }
                                        //v
                                        if (i == 3)
                                        { mydata.Hight = Double.Parse(data); }
                                        //o
                                        if (i == 4)
                                        { mydata.Low = Double.Parse(data); }
                                        //h
                                        if (i == 5)
                                        { mydata.Close = Double.Parse(data); }
                                        //l
                                        if (i == 8) { mydata.Volume = Double.Parse(data.Replace('.', ',')); }
                                        i++;
                                    }
                                }
                                if (mydata.IsHasData())
                                    listData.Add(mydata);
                            }

                        }
                    }
                }

                //lưu vao DB
                var dbManager = new DBManager();
                dbManager.InsertTicker(ticker, listData);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }
        }
        private string GetTicker(string url)
        {
            //https://www.stockbiz.vn/Stocks/TCB/HistoricalQuotes.aspx
            return url.Substring(31,3).ToUpper();
        }
        private string GetPage(string url)
        {
            //https://www.stockbiz.vn/Stocks/TCB/HistoricalQuotes.aspx
            //https://www.cophieu68.vn/historyprice.php?id=TCB
            System.Net.WebClient client = new System.Net.WebClient();
            byte[] data = client.DownloadData(url);
            String html = System.Text.Encoding.UTF8.GetString(data);
            
            return html;
        }
    }
}
