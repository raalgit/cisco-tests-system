using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.Json;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xaml.Behaviors.Core;
using System.Windows.Threading;
using ICSharpCode.SharpZipLib.Zip;

namespace CiscoTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime startTime;
        private DateTime endTime;
        private DateTime completeTime;
        private DispatcherTimer timer;
        private string exportDirPath;
        private string testDirPath;

        public ObservableCollection<Test> Tests { get; set; }
        public int TestsAmount { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            TestsAmount = 2;
            this.import();
            QuestionListForm.ItemsSource = Tests;
            MainWin.KeyDown += MainWin_KeyDown;
            MainWin.WindowState = WindowState.Maximized;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            startTime = DateTime.Now;
            endTime = startTime.AddSeconds(100);

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var time = (endTime - DateTime.Now);
            if (time.TotalSeconds <= 0)
            {
                timer.Stop();
                completedTest();
                return;
            }
            TimerLbl.Content = "Осталось времени: " + time.Hours + ":" + time.Minutes + ":" + time.Seconds;
        }

        private void MainWin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ShowAnswereForSelected();
            }
        }

        private void ShowAnswereForSelected()
        {
            var item = QuestionListForm.SelectedItem as Test;
            if (item == null) return;
            foreach (var correctAns in item.CorrectAnswereIndexes)
            {
                item.Answers[correctAns].IsChecked = true;
            }
        }

        public void export()
        {
            string json = JsonSerializer.Serialize(this.Tests);
            File.WriteAllText("example.json", json);
        }

        public void import()
        {
            string zipPath = "export.zip";
            if (!File.Exists(zipPath))
            {
                var fileDialog = new Microsoft.Win32.OpenFileDialog();
                fileDialog.DefaultExt = ".zip";
                fileDialog.Filter = "zip arch|*.zip";
                var result = fileDialog.ShowDialog();
                if (result == true)
                {
                    zipPath = fileDialog.FileName;
                }
                else
                {
                    MessageBox.Show("Необходимо выбрать архив с тестом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            FastZip fastZip = new FastZip();
            string fileFilter = null;
            fastZip.Password = "test";

            testDirPath = System.IO.Path.GetDirectoryName(zipPath) + "/CiscoTests";
            try
            {
                fastZip.ExtractZip(zipPath, testDirPath, fileFilter);
            }
            catch (Exception er)
            {
                System.Windows.MessageBox.Show(er.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string json = File.ReadAllText(testDirPath + "/input.json");
            try
            {
                this.Tests = JsonSerializer.Deserialize<ObservableCollection<Test>>(json);
                foreach (var test in this.Tests)
                {
                    test.Init(testDirPath + "/");
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            completedTest();
        }

        private void completedTest() 
        {
            SendBtn.IsEnabled = false;
            completeTime = DateTime.Now;
            if (timer.IsEnabled) timer.Stop();
            float _maxPoints = Tests.Sum(x => x.Points);
            float _userPoints = 0;
            foreach (var test in Tests)
            {
                // Количество выбранных ответов
                int _userAnsweres = test.Answers.Where(x => x.IsChecked).Count();
                if (_userAnsweres == 0)
                {
                    MessageBox.Show("Введите все ответы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Количество баллов за вопрос
                float _pointsPerQuestion = test.Points;

                // Количество правильных ответов
                int _selectedCorrectAnsweres = 0;
                // Количество неправильных ответов
                int _selectedUncorrectAnsweres = 0;

                var _answereArray = test.Answers.ToArray();
                for (int i = 0; i < _answereArray.Length; i++)
                {
                    if (_answereArray[i].IsChecked)
                    {
                        if (test.CorrectAnswereIndexes.Contains(i)) _selectedCorrectAnsweres++;
                        else _selectedUncorrectAnsweres++;
                    }
                }

                var diff = (float)(_selectedCorrectAnsweres - _selectedUncorrectAnsweres);
                if (diff < 0) diff = 0;
                _userPoints += _pointsPerQuestion * diff / (float)test.CorrectAnswereIndexes.Count;
            }

            string result =
                $"Количество набранных баллов: {_userPoints}\n" +
                $"Максимальное количество баллов: {_maxPoints}\n" +
                $"Процент правильных ответов: {_userPoints / _maxPoints * 100}%\n" +
                $"Время выполнения: {(int)((completeTime - startTime).TotalSeconds)} c.";
            MessageBox.Show(result, "Результат тестирования", MessageBoxButton.OKCancel);
        }
    }
}
