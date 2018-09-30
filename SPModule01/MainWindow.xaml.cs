
using SPModule01.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace SPModule01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Model1 db = new Model1();
        public List<Process> allProcesses = Process.GetProcesses(".").ToList();
        public bool flag = false;
        public bool ChFlag = false;
        public int k = 0;
        public List<Process> ids;
        public List<int> ids2;
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += OnTimerTick;
            timer.Start();
            DataProcessListView.ItemsSource = allProcesses;
            ids2 = new List<int>();

        }

        private void OnTimerTick(object sender, object e)
        {
            ids = (Process.GetProcesses(".").Where(w => w.ProcessName.Contains("chrome")).ToList());
            Process chrome = Process.GetProcesses().FirstOrDefault(f => f.ProcessName == "chrome");
            
            double avg = 0;
            int cnt = db.Chromes.Count();
            if (cnt > 10)
                 avg = (double)db.Chromes.Average(a => a.Memory);
            foreach (Process item in ids)
            {
                if (item.WorkingSet64 > 2 * avg & cnt > 10)
                {
                   // item.Kill();
                    continue;
                }
                if (!ids2.Contains(item.Id))
                {
                    try
                    {
                        Chrome t = new Chrome();
                        t.ProcessName = item.ProcessName;
                        t.Memory = item.WorkingSet64;
                        db.Chromes.Add(t);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            ids2 = ids.Select(s => s.Id).ToList();


            if (!string.IsNullOrEmpty(NameSearchTB.Text)) flag = true;
            else flag = false;
            if (!flag)
            {
                if ((bool)NameSortRB.IsChecked)
                {
                    allProcesses = Process.GetProcesses(".").OrderBy(o => o.ProcessName).ToList();
                    DataProcessListView.ItemsSource = allProcesses;
                }

                else if ((bool)ThreadsSortRB.IsChecked)
                {
                    allProcesses = Process.GetProcesses(".").OrderBy(o => o.Threads.Count).ToList();
                    DataProcessListView.ItemsSource = allProcesses;
                }

                else if ((bool)MemorySortRB.IsChecked)
                {
                    allProcesses = Process.GetProcesses(".").OrderBy(o => o.WorkingSet64).ToList();
                    DataProcessListView.ItemsSource = allProcesses;
                }

                else
                {
                    allProcesses = Process.GetProcesses(".").ToList();

                    DataProcessListView.ItemsSource = allProcesses;
                }
            }
            else
            {
                allProcesses = Process.GetProcesses(".").Where(w => w.ProcessName.Contains(NameSearchTB.Text)).ToList();
                DataProcessListView.ItemsSource = allProcesses;
            }

            

        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process p = (Process)DataProcessListView.SelectedItem;
            if(p!=null)
            p.Kill();
        }

        private void DataProcessListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ThreadWrapPanel.Children.Clear();
            Process p = (Process)DataProcessListView.SelectedItem;
            if (p != null)
            {
                foreach (ProcessThread thread in p.Threads)
                {
                    Label temp = new Label();
                    temp.Content = thread.Id;
                    temp.Margin = new Thickness(10);
                    ThreadWrapPanel.Children.Add(temp);
                }
            }
           
        }

        private void ProcessStartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(ProcessStartTextBox.Text);
                ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                ErrorLabel.Text = "Start";
            }
            catch(Exception ex)
            {
                ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
                ErrorLabel.Text = ex.Message;
            }
        }

        private void ShowCPUButton_Click(object sender, RoutedEventArgs e)
        {
           // ShowCPUTextBlock.Text = "";
            Process p = (Process)DataProcessListView.SelectedItem;
            if (p != null)
            {
                PerformanceCounter counter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName, true);
                for (int i = 0; i < 20; i++)
                {
                    try
                    {
                      //  ShowCPUTextBlock.Text += counter.NextValue() / Environment.ProcessorCount + "\n";
                        Thread.Sleep(200);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

        }
    }
}
