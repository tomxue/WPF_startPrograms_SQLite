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

namespace QQ_Byhh
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Info> mPath = new List<Info>();
        //List<string> mName = new List<string>();

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