using KAF304TESTS.CiscoTestEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KAF304TESTS.TestEditor
{
    [Serializable]
    public class Test : ObservableObject
    {
        /// <summary>
        /// Номер теста
        /// </summary>
        public int QuestionNumber { get { return questionNumber; } set { questionNumber = value; OnPropsChanged("QuestionNumber"); } }

        /// <summary>
        /// Вопрос
        /// </summary>
        public string Question { get { return question; } set { question = value; OnPropsChanged("Question"); } }

        /// <summary>
        /// Путь к изображению вопроса
        /// </summary>
        public string ImagePath { get { return imagePath; } set { imagePath = value; OnPropsChanged("ImagePath"); } }

        /// <summary>
        /// Один ответ к вопросу
        /// </summary>
        public bool SingleAnswere { get { return singleAnswere; } set { singleAnswere = value; OnPropsChanged("SingleAnswere"); } }

        /// <summary>
        /// Вопрос с картинкой или без нее
        /// </summary>
        public bool HasImage { get { return hasImage; } set { hasImage = value; OnPropsChanged("HasImage"); } }


        /// <summary>
        /// Список возможных ответов
        /// </summary>
        public ObservableCollection<Answere> Answers { get { return answers; } set { answers = value; OnPropsChanged("Answers"); } }

        /// <summary>
        /// Количество необходимых ответов
        /// </summary>
        public int AnswersAmount { get { return answersAmount; } set { answersAmount = value; OnPropsChanged("AnswersAmount"); } }

        /// <summary>
        /// Правильные ответы
        /// </summary>
        public List<int> CorrectAnswereIndexes { get { return correctAnswereIndexes; } set { correctAnswereIndexes = value; OnPropsChanged("CorrectAnswereIndexes"); } }

        /// <summary>
        /// Количетсво баллов за вопрос
        /// </summary>
        public int Points { get { return points; } set { points = value; OnPropsChanged("Points"); } }

        /// <summary>
        /// Тип теста
        /// </summary>
        public int Type { get { return type; } set { type = value; OnPropsChanged("Type"); } }


        [JsonIgnore]
        public System.Windows.Visibility ImageVisibility { get; set; }
        [JsonIgnore]
        public System.Windows.Visibility SingleAnswereFormVisibility { get; set; }
        [JsonIgnore]
        public System.Windows.Visibility MultiAnswereFormVisibility { get; set; }
        [JsonIgnore]
        public System.Windows.Media.ImageSource Image { get; set; }
        [JsonIgnore]
        public string CorrectAnswersStr { 
            get { return correctAnswersStr; } 
            set { 
                correctAnswersStr = value; 
                OnPropsChanged("CorrectAnswersStr");
                try 
                {
                    var indexes = correctAnswersStr.Split(',').ToList();
                    if (indexes != null && indexes.Count > 0)
                    {
                        CorrectAnswereIndexes = new List<int>();
                        foreach (var index in indexes)
                        {
                            int correctAnswere = default;
                            if (int.TryParse(index, out correctAnswere))
                            {
                                CorrectAnswereIndexes.Add(correctAnswere);
                            }
                        }
                    }
                }
                catch (Exception er)
                {

                }
            } 
        }


        private int questionNumber;
        private string question;
        private string imagePath;
        private bool singleAnswere;
        private bool hasImage;
        private ObservableCollection<Answere> answers;
        private int answersAmount;
        private List<int> correctAnswereIndexes;
        private int points;
        private int type;
        public string correctAnswersStr;


        #region commands
        [JsonIgnore]
        public ActionCommand AddNewAnswereCommand { get; set; }
        #endregion

        #region methods
        public void AddAnswere()
        {
            var count = Answers.Count;
            var newAnswere = new Answere()
            {
                AnswereText = "Ответ",
                IsChecked = false,
                Tag = $"{QuestionNumber}_{count + 1}"
            };
            newAnswere.Checked += OnAnswerChange;
            newAnswere.Removed += OnAnswerRemoved;
            newAnswere.Init();
            Answers.Add(newAnswere);
        }

        public void Init(string dirPath)
        {
            this.ImageVisibility = this.HasImage ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.SingleAnswereFormVisibility = this.SingleAnswere ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.MultiAnswereFormVisibility = this.SingleAnswere ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            if (!string.IsNullOrEmpty(this.ImagePath)) this.Image = this.LoadImage(dirPath + this.ImagePath);

            foreach (var answer in this.Answers)
            {
                answer.Init();
                answer.Checked += OnAnswerChange;
                answer.Removed += OnAnswerRemoved;
            }

            AddNewAnswereCommand = new ActionCommand(() =>
            {
                AddAnswere();
            });

            CorrectAnswersStr = string.Join(',', CorrectAnswereIndexes);
        }
        public void OnAnswerChange(object sender, EventArgs arg)
        {
            if (!this.SingleAnswere) return;

            var tag = (sender as Answere).Tag.ToString();

            Answers.Where(x => x.IsChecked && x.Tag != tag).ToList().ForEach(x => x.IsChecked = false);
        }
        public void OnAnswerRemoved(object sender, EventArgs args)
        {
            Answers.Remove(sender as Answere);
            int index = 0;
            foreach (var answere in Answers)
            {
                index++;
                answere.Tag = $"{QuestionNumber}_{index}";
            }
        }
        public ImageSource LoadImage(string fileName)
        {
            var image = new BitmapImage();

            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }

            return image;
        }
        #endregion
    }
}
