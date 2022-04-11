using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CiscoTest
{
    [Serializable]
    public class Test : ObservableObject
    {
        public void Init() 
        {
            this.ImageVisibility = this.HasImage ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.SingleAnswereFormVisibility = this.SingleAnswere ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.MultiAnswereFormVisibility = this.SingleAnswere ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            if (!string.IsNullOrEmpty(this.ImagePath)) this.Image = this.LoadImage(this.ImagePath);

            foreach (var answer in this.Answers)
            {
                answer.Checked += OnAnswerChange;
            }
        }

        /// <summary>
        /// Номер теста
        /// </summary>
        public int QuestionNumber { get; set; }

        /// <summary>
        /// Вопрос
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Путь к изображению вопроса
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Один ответ к вопросу
        /// </summary>
        public bool SingleAnswere { get; set; }

        /// <summary>
        /// Вопрос с картинкой или без нее
        /// </summary>
        public bool HasImage { get; set; }


        /// <summary>
        /// Список возможных ответов
        /// </summary>
        public List<Answere> Answers { get; set; }

        /// <summary>
        /// Количество необходимых ответов
        /// </summary>
        public int AnswersAmount { get; set; }

        /// <summary>
        /// Правильные ответы
        /// </summary>
        public ObservableCollection<int> CorrectAnswereIndexes { get; set; }

        /// <summary>
        /// Количетсво баллов за вопрос
        /// </summary>
        public int Points { get; set; }


        [JsonIgnore]
        public System.Windows.Visibility ImageVisibility { get; set; }
        [JsonIgnore]
        public System.Windows.Visibility SingleAnswereFormVisibility { get; set; }
        [JsonIgnore]
        public System.Windows.Visibility MultiAnswereFormVisibility { get; set; }
        [JsonIgnore]
        public System.Windows.Media.ImageSource Image { get; set; }

        public void OnAnswerChange(object sender, EventArgs arg)
        {
            if (!this.SingleAnswere) return;

            var tag = (sender as Answere).Tag.ToString();

            Answers.Where(x => x.IsChecked && x.Tag != tag).ToList().ForEach(x => x.IsChecked = false);
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
    }
}
