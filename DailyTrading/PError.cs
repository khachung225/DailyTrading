using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FinancialMarketPredictor.Utilities;
using ZedGraph;

namespace DailyTrading
{
    public partial class PError : Form
    {
        public PError()
        {
            InitializeComponent();
            GraphInit();
        }

        private void Error_Load(object sender, EventArgs e)
        {
           
        }
        private void GraphInit( )
        {
            GraphPane myPane1 = DoThi_Error.GraphPane; // Khai báo sửa dụng Graph loại GraphPane;

            myPane1.Title.Text = "Đồ thị giá trị lỗi Trainning";
            myPane1.XAxis.Title.Text = "Số lần lặp";
            myPane1.YAxis.Title.Text = "Giá trị lỗi";
            // Định nghĩa list để vẽ đồ thị. Để các bạn hiểu rõ cơ chế làm việc ở đây khai báo 2 list điểm <=> 2 đường đồ thị
            RollingPointPairList list6_1 = new RollingPointPairList(1000);
           // RollingPointPairList list6_2 = new RollingPointPairList(1000);
            // dòng dưới là định nghĩa curve để vẽ.
            myPane1.AddCurve("Giá trị đo", list6_1, Color.Red, SymbolType.Diamond);
          //  myPane1.AddCurve("Giá trị tính toán bởi mạng", list6_2, Color.Blue, SymbolType.Star);

            // Định hiện thị cho trục thời gian (Trục X)
            //myPane1.XAxis.Scale.Min = 0;
            //myPane1.XAxis.Scale.Max = 100000;
            //myPane1.XAxis.Scale.MinorStep = 1;
            //myPane1.XAxis.Scale.MajorStep = 1;
            //myPane1.XAxis.Type = AxisType.Date;
            //myPane1.XAxis.Scale.Min = new XDate(_predictFrom);  // We want to use time from now
            //myPane1.XAxis.Scale.Max = new XDate(_predictTo);  // to 5 minutes per default
            //myPane1.XAxis.Scale.MinorUnit = DateUnit.Day;         // set the minimum x unit to time/seconds
            //myPane1.XAxis.Scale.MajorUnit = DateUnit.Day;         // set the maximum x unit to time/minutes
            //myPane1.XAxis.Scale.Format = "MM/dd/yyyy";
            // Gọi hàm xác định cỡ trục
            myPane1.AxisChange();

        }

        public void DrawGraph(List<MyError> results)
        {
            //ve gia tri
            LineItem curve2_1 = DoThi_Error.GraphPane.CurveList[0] as LineItem;
           // LineItem curve2_2 = DoThi.GraphPane.CurveList[1] as LineItem;

            //init do thi.

            // Get the PointPairList
            IPointListEdit list21 = curve2_1.Points as IPointListEdit;
           
            list21.Clear();

            DoThi_Error.AxisChange();
            DoThi_Error.Invalidate();
            int i = 0;
            foreach (var item in results)
            {
                
                list21.Add(item.index, item.value);
                
                // đoạn chương trình thực hiện vẽ đồ thị
                Scale xScale = DoThi_Error.GraphPane.XAxis.Scale;
                i++;
            }
            
            // Vẽ đồ thị
            DoThi_Error.AxisChange();
            // Force a redraw
            DoThi_Error.Invalidate();

            //if (AppGlobol.IsAutoRun)
            //    DoThi_Error.GetImage().Save(AppGlobol.FolderPath + "/Error.png");
        }

    }
}
