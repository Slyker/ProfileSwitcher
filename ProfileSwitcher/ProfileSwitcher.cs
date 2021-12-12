using System.Data;

namespace ProfileSwitcher
{
    public partial class ProfileSwitcher : Form
    {
        private DataGridView Dgv;
        private DataSet Ds1 = new DataSet("presets");
        private string FileName;
        public ProfileSwitcher(DataGridView _dgvRows, string _FileName, DataSet _Ds1)
        {
            InitializeComponent();
            Dgv = _dgvRows;
            FileName = _FileName;
            if (_Ds1 != null)
            {
                Ds1 = _Ds1;
            }
            else
            {
                Ds1 = new DataSet();
            }
        }

        private void ajouterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputBox inputBox = new InputBox("Création d'un preset", "Veuillez selectionner un nom pour votre preset", "Valider", "Annuler", (res) =>
             {





                 //XMLsave();

             }, (res) => { });
            inputBox.ShowDialog();
        }

        private void renommerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void supprimeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void refreshUi()
        {

        }
        private void ProfileSwitcher_Load(object sender, EventArgs e)
        {
            string path = FileManager.AppPath + FileName + ".bin";


            foreach (DataRow dr in Ds1.Tables[FileName].Rows)
            {
               listBox1.Items.Add(dr.ItemArray[0].ToString());
            }


        }
        //XML save throught dataset
        private void XMLsave(object sender, EventArgs e)
        {
            Dgv.AllowUserToAddRows = false;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dt.TableName = "OrderData";
            dt.Columns.Add("OrderNr");
            dt.Columns.Add("Custommer");
            dt.Columns.Add("Material");
            dt.Columns.Add("MaterialCode");
            ds.Tables.Add(dt);

            DataTable dt1 = new DataTable();
            dt1.TableName = "Data";
            dt1.Columns.Add("Lenght");
            dt1.Columns.Add("Width");
            dt1.Columns.Add("Qty");
            ds.Tables.Add(dt1);

            DataRow row = ds.Tables["OrderData"].NewRow();
            row["OrderNr"] = "tbOrderNr.Text";
            row["Custommer"] = "tbOrderNr.Text";
            row["Material"] = "tbOrderNr.Text";
            row["MaterialCode"] = "tbOrderNr.Text";
            ds.Tables["Data"].Rows.Add(row);

            foreach (DataGridViewRow r in Dgv.Rows)
            {
                DataRow row1 = ds.Tables["Data"].NewRow();
                row1["Lenght"] = r.Cells[0].Value;
                row1["Width"] = r.Cells[1].Value;
                row1["Qty"] = r.Cells[2].Value;
                ds.Tables["Data"].Rows.Add(row1);
            }
            ds.WriteXml("test.xml");
            Dgv.AllowUserToAddRows = true;

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            Dgv.Rows.Clear();
            string file = FileManager.AppPath + FileName + ".bin";
            using (BinaryReader bw = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                int n = bw.ReadInt32();
                int m = bw.ReadInt32();
                for (int i = 0; i < m; ++i)
                {
                    Dgv.Rows.Add();
                    for (int j = 0; j < n; ++j)
                    {
                        if (bw.ReadBoolean())
                        {
                            Dgv.Rows[i].Cells[j].Value = bw.ReadString();
                        }
                        else bw.ReadBoolean();
                    }
                }
            }
        }
    }
}
