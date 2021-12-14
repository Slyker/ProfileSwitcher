using Newtonsoft.Json;
using System.Data;

namespace ProfileSwitcher
{

    public static class DgvSave
    {
        //Comment
        internal static List<DataSet> DataSets { get; } = new List<DataSet>();
        public static List<DgvSet> DgvSets { get; } = new List<DgvSet>();
        public static DgvSet Add(string _FileName, List<DgvColumn> dgvColumns, string SetName = "Defaut", string TableName = "Defaut")
        {
            DgvSet dgvSet = new DgvSet(_FileName, dgvColumns, SetName, TableName);
            DgvSets.Add(dgvSet);
            return dgvSet;
        }
        public static void Save(string SetName = "")
        {
            var item = (SetName != "") ? DgvSets.Find(x => x.FileName == SetName) : null;
            if (item != null)
            {
                item.Save();
            }
            else
            {
                DgvSets.ForEach(x => x.Save());
            }
        }
        public static void Load(string SetName = "")
        {
            var item = (SetName != "") ? DgvSets.Find(x => x.FileName == SetName) : null;
            if (item != null)
            {
                item.Load();
            }
            else
            {
                DgvSets.ForEach(x => x.Load());
            }
        }

    }
    public class DgvSet
    {
        public string FileName { get; set; }
        public string Description { get; set; } = "Test description";
        public DataSet DataSet { get; set; }
        public DataGridView Dgv { get; set; }
        public List<DgvColumn> Columns { get; set; } = new List<DgvColumn>();

        public string Path { get { return FileManager.AppPath + FileName + ".json"; } }
        public DgvSet(string _FileName, List<DgvColumn> dgvColumns, string SetName = "Defaut", string TableName = "Defaut")
        {

            string path = FileManager.AppPath + _FileName + ".json";
            FileName = _FileName;
            Columns = dgvColumns;
            Dgv = new DataGridView();
            SetName = _FileName;

            DataSet = new DataSet(SetName);
            //DataSet.EnforceConstraints = false;
            if (File.Exists(path))
            {
                Load();
            }
            DataSet ds = DgvSave.DataSets.Find(x => x.DataSetName == SetName);
            if (ds == null)
            {
                DgvSave.DataSets.Add(DataSet);
            }
            else
            {
                DataSet = ds;
            }
            AddDTable(TableName);
            //AddRow(dt, new string[] { "", "" });
            Save();
        }
        public void Save()
        {
            //DataSet.WriteXml(Path);


            using (StreamWriter file = new StreamWriter(Path))
            {
                string json = JsonConvert.SerializeObject(DataSet, Formatting.Indented);
                file.Write(json);
            }


            //DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);
        }
        public DataTable? Load(string? tableName = null)
        {
            Dgv.DataSource = null;
            Dgv.Rows.Clear();
            Dgv.Columns.Clear();

            if (tableName != null)
            {
                Dgv.DataSource = DataSet.Tables[tableName];
                return DataSet.Tables[tableName];
            }
            else
            {

                DataSet? dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText(Path).ToString());
                if (dataSet != null)
                {
                    DataSet.Merge(dataSet);
                }
                return null;
            }

        }
        public void OpenSwitcher(string TableName = "Defaut")
        {
            Main mainSwitcher = new Main(FileName, TableName);
            Dgv = mainSwitcher.dataGridView1;
            Dgv.DataSource = Load(TableName);
            mainSwitcher.Show();
        }

        public DataTable? RemoveDTable(string? TableName)
        {
            if (TableName == null) { return null; }
            DataTable dt = DataSet.Tables[TableName];
            if (dt == null)
            {

                return null;
            }
            else
            {
                if (Dgv.DataSource == dt)
                {
                    DataTable dataTable = DataSet.Tables.Cast<DataTable>().First();
                    Dgv.DataSource = dataTable;
                    DataSet.Tables.Remove(dt);
                    Save();
                    return dataTable;
                }
                DataSet.Tables.Remove(dt);
                Save();
                return null;
            }
        }
        public bool RenameDTable(string TableName, string newName)
        {
            DataTable dt = DataSet.Tables[TableName];
            if (dt == null)
            {

                return false;
            }
            else
            {
                try
                {
                    dt.TableName = newName;
                    Save();
                }
                catch
                {
                    MessageBox.Show("Un preset porte déjà ce nom, merci d'en choisir un autre.");
                    return false;
                }


                return true;
            }
        }
        public void AddDTable(string TableName, bool baseRow = false)
        {
            
            DataTable? dt = DataSet.Tables[TableName];
           
            DataTable DataTable = dt ?? new DataTable(TableName);
            List<DataColumn> keys = new List<DataColumn>();
            DataColumn column;
            if (dt == null)
            {
                DataSet.Tables.Add(DataTable);
            }
            string? FirstColName = null;
            foreach (DgvColumn dgvColumn in Columns)
            {

                column = new DataColumn();
                column.DataType = dgvColumn.Type;
                column.ColumnName = dgvColumn.Name;
                column.DefaultValue = dgvColumn.defaultValue ?? null;
                if (!DataTable.Columns.Contains(dgvColumn.Name))
                {
                    DataTable.Columns.Add(column);

                }
                if (FirstColName == null)
                {
                    FirstColName = dgvColumn.Name;
                }
                if (dgvColumn.isPrimary)
                {
                    keys.Add(DataTable.Columns[dgvColumn.Name]);
                }

            }
            if (keys.Count() == 0)
            {
                keys.Add(DataTable.Columns[FirstColName]);
            }
            DataTable.PrimaryKey = keys.ToArray();
            //DataTable.PrimaryKey = keys;
            if (baseRow)
            {
                string[] strs = new string[Columns.Count];
                for (int i = 0; i < Columns.Count; i++)
                {
                    if(DataTable.PrimaryKey.Count() > i && DataTable.PrimaryKey[i] != null)
                    {
                        strs[i] = Columns[i].defaultValue?? "0";
                    }
                   
                }

                DataTable = AddRow(DataTable, strs);
            }

            Dgv.DataSource = DataTable;
            Save();
            //return;

        }
        public DataTable AddRow(DataTable dataTable, string[] items)
        {
            DataRow row = dataTable.NewRow();
            int id = 0;

            foreach (string item in items)
            {
                row[Columns[id].Name] = item;
                id++;
            }

            dataTable.Rows.Add(row);
            Save();
            return dataTable;

        }
    }

    public class DgvColumn
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool isPrimary { get; set; }
        public string? defaultValue { get; set; } = null;

        public DgvColumn(string _name, Type _Type, bool _isPrimary = false, string? _defaultValue = null)
        {
            Name = _name;
            Type = _Type;
            isPrimary = _isPrimary;
            defaultValue = _defaultValue;
        }

    }
}
