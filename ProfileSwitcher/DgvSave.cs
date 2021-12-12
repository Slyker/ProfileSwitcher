using System.Data;
using System.Reflection;

namespace ProfileSwitcher
{

    public static class DgvSave
    {
        internal static List<DataSet> DataSets { get; } = new List<DataSet>();
        public static List<DgvSet> Tables { get; } = new List<DgvSet>();
        public static void Add(string SetName, DataGridView Dgv, List<DgvColumn> dgvColumns, string TableName = "Defaut")
        {
            Tables.Add(new DgvSet(SetName, Dgv, dgvColumns, TableName));
        }
        public static void Save(string SetName = "")
        {
            var item =(SetName!="")?  Tables.Find(x => x.Name == SetName):null;
            if (item != null)
            {
                item.Save();
            }
            else
            {
                Tables.ForEach(x => x.Save());
            }
        }
        public static void Load(string SetName = "")
        {
            var item = (SetName != "") ? Tables.Find(x => x.Name == SetName) : null;
            if (item != null)
            {
                item.Load();
            }
            else
            {
                Tables.ForEach(x => x.Load());
            }
        }

    }
    public class DgvSet
    {
        public string Name { get; set; }
        public string Description { get; set; } = "Test description";
        public DataSet DataSet { get; set; }
        public List<DgvColumn> Columns { get; set; } = new List<DgvColumn>();

        public string Path { get { return FileManager.AppPath + Name + ".xml"; } }
        public void Save()
        {
            DataSet.WriteXml(Path);
        }
        public void Load()
        {
            DataSet.ReadXml(Path);
        }
        public DgvSet(string SetName, DataGridView Dgv, List<DgvColumn> dgvColumns, string TableName = "Defaut")
        {
            string path = FileManager.AppPath + SetName + ".xml";
            Name = SetName;
            Columns = dgvColumns;
            DataSet = new DataSet(SetName);
            if (File.Exists(path))
            {
                Load();
            }
            DataSet ds = DgvSave.DataSets.Find(x => x.DataSetName == SetName);
            if(ds == null)
            {
                DgvSave.DataSets.Add(DataSet);
            }
            else
            {
                DataSet = ds;
            }
            

            Dgv.AllowUserToAddRows = false;

            DataTable DataTable = new DataTable(TableName);
            DataTable dt = DataSet.Tables[TableName];
            if (dt == null)
            {
                DataSet.Tables.Add(DataTable);
                foreach (DgvColumn dgvColumn in Columns)
                {
                    DataTable.Columns.Add(dgvColumn.Name, dgvColumn.Type);
                }
            }
            else
            {
                DataTable = dt;
            }

            DataRow row = DataSet.Tables[TableName].NewRow();
            row["Description"] = "tbOrderNr.Text";
            row["Test"] = "tbOrderNr.Text";
            DataSet.Tables[TableName].Rows.Add(row);


            Save();


            Dgv.DataSource = DataSet.Tables[TableName];
            Dgv.AllowUserToAddRows = true;


        }
        public object this[string propertyName]
        {
            get
            {
                // probably faster without reflection:
                // like:  return Properties.Settings.Default.PropertyValues[propertyName] 
                // instead of the following
                Type myType = typeof(DgvSet);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                Type myType = typeof(DgvSet);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                myPropInfo.SetValue(this, value, null);
            }
        }
    }

    public class DgvColumn
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public DgvColumn(string _name, Type _Type)
        {
            Name = _name;
            Type = _Type;
        }

    }
}
