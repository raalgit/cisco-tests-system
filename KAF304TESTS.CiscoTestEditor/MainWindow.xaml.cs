using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using KAF304TESTS.CiscoTestEditor;

namespace KAF304TESTS.TestEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string testDirPath;
        private string exportDirPath;

        public ObservableCollection<Test> Tests { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainWin.WindowState = WindowState.Maximized;
            ExportTestsBtn.IsEnabled = false;
        }

        #region button events
        private void AddNewTestBtn_Click(object sender, RoutedEventArgs e)
        {
            var newTest = new Test();
            newTest.Answers = new ObservableCollection<Answere>();
            newTest.AnswersAmount = 0;
            newTest.CorrectAnswereIndexes = new List<int>();
            newTest.CorrectAnswersStr = "";
            newTest.HasImage = false;
            newTest.Question = "Вопрос";
            newTest.Type = 0;

            newTest.Init(testDirPath + "/");
            Tests.Add(newTest);
        }
        private void ExportTestsBtn_Click(object sender, RoutedEventArgs e)
        {
            exportManual();
        }
        private void ImportTestsBtn_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = ".zip";
            fileDialog.Filter = "zip arch|*.zip";
            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                importManual(fileDialog.FileName);
            }
        }
        #endregion

        #region import/export
        private void exportManual()
        {
            if (string.IsNullOrEmpty(testDirPath))
            {
                System.Windows.MessageBox.Show("Путь к директории не определен", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            using (StreamWriter writer = new StreamWriter(testDirPath + "/input.json", false))
            {
                string json = JsonSerializer.Serialize(this.Tests);
                writer.WriteLine(json);
            }

            if (!Directory.Exists(exportDirPath))
            {
                Directory.CreateDirectory(exportDirPath);
            }

            var destinationZipPath = exportDirPath + "/export_tests.zip";

            try
            {
                ZipManager.CreateZip(destinationZipPath, testDirPath);
                System.Windows.MessageBox.Show($"Тесты экспортированы! Путь к архиву: {destinationZipPath}", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception er)
            {
                System.Windows.MessageBox.Show(er.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void importManual(string path)
        {
            var zipFileName = path;
            exportDirPath = Path.GetDirectoryName(zipFileName) + "/ExportTests";
            testDirPath = Path.GetDirectoryName(zipFileName) + "/CiscoTests";
            if (!Directory.Exists(testDirPath))
            {
                Directory.CreateDirectory(testDirPath);
            }

            FastZip fastZip = new FastZip();
            string fileFilter = null;
            fastZip.Password = "test";

            try
            {
                fastZip.ExtractZip(zipFileName, testDirPath, fileFilter);
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
                QuestionListForm.ItemsSource = Tests;
                ExportTestsBtn.IsEnabled = true;
                System.Windows.MessageBox.Show($"Тесты импортированы! Путь к директории: {testDirPath}", "Импорт", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception er)
            {
                System.Windows.MessageBox.Show(er.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
