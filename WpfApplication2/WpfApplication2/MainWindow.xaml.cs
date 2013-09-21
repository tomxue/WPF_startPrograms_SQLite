using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace QQ_Byhh
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Info> mPath = new List<Info>();
        //List<string> mName = new List<string>();
        string datasource = @"C:\Users\Administrator\Desktop\WPF_startPrograms\test.db";

        public MainWindow()
        {
            InitializeComponent();
            this.DataBinding();
        }

        private void DataBinding()
        {
            this.ProgramList.ItemsSource = mPath;//为ListView绑定数据源：多进程路径  
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool AddFlag = true;
            Microsoft.Win32.OpenFileDialog oFile = new Microsoft.Win32.OpenFileDialog();
            oFile.InitialDirectory = @"c:\";
            oFile.RestoreDirectory = true;
            oFile.Filter = "可执行文件(*.exe)|*.exe";
            oFile.ShowDialog();
            if (oFile.FileName != String.Empty)
            {
                // 若是已加入过的进程，则不再加入
                for (int i = 0; i < mPath.Count; i++)
                {
                    if (mPath[i].path == oFile.FileName)
                    {
                        AddFlag = false;
                    }
                }   // 没加入过的进程，则加入，即mPath的赋值过程
                if (AddFlag)
                {
                    Info myPath = new Info()
                    //myPath.Path = oFile.FileName;
                    {
                        Path = oFile.FileName   // object initializer
                    };
                    mPath.Add(myPath);
                }
            }
            this.ProgramList.Items.Refresh();
        }

        private void DeleteProgram(string sP)
        {

            if (mPath.Count > 0)
            {
                for (int i = 0; i < mPath.Count; i++)
                {
                    if (mPath[i].path == sP)
                    {
                        mPath.Remove(mPath[i]);
                    }
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            string sP = Convert.ToString(b.CommandParameter);
            this.DeleteProgram(sP);
            this.ProgramList.Items.Refresh();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < mPath.Count; i++)
            {
                Process p = new Process();
                p.StartInfo.FileName = mPath[i].path;
                p.Start();
            }
        }

        private void Set_Click(object sender, RoutedEventArgs e)
        {
            //创建一个数据库文件
            SQLiteConnection.CreateFile(datasource);

            //连接数据库
            SQLiteConnection conn =
                new SQLiteConnection();

            SQLiteConnectionStringBuilder connstr =
                new SQLiteConnectionStringBuilder();

            connstr.DataSource = datasource;
            //设置密码，SQLite ADO.NET实现了数据库密码保护
            connstr.Password = "admin";
            // 关联conn和connstr
            conn.ConnectionString = connstr.ToString();
            conn.Open();

            //创建表
            SQLiteCommand cmd = new SQLiteCommand();
            string sql = "CREATE TABLE test(username varchar(20),password varchar(20))";
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();

            //插入数据
            for (int i = 0; i < mPath.Count; i++)
            {
                sql = "INSERT INTO test VALUES('" + mPath[i].Path + "','mypassword')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            //取出数据
            sql = "SELECT * FROM test";
            cmd.CommandText = sql;
            SQLiteDataReader reader = cmd.ExecuteReader();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mPath.Count; i++)
            {
                reader.Read();
                sb.Append(reader.GetString(0)).Append("\n");
            }

            MessageBox.Show(sb.ToString());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    class Info  // one property
    {
        public string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }

    // TODO： Add database support to store the starting processes
}