// ciumac.sergiu@gmail.com
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Permissions;
using ZedGraph;
using DatabaseDAL.DAO;
using DatabaseDAL.Entity;
using System.Linq;

namespace DailyTrading
{
    public partial class WinFinancialMarketPredictor : Form
    {
        #region Private member fields
        
        /// <summary>
        /// Default path to S&P csv
        /// </summary>
        private string _pathToSp = "S&P500Index_1104.csv";

        /// <summary>
        /// Default path to Prime interest rates csv
        /// </summary>
        private string _pathToEURUSD = "EUR_USD_1990.csv";
        private string _pathToUSDJPY = "USD-JPY_1367.csv";

        /// <summary>
        /// Default path to Nasdaq indexes csv
        /// </summary>
        private string _pathToNasdaq = "nasdaq.csv";

        /// <summary>
        /// Default path to Dow indexes csv
        /// </summary>
        private string _pathToDow = "DOWI_index_1104.csv";

        private string _pathToCommodity = "KCZ13_718.csv";

        private string _pathToXAUUSD = "XAU-USD.csv";
        private string _pathToNikkie = "NKY_index_1074.csv";
   
        /// <summary>
        /// Predictor
        /// </summary>
        private PredictIndicators _predictor;

        /// <summary>
        /// Predict the percentage movement from a specific date
        /// </summary>
        private readonly DateTime _predictFrom = new DateTime(2001,01,01);// DateTime.ParseExact("2001/01/01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Predict the percentage movement to a specific date
        /// </summary>
        private readonly DateTime _predictTo = new DateTime(2001, 01, 01);//DateTime.ParseExact("2001/01/01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Learn from a specific date
        /// </summary>
        private readonly DateTime _learnFrom = new DateTime(2001, 01, 01);// DateTime.ParseExact("2001/01/01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Learn until a specific date
        /// </summary>
        private readonly DateTime _learnTo = new DateTime(2001, 01, 01);//DateTime.ParseExact("2001/01/01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Maximum date that can be specified for training and predicting, specified in the AppConfig
        /// </summary>
        private  DateTime _maxDate;

        /// <summary>
        /// Minimum date that can be specified for training and predicting, specified in the AppConfig
        /// </summary>
        private  DateTime _minDate;

        /// <summary>
        /// Default parameter for hidden layers
        /// </summary>
        private int _hiddenLayers = 6;

        /// <summary>
        /// Default parameter for hidden units
        /// </summary>
        private int _hiddenUnits = 12;

        /// <summary>
        /// Check if there is a need in reloading the files
        /// </summary>
        private bool _reloadFiles = false;

        private static System.Timers.Timer aTimer;

        private List<PredicInput> listPredicInput = new List<PredicInput>();
        #endregion

        /// <summary>
        /// Public parameter less constructor
        /// </summary>
        public WinFinancialMarketPredictor()
        {
            InitializeComponent();
            _btnStop.Enabled = false;
            _btnExport.Enabled = false;
            try
            {
                _maxDate = DateTime.Now;
                _minDate = new DateTime(2001, 01, 01);//DateTime.ParseExact("2001/01/01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                _maxDate = DateTime.Now;                        /*Maximum specified in the csv files*/
                _minDate = new DateTime(2001, 01, 01);// DateTime.ParseExact("2001/01/01", "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            }

            /*Set some reasonable default values*/
            _dtpTrainFrom.Value = _learnFrom;
            _dtpTrainUntil.Value = _learnTo;
            _dtpPredictFrom.Value = _predictFrom;
            _dtpPredictTo.Value = _predictTo;

            _dtpTrainFrom.MaxDate = _dtpTrainUntil.MaxDate = _dtpPredictFrom.MaxDate = _dtpPredictTo.MaxDate = _maxDate;
            _dtpTrainFrom.MinDate = _dtpTrainUntil.MinDate = _dtpPredictFrom.MinDate = _dtpPredictTo.MinDate = _minDate;

            _nudHiddenLayers.Value = _hiddenLayers;
            _nudHiddenUnits.Value = _hiddenUnits;
                                 
           
        }
       
        /// <summary>
        /// Load the form
        /// </summary>
        private void WinFinancialMarketPredictorLoad(object sender, EventArgs e)
        {
            
            GraphInit(DoThi_GiaiTri);
            
             //aTimer.Enabled = true;
            
        }


         

        /// <summary>
        /// Training callback, invoked at each iteration
        /// </summary>
        /// <param name="epoch">Epoch number</param>
        /// <param name="error">Current error</param>
        /// <param name="algorithm">Training algorithm</param>
        private void TrainingCallback(int epoch, double error, TrainingAlgorithm algorithm)
        {
            Invoke(addAction, new object [] {epoch, error, algorithm, _dgvTrainingResults});

        }

        /// <summary>
        /// Start training button pressed
        /// </summary>
        private void BtnStartTrainingClick(object sender, EventArgs e)
        {
            if (_dgvTrainingResults.Rows.Count != 0)
                _dgvTrainingResults.Rows.Clear();

            if (_predictor == null)
            {
                _reloadFiles = false;
               
            }

            DateTime trainFrom = _dtpTrainFrom.Value;
            DateTime trainTo = _dtpTrainUntil.Value;

            if (trainFrom > trainTo)
            {
                MessageBox.Show("thong bao", "Tham so khong dung", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dtpTrainFrom.Focus();
                return;
            }
            FadeControls(true);
            if (_predictor == null)
            {                
                 Cursor = Cursors.WaitCursor;
                _hiddenLayers = (int)_nudHiddenLayers.Value;
                _hiddenUnits = (int)_nudHiddenUnits.Value;
                try
                {
                    _predictor = new PredictIndicators(listPredicInput, _hiddenUnits, _hiddenLayers);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _predictor = null;
                    return;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            else if (_reloadFiles) /*Reload training sets*/
            {
              
                _dtpTrainFrom.MinDate = _predictor.MinIndexDate;
                _dtpTrainUntil.MaxDate = _predictor.MaxIndexDate;
            }
            /*Verify if dates do conform with the min/max ranges*/
            if (trainFrom < _predictor.MinIndexDate)
                _dtpTrainFrom.MinDate = _dtpTrainFrom.Value = trainFrom = _predictor.MinIndexDate;
            if (trainTo > _predictor.MaxIndexDate)
                _dtpTrainUntil.MaxDate = _dtpTrainUntil.Value = trainTo = _predictor.MaxIndexDate;
            _predictor.timeTrainningSet +=PredictorOnTimeTrainningSet;
            TrainingStatus callback = TrainingCallback;
            _predictor.TrainNetworkAsync(trainFrom, trainTo, callback);
        }

        private void PredictorOnTimeTrainningSet(string timeset, TimeSpan timeSpan, bool ishoitu,int counter)
        {
            Invoke(new MethodInvoker(delegate
                {
                    lblTimeTran.Text = @"Trainning Time:" + timeset;
                    _predictor.timeTrainningSet -= PredictorOnTimeTrainningSet;
                }));
           
        }


        /// <summary>
        /// Predict the values
        /// </summary>
        private void BtnPredictClick(object sender, EventArgs e)
        {
            if (_dgvPredictionResults.Rows.Count != 0)
                _dgvPredictionResults.Rows.Clear();

            if (_predictor == null)         /*The network is untrained*/
            {
                _reloadFiles = false;
               
                switch (MessageBox.Show("UntrainedPredictorWarning", "NoNetwork", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case DialogResult.Yes:
                        break;
                    case DialogResult.No:
                        /*Load the network*/
                        this.Cursor = Cursors.WaitCursor;
                        _hiddenLayers = (int)_nudHiddenLayers.Value;
                        _hiddenUnits = (int)_nudHiddenUnits.Value;
                        try
                        {
                            _predictor = new PredictIndicators(listPredicInput, _hiddenUnits, _hiddenLayers);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _predictor = null;
                            return;
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default;
                        }
                        using (OpenFileDialog ofd = new OpenFileDialog() { FileName = "predictor.ntwrk", Filter = "*.ntwrk" })
                        {
                            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                try
                                {
                                    _predictor.LoadNeuralNetwork(Path.GetFullPath(ofd.FileName));
                                }
                                catch
                                {
                                    MessageBox.Show("Exception", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            DateTime predictFrom = _dtpPredictFrom.Value;
            DateTime predictTo = _dtpPredictTo.Value;
            if (predictFrom.Date > predictTo.Date)
            {
                MessageBox.Show("PredictFromToWarning", "BadParameters", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dtpPredictFrom.Focus();
                return;
            }

            if (_predictor == null)
            {
               
                 Cursor = Cursors.WaitCursor;
                _hiddenLayers = (int)_nudHiddenLayers.Value;
                _hiddenUnits = (int)_nudHiddenUnits.Value;
                try
                {
                    _predictor = new PredictIndicators(listPredicInput, _hiddenUnits, _hiddenLayers);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _predictor = null;
                    return;
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
            PredicResults predicResults;
            List<PredicResults> results = null;
            try
            {
                results = _predictor.Predict(predictFrom, predictTo);
                //predicResults = _predictor.Predict();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            foreach (var item in results)
            {
                _dgvPredictionResults.Rows.Add(item.Date.ToShortDateString(), item.ActualClose,
                                               item.PredictedClose.ToString("F6", CultureInfo.InvariantCulture),
                                               item.Error.ToString("F6", CultureInfo.InvariantCulture));
            }

            //_dgvPredictionResults.Rows.Add(predicResults.Date.ToShortDateString(), predicResults.ActualClose,
            //                                   predicResults.PredictedClose.ToString("F6", CultureInfo.InvariantCulture),
            //                                   predicResults.Error.ToString("F6", CultureInfo.InvariantCulture));

            //ve do thi
            //DrawGraph(DoThi_GiaiTri, results);
            _predictor.ShowError();



        }
        private void GraphInit(ZedGraphControl DoThi)
        {
            GraphPane myPane1 = DoThi.GraphPane; // Khai báo sửa dụng Graph loại GraphPane;

            myPane1.Title.Text = "Đồ thị dự đoán giá Close Mã GD: KCZ13";
            myPane1.XAxis.Title.Text = "Ngày dự đoán";
            myPane1.YAxis.Title.Text = "Giá trị dự đoán";
            // Định nghĩa list để vẽ đồ thị. Để các bạn hiểu rõ cơ chế làm việc ở đây khai báo 2 list điểm <=> 2 đường đồ thị
            RollingPointPairList list6_1 = new RollingPointPairList(1000);
            RollingPointPairList list6_2 = new RollingPointPairList(1000);
            // dòng dưới là định nghĩa curve để vẽ.
          myPane1.AddCurve("Giá trị thực đo", list6_1, Color.Red, SymbolType.Diamond);
          myPane1.AddCurve("Giá trị tính toán bởi mạng", list6_2, Color.Blue, SymbolType.Star);

            // Định hiện thị cho trục thời gian (Trục X)
            //myPane1.XAxis.Scale.Min = 0;
            //myPane1.XAxis.Scale.Max = 10;
            //myPane1.XAxis.Scale.MinorStep = 1;
            //myPane1.XAxis.Scale.MajorStep = 1;
          myPane1.XAxis.Type = AxisType.Date;
          myPane1.XAxis.Scale.Min = new XDate(_predictFrom);  // We want to use time from now
          myPane1.XAxis.Scale.Max = new XDate(_predictTo);  // to 5 minutes per default
          myPane1.XAxis.Scale.MinorUnit = DateUnit.Day;         // set the minimum x unit to time/seconds
          myPane1.XAxis.Scale.MajorUnit = DateUnit.Day;         // set the maximum x unit to time/minutes
          myPane1.XAxis.Scale.Format = "MM/dd/yyyy";
            // Gọi hàm xác định cỡ trục
            myPane1.AxisChange();
        
        }

       private void DrawGraph(ZedGraphControl DoThi, List<PredicResults> results)
        {
            //ve gia tri
            LineItem curve2_1 = DoThi.GraphPane.CurveList[0] as LineItem;
            LineItem curve2_2 = DoThi.GraphPane.CurveList[1] as LineItem;

            //init do thi.
 
            // Get the PointPairList
            IPointListEdit list21 = curve2_1.Points as IPointListEdit;
            IPointListEdit list22 = curve2_2.Points as IPointListEdit;
            list21.Clear();
            list22.Clear();
            DoThi.AxisChange();
            DoThi.Invalidate();
            int i = 0;
            foreach (var item in results)
            {
                var xdate = new XDate(item.Date);
                list21.Add(xdate, item.ActualClose);
                list22.Add(xdate, item.PredictedClose);
                // đoạn chương trình thực hiện vẽ đồ thị
                Scale xScale = DoThi.GraphPane.XAxis.Scale;
                i++;
            }
            // Vẽ đồ thị
            DoThi.AxisChange();
            // Force a redraw
            DoThi.Invalidate();
        }

        /// <summary>
        /// Form closing event
        /// </summary>
        private void WinFinancialMarketPredictorFormClosing(object sender, FormClosingEventArgs e)
        {
            if(_predictor != null)
                _predictor.AbortTraining();
        }


        #region double click

       
        #endregion

        #region Btn Click

        /// <summary>
        /// Stop training
        /// </summary>
        private void BtnStopClick(object sender, EventArgs e)
        {
            FadeControls(false);
            _predictor.AbortTraining();
            _btnExport.Enabled = true;
        }

        /// <summary>
        /// Fade controls from the main form
        /// </summary>
        /// <param name="fade">If true - fade, otherwise - restore</param>
        private void FadeControls(bool fade)
        {
            Action<bool> action = (param) =>
                                  {                                 
                                      _btnStartTraining.Enabled = param;
                                      _btnStop.Enabled = !param;
                                      _dtpPredictFrom.Enabled = param;
                                      _dtpPredictTo.Enabled = param;
                                      _dtpTrainFrom.Enabled = param;
                                      _dtpTrainUntil.Enabled = param;
                                      _nudHiddenLayers.Enabled = param;
                                      _nudHiddenUnits.Enabled = param;
                                  };
            Invoke(action, !fade);
        }

        /// <summary>
        /// Save the network, for later reuse
        /// </summary>
        private void BtnExportClick(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { FileName = "predictor.nwk", Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|*.*" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    FileIOPermission perm = new FileIOPermission(FileIOPermissionAccess.Write, Path.GetFullPath(sfd.FileName));
                    try
                    {
                        perm.Demand();
                    }
                    catch (System.Security.SecurityException)
                    {
                        MessageBox.Show("SecurityExceptionMessage", "SecurityException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    _predictor.ExportNeuralNetwork(Path.GetFullPath(sfd.FileName));
                }
            }
        }

        /// <summary>
        /// Load previously saved network
        /// </summary>
        private void BtnLoadClick(object sender, EventArgs e)
        {

            using (OpenFileDialog ofd = new OpenFileDialog() { FileName = "predictor.nwk", Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _predictor.LoadNeuralNetwork(Path.GetFullPath(ofd.FileName));
                        _nudHiddenLayers.Value = _predictor.HiddenUnits1;
                        _nudHiddenUnits.Value = _predictor.HiddenUnits2;
                    }
                    catch (System.Security.SecurityException)
                    {
                        MessageBox.Show("SecurityExceptionFolderLevel", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch
                    {
                        MessageBox.Show("ExceptionMessage", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Save results
        /// </summary>
        private void BtnSaveResultsClick(object sender, EventArgs e)
        {
            var dgvResults = _dgvPredictionResults;
            SaveFileDialog ofd = new SaveFileDialog { Filter = "*.csv", FileName = "results.csv" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //CSVWriter writer = null;
                //try
                //{
                //    writer = new CSVWriter(ofd.FileName);
                //}
                //catch (System.Security.SecurityException)
                //{
                //    //MessageBox.Show(Resources.SecurityExceptionFolderLevel, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //object[,] values = new object[dgvResults.Rows.Count + 2, dgvResults.Columns.Count];
                //int rowIndex = 0;
                //int colIndex = 0;
                //foreach (DataGridViewColumn col in dgvResults.Columns) /*Writing Column Headers*/
                //{
                //    values[rowIndex, colIndex] = col.HeaderText;
                //    colIndex++;
                //}
                //rowIndex++; /*1*/

                //foreach (DataGridViewRow row in dgvResults.Rows) /*Writing the values*/
                //{
                //    colIndex = 0;
                //    foreach (DataGridViewCell cell in row.Cells)
                //    {
                //        values[rowIndex, colIndex] = cell.Value;
                //        colIndex++;
                //    }
                //    rowIndex++;
                //}

                ///*Writing the results in the last row*/
                //writer.Write(values);
            }
        }

        #endregion

        /// <summary>
        /// Number of hidden units changed
        /// </summary>
        private void NudHiddenUnitsValueChanged(object sender, EventArgs e)
        {
            if(_predictor != null)
            {
                if(MessageBox.Show("ChangedNetwork", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    _predictor = null;
                }
            }
        }

        /// <summary>
        /// Number of hidden layers changed
        /// </summary>
        private void NudHiddenLayersValueChanged(object sender, EventArgs e)
        {
            if (_predictor != null)
            {
                if (MessageBox.Show("ChangedNetwork", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    _predictor = null;
                }
            }
        }

        private void DoThi_GiaiTri_Load(object sender, EventArgs e)
        {

        }
        //Load data Ticker
        private void Button1_Click(object sender, EventArgs e)
        {
            //load dl ticker 
            string connstring = String.Format("Server={0};Port={1};" +
                 "User Id={2};Password={3};Database={4};",
                 "127.0.0.1", "5433", "lemon",
                 "admin", "DailyTrading");

            var tickerbase = txtTicker.Text;
            var dao = new TickerDAO(connstring, tickerbase);

            listPredicInput.Clear();
            _maxDate = new DateTime(2000,01,01);
            _minDate = DateTime.Now.Date;
            //check db da tao chua
            var isexistticker = dao.IsTableExisted(tickerbase);
            if (isexistticker)
            {
               var listdata = dao.SelectALL(tickerbase);
                foreach(var data in listdata)
                {
                    var predicInput = new PredicInput();

                    predicInput.Date = data.Day;

                    predicInput.SetValue((int)PredicInputIndexe.OpenIndex,data.Open);
                    predicInput.SetValue((int)PredicInputIndexe.HighIndex, data.Hight);
                    predicInput.SetValue((int)PredicInputIndexe.LowIndex, data.Low);
                    predicInput.SetValue((int)PredicInputIndexe.CloseIndex, data.Close);
                    predicInput.SetValue((int)PredicInputIndexe.VolumeIndex, data.Volume);

                    predicInput.SetValue((int)PredicInputIndexe.VNIndex, 0);
                    listPredicInput.Add(predicInput);

                    if (_maxDate.Date < data.Day) _maxDate = data.Day;
                    if (_minDate.Date > data.Day) _minDate = data.Day;
                }
            }
            else { MessageBox.Show("Load dl ticker khong thanh cong"); return; }
            dao.Dispose();
            //load dl vnindex
            var daoindex = new TickerDAO(connstring, txtVNIndex.Text);
            isexistticker = daoindex.IsTableExisted(txtVNIndex.Text);
            if (isexistticker)
            {
                var listdata = daoindex.SelectALL(txtVNIndex.Text);
                foreach (var data in listPredicInput)
                {
                    foreach (TickerBase index in from index in listdata
                                          where index.Day == data.Date
                                          select index)
                    {
                        data.SetValue((int)PredicInputIndexe.VNIndex, index.Close);
                    }
                }
            }
            else { MessageBox.Show("Load dl ticker VNIndex khong thanh cong"); return; }
            
            daoindex.Dispose();

            //set value to txtbox
            txtstartDate.Text = _minDate.Date.ToShortDateString();
            _dtpTrainFrom.Value = _minDate.Date;

            txtEndDate.Text = _maxDate.Date.ToShortDateString();
            _dtpTrainUntil.Value = _maxDate.Date.AddDays(-10);

            _dtpPredictFrom.Value = _maxDate.Date.AddDays(-9);
            _dtpPredictTo.Value = _maxDate.Date;

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var formload = new LoadDataFromFile();
            formload.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var formload = new LoadDataFromWeb();
            formload.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }
    }
}
