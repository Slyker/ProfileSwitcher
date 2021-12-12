using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace ProfileSwitcher
{
    public partial class Form1 : Form
    {
        public List<DataSet> DataSets { get; set; } = new List<DataSet>();
        
        public Form1()
        {
                
            InitializeComponent();

            foreach (Control _control in this.Controls)
            {
                if(_control is DataGridView)
                {
                    DataGridView control = _control as DataGridView;
                    control.Columns.Insert(0, new DataGridViewColumn() { HeaderText = "#" });
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProfileSwitcher profileSwitcher = new ProfileSwitcher(dataGridView1,tabControl1.SelectedTab.Name, DataSets.Find(x => x.DataSetName == tabControl1.SelectedTab.Name));
           // profileSwitcher.

            profileSwitcher.Show();
        }
        private void InitDataSource(string setName, DataGridView Dgv,string tableName ="Defaut")
        {
            string path = FileManager.AppPath + setName + ".xml";

            DataSet dataSet = new DataSet(setName);
            DataSets.Add(dataSet);
            SetDataSource(setName, Dgv, tableName);
            if (File.Exists(path))
            {
                dataSet.ReadXml(path);
            }

            dataSet.WriteXml(path);
      
        }
        List<List<(string, string)>> Tables = new List<List<(string, string)>>();
        List<(string, string)> Columns = new List<(string, string)>();
            
        private void SetDataSource(string name, DataGridView Dgv, string tableName = "Defaut")
        {
            string path = FileManager.AppPath + tabControl1.SelectedTab.Name + ".xml";
            Dgv.AllowUserToAddRows = false;

            DataTable dt = new DataTable(tableName);
            dt.Columns.Add("test");
            dt.Columns.Add("test2");
            dt.Columns.Add("test3");
            DataSet ds = DataSets.Find(x => x.DataSetName == name);
            ds.Tables.Add(dt);

            DataRow row = ds.Tables[tableName].NewRow();
            row["test"] = "tbOrderNr.Text";
            row["test2"] = "tbOrderNr.Text";
            row["test3"] = "tbOrderNr.Text";
            ds.Tables[tableName].Rows.Add(row);

            Dgv.DataSource = ds.Tables[tableName];

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            FileManager.Init();
            string path = FileManager.AppPath + tabControl1.SelectedTab.Name + ".xml";
            DgvSave.Add("addtext",dataGridView1,new List<DgvColumn>() { new DgvColumn("Description",typeof(string)), new DgvColumn("Test", typeof(string)) });
            //InitDataSource("addtext",dataGridView1);
            //InitDataSource("replacetext", dataGridView2);
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //source.ResetBindings(false);
            dataGridView1.Refresh();

            dataGridView1.Update();
            foreach (DataTable dt in DataSets[0].Tables)
            {
                var result = "";
                foreach(DataRow dr in dt.Rows)
                {
                    
                    result += dr["Column1"] + "\n";
                }
                MessageBox.Show(result);
            }
        }
    }
}