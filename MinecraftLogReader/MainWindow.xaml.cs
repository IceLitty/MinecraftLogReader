using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.IO;

namespace MinecraftLogReader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            logbox.Clear();
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += timer_tick;
            //Task task = new Task(() => {
            //    while (true)
            //    {
            //        taskTime++;
            //        copyFile(true);
            //        while (taskTime >= 50)
            //        {
            //            taskC();
            //            taskTime = 0;
            //        }
            //    }
            //});
            //task.Start();
        }

        //private void taskC() {
        //    if (canRunTask)
        //    {
        //        if (File.Exists(path))
        //        {
        //            refreshWithoutUI();
        //        }
        //    }
        //}

        //private int taskTime = 0;
        //private bool canRunTask = false;

        static System.Windows.Threading.DispatcherTimer timer;
        //private int timetick = 0;

        private void timer_tick(object sender, EventArgs e)
        {
            act();
            //if (withoutUIOutputChanged)
            //{
            //    logbox.Text += withoutUIOutput;
            //    withoutUIOutput = "";
            //    withoutUIOutputChanged = false;
            //}
        }

        private void act() 
        {
            for (int i = 0; i <= 10; i++)
            {
                if (i == 10)
                {
                    refreshLog();
                    copyFile(true);
                }
            }
        }

        private void autoRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (autoRefresh.IsChecked)
            {
                refresh.IsEnabled = false;
                //canRunTask = true;
                timer.Start();
            }
            else
            {
                refresh.IsEnabled = true;
                //canRunTask = false;
                timer.Stop();
            }
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            refreshLog();
        }

        private void choose_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "选择日志文件";
            openFileDialog.Filter = "log文件|*.log|所有文件|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.ShowDialog();
            path = openFileDialog.FileName;
            copyFile(true);
        }

        private void copyFile(bool load) 
        {
            try
            {
                if (File.Exists(appPathFile))
                {
                    File.Delete(appPathFile);
                }
                File.Copy(path, appPathFile);
                if (load)
                {
                    loadFile();
                }
            }
            catch (Exception) { }
        }

        private void loadFile() 
        {
            using (StreamReader sr = new StreamReader(appPathFile, Encoding.Default))
            {
                content.Clear();
                int lineCount = 0;
                while (sr.Peek() > 0)
                {
                    lineCount++;
                    string temp = sr.ReadLine();
                    content.Add(temp);
                }
            }
            contentIndex = content.Count();
            try
            {
                if (File.Exists(appPathFile))
                {
                    File.Delete(appPathFile);
                }
            }
            catch (Exception) { }
        }

        private string appPathFile = Directory.GetCurrentDirectory() + @"\latest.log";
        private string path = "";
        private List<string> content = new List<string>();
        private int contentIndex = 0;

        private void refreshLog() 
        {
            if (path == "")
            {
                string temp = "未选择日志文件！";
                this.ShowMessageAsync("警告", temp, MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确认", NegativeButtonText = "取消", AnimateShow = true });
            }
            else if(!File.Exists(path))
            {
                string temp = "日志文件不存在！";
                this.ShowMessageAsync("警告", temp, MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确认", NegativeButtonText = "取消", AnimateShow = true });
            }
            else
            {
                logbox.Text += refreshStr();
            }
        }

        //string withoutUIOutput = "";
        //bool withoutUIOutputChanged = false;

        //private void refreshWithoutUI() 
        //{
        //    withoutUIOutput = refreshStr();
        //    withoutUIOutputChanged = true;
        //}

        private string refreshStr() 
        {
            string output = "";
            int cInt = contentIndex;
            //刷新文件
            copyFile(true);
            if (contentIndex != cInt)
            {
                int eachLine = contentIndex - cInt;
                for (int i = 0; i < eachLine; i++)
                {
                    //判断新内容
                    if (content[cInt + i].IndexOf("/WARN]") != -1)
                    {
                        if (content[cInt + i].IndexOf("Can't keep up!") != -1)
                        {
                            string temp = content[cInt + i];
                            int tempa = temp.IndexOf("Running ");
                            int tempb = temp.IndexOf("ms behind");
                            int tempc = temp.IndexOf("skipping ");
                            int tempd = temp.IndexOf(" tick(s)");
                            output += temp.Substring(0, 10) + " 系统过载，运行" + temp.Substring(tempa + 8, tempb - tempa - 8) + "ms后跳过了" + temp.Substring(tempc + 9, tempd - tempc - 9) + "ticks。\r\n";
                        }
                    }
                    if (content[cInt + i].IndexOf("[CHAT] <") != -1)
                    {
                        string temp = content[cInt + i];
                        int tempa = temp.IndexOf("<");
                        int tempb = temp.IndexOf(">");
                        output += temp.Substring(0, 10) + " 玩家" + temp.Substring(tempa + 1, tempb - tempa - 1) + "说：" + temp.Substring(tempb + 2, temp.Length - tempb - 2) + "\r\n";
                    }
                }
            }
            return output;
        }
    }
}
