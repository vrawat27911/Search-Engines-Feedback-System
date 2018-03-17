using EyeTribe.ClientSdk;
using EyeTribe.ClientSdk.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SearchingGoogle
{   
    public partial class SearchWin : Form, IGazeListener
    {
        private float m_SmoothedX;
        private float m_SmoothedY;
        private float m_RawX;
        private float m_RawY;
        private int screenX;
        private int screenY;
        private string searchTerm;
        private List<ResultItem> resultItems;
        private bool isTrain = true;

        // maps search term to click data n gaze data.
        private Dictionary<string, Data> search_data = new Dictionary<string, Data>();

        // If Eye Tracker is On
        bool trackerOn = false;

        //Seach Tracking 
        bool trackingActive = false;

        //Storing the Template Data
        CsvRow mTemplateData = null;

        CsvRow mTemplateclick = new CsvRow();

        // Dictionary<String, WriteCVS> dict_clickCVS = new Dictionary<string, WriteCVS>();
        Dictionary<String, WriteCVS> dict_gazeCVS = new Dictionary<string, WriteCVS>();
        
        int UserId;

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }
        
        private void keyPressed(Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Shift)
            {
                if (!trackerOn)
                    startTracking();
                else
                    stopEyeTracking();
            }
            if (e.Control)
            {
                trackingActive = true;
            }

            if (e.Alt)
            {
                trackingActive = false;
            }

            if (e.KeyData == Keys.Escape)
            {
                Environment.Exit(0);
            }
        }

        private void startTracking()
        {
            if (!trackerOn)
            {
                // Connect client
                GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);

                // Register this class for events
                GazeManager.Instance.AddGazeListener(this);
                trackerOn = true;
            }
        }

        private static readonly object writeLock = new object();

        private void writeGazeData()
        {
            lock (writeLock)
            {
                // Record gaze data as candidate path
                if (trackerOn && trackingActive)
                {
                    //Console.WriteLine("X = {0} and Y = {1} and tracking = {2}", m_RawX, m_RawY);
                    mTemplateData.Add(screenX.ToString());
                    mTemplateData.Add(screenY.ToString());
                    mTemplateData.Add(m_RawX.ToString());
                    mTemplateData.Add(m_RawY.ToString());
                    mTemplateData.Add(m_SmoothedX.ToString());
                    mTemplateData.Add(m_SmoothedY.ToString());
                    mTemplateData.Add(DateTime.UtcNow.ToString(DateTime.UtcNow.ToString("hh:mm:ss:fff")));

                    Point p = new Point(screenX, screenY);
                    Control gBcntrl = null;

                    IntPtr hWnd = WindowFromPoint(Control.MousePosition);
                    if (hWnd != IntPtr.Zero)
                    {
                        gBcntrl = Control.FromHandle(hWnd);
                    }

                    if (gBcntrl != null)
                    {
                        if (!search_data.ContainsKey(searchTerm))
                        {
                            search_data[searchTerm] = new Data();
                        }

                        string controlName = gBcntrl.Name;

                        if (controlName.Contains("group") || controlName.Contains("rich") || controlName.Contains("label"))
                        {
                            string indexString = Regex.Match(controlName, @"\d+").Value;
                            int index = Int32.Parse(indexString);
                            search_data[searchTerm].gaze_data[index - 1] += 1;
                        }
                    }

                    string tabPageName = "";

                    tabControl1.Invoke(new MethodInvoker(delegate { tabPageName = tabControl1.SelectedTab.Text; }));

                    WriteCVS writer = dict_gazeCVS["tab" + tabPageName];

                    FileWriter.writeData(mTemplateData, writer);
                    mTemplateData.Clear();
                }
            }
        }

        public void OnGazeUpdate(GazeData gazeData)
        {
            bool Smooth = true;

            var x = this.Bounds.X;
            var y = this.Bounds.Y;

            var gX = Smooth ? gazeData.SmoothedCoordinates.X : gazeData.RawCoordinates.X;
            var gY = Smooth ? gazeData.SmoothedCoordinates.Y : gazeData.RawCoordinates.Y;
            screenX = (int)Math.Round(x + gX, 0);
            screenY = (int)Math.Round(y + gY, 0);

            m_RawX = gazeData.RawCoordinates.X;
            m_RawY = gazeData.RawCoordinates.Y;

            m_SmoothedX = gazeData.SmoothedCoordinates.X;
            m_SmoothedY = gazeData.SmoothedCoordinates.Y;

            // return in case of 0,0 
            if (screenX == 0 && screenY == 0) return;

            // Write the Updated gaze Data
            writeGazeData();

            Win32.SetCursorPos((int)m_SmoothedX, (int)m_SmoothedY);

        }

        public void stopEyeTracking()
        {
            if (trackerOn)
            {
                GazeManager.Instance.RemoveGazeListener(this);
                Console.WriteLine("*DISCONNECTING CONNECTION");
                GazeManager.Instance.Deactivate();
                trackerOn = false;
            }
        }

        public static void createDirectory(string path)
        {
            // string path = @"c:\MyDir";

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Console.WriteLine("That path exists already.");
                    return;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public WriteCVS CreateCSVFile(string fileName)
        {   
            WriteCVS m_CSVWrtier = FileWriter.CreateCSVFile(fileName);
            mTemplateData.Add("X");
            mTemplateData.Add("Y");
            mTemplateData.Add("Time");
            FileWriter.writeData(mTemplateData, m_CSVWrtier);
            mTemplateData.Clear();
            return m_CSVWrtier;
        }

        //public void mouse_click(Object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    // Console.WriteLine("X = {0} and Y = {1} and tracking = {2}", m_RawX, m_RawY);
        //    mTemplateData.Add(e.X.ToString());
        //    mTemplateData.Add(e.Y.ToString());
        //    //mTemplateData.Add(e.RawX.ToString());
        //    //mTemplateData.Add(m_RawY.ToString());
        //    //mTemplateData.Add(m_SmoothedX.ToString());
        //    //mTemplateData.Add(m_SmoothedY.ToString());

        //    mTemplateData.Add(DateTime.UtcNow.ToString(DateTime.UtcNow.ToString("hh:mm:ss:fff")));

        //    WriteCVS writer = dict_clickCVS[this.tabControl1.SelectedTab.Text];
        //    FileWriter.writeData(mTemplateData, writer);
        //    mTemplateData.Clear();
        //}

        public SearchWin(int pUserId, bool isTrain)
        {
            InitializeComponent();
            mTemplateData = new CsvRow();
            this.isTrain = isTrain;

            if (isTrain)
            {
                this.KeyPreview = true;
                this.KeyDown += new System.Windows.Forms.KeyEventHandler(keyPressed);
                this.FormClosing += this.onClos;

                this.linkLabel1.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 1));
                this.linkLabel2.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 2));
                this.linkLabel3.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 3));
                this.linkLabel4.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 4));
                this.linkLabel5.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 5));
                this.linkLabel6.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 6));
                this.linkLabel7.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 7));
                this.linkLabel8.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 8));
                this.linkLabel9.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 9));
                this.linkLabel10.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 10));
                this.linkLabel11.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 11));
                this.linkLabel12.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 12));
                this.linkLabel13.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 13));
                this.linkLabel14.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 14));
                this.linkLabel15.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 15));
                this.linkLabel16.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 16));
                this.linkLabel17.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 17));
                this.linkLabel18.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 18));
                this.linkLabel19.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 19));
                this.linkLabel20.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 20));

                this.label1.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 1));
                this.label2.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 2));
                this.label3.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 3));
                this.label4.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 4));
                this.label5.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 5));
                this.label6.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 6));
                this.label7.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 7));
                this.label8.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 8));
                this.label9.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 9));
                this.label10.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 10));
                this.label11.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 11));
                this.label12.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 12));
                this.label13.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 13));
                this.label14.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 14));
                this.label15.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 15));
                this.label16.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 16));
                this.label17.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 17));
                this.label18.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 18));
                this.label19.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 19));
                this.label20.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 20));

                this.groupBox1.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 1));
                this.groupBox2.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 2));
                this.groupBox3.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 3));
                this.groupBox4.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 4));
                this.groupBox5.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 5));
                this.groupBox6.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 6));
                this.groupBox7.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 7));
                this.groupBox8.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 8));
                this.groupBox9.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 9));
                this.groupBox10.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 10));
                this.groupBox11.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 11));
                this.groupBox12.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 12));
                this.groupBox13.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 13));
                this.groupBox14.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 14));
                this.groupBox15.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 15));
                this.groupBox16.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 16));
                this.groupBox17.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 17));
                this.groupBox18.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 18));
                this.groupBox19.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 19));
                this.groupBox20.MouseClick += new System.Windows.Forms.MouseEventHandler((sender, e) => this.groupBox_Click(sender, e, 20));
            }
            //this.MouseClick += new System.Windows.Forms.MouseEventHandler(mouse_click);

            UserId = pUserId;
            
            //Clearing all the Values
            ClearAll();
        }

        private void PopulateWindow(List<ResultItem> resultsList)
        {
            Control linkLabel;
            Control label;
            Control description;

            String linklabelStr = "linkLabel";
            String labelStr = "label";
            String descriptionStr = "richTextBox";

            Control groupBox;
            String groupBoxStr = "groupBox";

            tabControl1.Visible = true;
            
            for (int i = 1; i <= 20; i++)
            {
                linkLabel = this.Controls.Find(linklabelStr + i.ToString(), true)[0];
                label = this.Controls.Find(labelStr + i.ToString(), true)[0];
                description = this.Controls.Find(descriptionStr + i.ToString(), true)[0];
                groupBox = this.Controls.Find(groupBoxStr + i.ToString(), true)[0];

                //Clear Text
                linkLabel.Text = resultsList[i-1].title;
                linkLabel.ForeColor = System.Drawing.Color.Blue;
                linkLabel.Font = new Font("Calibiri", 15);
                label.Text = resultsList[i-1].link;
                label.ForeColor = System.Drawing.ColorTranslator.FromHtml("#009933");
                description.Text = resultsList[i-1].description;
                description.Font = new Font("Calibiri", 10);
                description.BackColor = System.Drawing.Color.White;

                linkLabel.Visible = true;
                label.Visible = true;
                description.Visible = true;
                groupBox.Visible = true;
                groupBox.Enabled = true;
                description.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackingActive = true;

            if (searchTerm == textBox1.Text)
                return;

            searchTerm = textBox1.Text;

            //Load the files to the search
            if (!String.IsNullOrEmpty(searchTerm)) // String is not Null
            {
                searchTerm = textBox1.Text;
                string directryPath = @"..\..\Data\UserStudy\P_" + UserId.ToString();
                createDirectory(directryPath);
                string filePath = directryPath + "\\";
                string[] file_gaze = new string[10];
                //string[] file_click = new string[10];

                if (isTrain)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        file_gaze[i] = filePath + searchTerm + "_tab" + i.ToString() + "_gazeData.csv";
                        // file_click[i] = filePath + searchTerm + "_click" + i.ToString() + "_clickData.csv";
                        dict_gazeCVS["tabPage" + i.ToString()] = CreateCSVFile(file_gaze[i]);
                        //dict_clickCVS["tabPage" + i.ToString()] = CreateCSVFile(file_click[i]);
                    }
                }

                string basePath;

                if (isTrain)
                {
                    basePath = @"..\..\Data\SearchData\";
                }
                else
                {
                    basePath = @"..\..\Data\ResultsData\";

                    if(!File.Exists(basePath + searchTerm))
                        basePath = @"..\..\Data\SearchData\";
                }
                
                JsonReader jsonObject = new JsonReader();

                resultItems = jsonObject.LoadJson(basePath + searchTerm);

                if (resultItems!= null)
                {
                    labl_a.Text = "Following Results Were Found";
                    labl_a.ForeColor = System.Drawing.Color.Blue;
                    tabControl1.Visible = true;
                    Console.WriteLine("Results Found");
                    PopulateWindow(resultItems);
                }
                else
                {
                    labl_a.Text = "No results Found, The Term is not added to the database yet";
                    labl_a.ForeColor = System.Drawing.Color.Red;
                    ClearAll();
                    Console.WriteLine("We are still trying to imbed the search terms in the interface");
                }
            }
        }

        private void ClearAll()
        {
            //Control linkLabel;
            //Control label;
            //Control description;
            //String linklabelStr = "linkLabel";
            //String labelStr = "label";
            //String descriptionStr = "richTextBox";

            Control groupBox;
            String groupBoxStr = "groupBox";

            tabControl1.Visible = false;
            textBox1.Clear();           

            for (int i = 1; i <= 20; i++)
            {
               // linkLabel = this.Controls.Find(linklabelStr + i.ToString(), true)[0];
               // label = this.Controls.Find(labelStr + i.ToString(), true)[0];
               // description = this.Controls.Find(descriptionStr + i.ToString(), true)[0];
                groupBox = this.Controls.Find(groupBoxStr + i.ToString(), true)[0];

                //Clear Text
               // linkLabel.Text = "";
               // label.Text = "";
               // description.Text = "";
                

              // Hide the control
              //  linkLabel.Visible = false;
              //  label.Visible = false;
              //  description.Visible = false;
                groupBox.Visible = false;
            }
        }

        private void gazeNclickBasedRank()
        {
            if (resultItems != null)
            {
                string basePath = @"..\..\Data\ResultsData\";
                JsonReader jsonObject = new JsonReader();
                List<GazeNClickResult> new_resultItems = new List<GazeNClickResult>();

                var filepath = basePath + searchTerm;

                if (File.Exists(filepath))
                {
                    new_resultItems = jsonObject.LoadJsonNew(basePath + searchTerm);
                    int i = 0;

                    new_resultItems.Sort(GazeNClickResult.sortByRank());

                    if (search_data.ContainsKey(searchTerm))
                    {
                        foreach (var newResult in new_resultItems)
                        {
                            newResult.clickCount += search_data[searchTerm].click_data[i];
                            newResult.gazeCount += search_data[searchTerm].gaze_data[i];
                            i++;
                            if (i == 20) break;
                        }
                    }

                    new_resultItems.Sort(GazeNClickResult.sortByClkNGaze());

                    File.Delete(filepath);
                }
                else
                {
                    int i = 0;
                    //Parse resultItems from regular search engine
                    foreach (var result in resultItems)
                    {
                        GazeNClickResult newResult = new GazeNClickResult();
                        newResult.link = result.link;
                        newResult.description = result.description;
                        newResult.title = result.title;
                        newResult.rank = result.rank;

                        if (search_data.ContainsKey(searchTerm))
                        {
                            newResult.clickCount = search_data[searchTerm].click_data[i];
                            newResult.gazeCount = search_data[searchTerm].gaze_data[i];
                        }

                        new_resultItems.Add(newResult);

                        if (search_data.ContainsKey(searchTerm))
                            new_resultItems.Sort(GazeNClickResult.sortByClkNGaze());

                        i++;

                        if (i == 20) break;
                    }
                }

                int gazeRank = 1;
                foreach (var newResult in new_resultItems)
                {
                    newResult.gazeRank = gazeRank++;
                    if (gazeRank > 20)
                        break;
                }

                string json = JsonConvert.SerializeObject(new_resultItems.ToArray());
                //write string to file
                System.IO.File.WriteAllText(filepath, json);
            }
        }

        private void onClos(Object sender, FormClosingEventArgs e)
        {
            trackingActive = false;

            if (dict_gazeCVS.Count != 0)
            {
                foreach (var item in dict_gazeCVS)
                    FileWriter.closeCSVFile(item.Value);

                gazeNclickBasedRank();
            }

            labl_a.Text = "";
            ClearAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            trackingActive = false;


            if (dict_gazeCVS.Count != 0)
            {
                foreach (var item in dict_gazeCVS)
                    FileWriter.closeCSVFile(item.Value);

                gazeNclickBasedRank();
            }

            labl_a.Text = "";
            ClearAll();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (FileWriter.m_CSVWriter != null)
            //FileWriter.closeCSVFile();
            {
                if (dict_gazeCVS.Count != 0)
                {
                    foreach (var item in dict_gazeCVS)
                        FileWriter.closeCSVFile(item.Value);
                }

                gazeNclickBasedRank();
            }   
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pnt);

        private void groupBox_Click(object sender, MouseEventArgs e, int i)
        {
            if (e is MouseEventArgs)
            {
                MouseEventArgs a = (MouseEventArgs)e;

                int x_coord = a.X;
                int y_coord = a.Y;
                mTemplateclick.Add(x_coord.ToString());
                mTemplateclick.Add(y_coord.ToString());
                mTemplateclick.Add(DateTime.UtcNow.ToString(DateTime.UtcNow.ToString("hh:mm:ss:fff")));

                Point p = new Point(x_coord, y_coord);
                // var cntrl = GetChildAtPoint(p);
                // Control tbpagecntrl = null;
                Control gBcntrl = null;

                IntPtr hWnd = WindowFromPoint(Control.MousePosition);
                if (hWnd != IntPtr.Zero)
                {
                    gBcntrl = Control.FromHandle(hWnd);
                }

                //if (cntrl != null)
                //{
                //    tbpagecntrl = cntrl.GetChildAtPoint(p);
                //    if (tbpagecntrl != null)
                //        gBcntrl = tbpagecntrl.GetChildAtPoint(p);
                //}

                if (gBcntrl != null)
                {
                    if (!search_data.ContainsKey(searchTerm))
                    {
                        search_data[searchTerm] = new Data();
                    }

                    string indexString = Regex.Match(gBcntrl.Name, @"\d+").Value;
                    int index = Int32.Parse(indexString);

                    search_data[searchTerm].click_data[index - 1] += 1;
                }

                string directryPath = @"..\..\Data\UserStudy\P_" + UserId.ToString();
                createDirectory(directryPath);
                string filePath = directryPath + "\\";

                filePath += searchTerm + "_groupBox" + i.ToString() + "_clickData.csv";
                StreamWriter strmwrtr = null;

                if (File.Exists(filePath))
                {
                    strmwrtr = new StreamWriter(filePath, append: true);
                }
                else
                    strmwrtr = new StreamWriter(filePath);

                WriteCVS.WriteRow1(mTemplateclick, strmwrtr);
                strmwrtr.Close();
                mTemplateclick.Clear();
            }
        }
    }

    public class Data
    {
        // int[] index = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19};
        public int[] click_data = Enumerable.Repeat(0, 20).ToArray();
        public int[] gaze_data = Enumerable.Repeat(0, 20).ToArray();
    }
}
