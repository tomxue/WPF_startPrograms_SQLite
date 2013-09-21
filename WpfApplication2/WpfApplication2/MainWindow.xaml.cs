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

namespace MultiStart
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Info> mPath = new List<Info>();
        //List<string> mName = new List<string>();
        string datasource = @"C:\Users\Administrator\Desktop\WPF_startPrograms\test.db";
        enum dowhat { set, get };

        public MainWindow()
        {
            InitializeComponent();
            this.DataBinding();
        }

        private void DataBinding()
        {
            this.ProgramList.ItemsSource = mPath;//为ListView绑定数据源：多进程路径  

            dbOp(dowhat.get);
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
            dbOp(dowhat.set);
        }

        private void dbOp(dowhat dw)
        {
            if (dw == dowhat.set)
            {
                //创建一个数据库文件
                SQLiteConnection.CreateFile(datasource);
            }

            //连接数据库
            SQLiteConnection conn =
                new SQLiteConnection();

            SQLiteConnectionStringBuilder connstr =
                new SQLiteConnectionStringBuilder();

            connstr.DataSource = datasource;
            //设置密码，SQLite ADO.NET实现了数据库密码保护
            connstr.Password = "admin";
            //关联conn和connstr
            conn.ConnectionString = connstr.ToString();
            conn.Open();

            string sql = null;
            SQLiteCommand cmd = null;

            if (dw == dowhat.set)
            {
                //创建表
                cmd = new SQLiteCommand();
                sql = "CREATE TABLE test(username varchar(20),password varchar(20))";
                cmd.CommandText = sql;
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
            }

            if (dw == dowhat.set)
            {
                //插入数据
                //先插入数据个数
                sql = "INSERT INTO test VALUES('" + mPath.Count + "','mypassword')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                //再插入待执行程序路径
                for (int i = 0; i < mPath.Count; i++)
                {
                    sql = "INSERT INTO test VALUES('" + mPath[i].Path + "','mypassword')";
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }

            if (dw == dowhat.get)
            {
                int num;

                //取出数据
                cmd = new SQLiteCommand();
                sql = "SELECT * FROM test";
                cmd.CommandText = sql;
                cmd.Connection = conn;
                SQLiteDataReader reader = cmd.ExecuteReader();
                //取出的第一个数据是预先加载的程序数量
                if (reader.Read())  //如果有数据，再进一步处理
                {
                    num = Int32.Parse(reader.GetString(0));
                    for (int i = 0; i < num; i++)
                    {
                        reader.Read();
                        Info myPath = new Info()
                        {
                            Path = reader.GetString(0)   // object initializer
                        };
                        mPath.Add(myPath);
                    }
                }
                else
                    MessageBox.Show("No process data in db!");
                this.ProgramList.Items.Refresh();
            }

            //关闭数据库的连接
            conn.Close();
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
}