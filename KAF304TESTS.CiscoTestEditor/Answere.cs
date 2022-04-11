using KAF304TESTS.CiscoTestEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KAF304TESTS.TestEditor
{
    [Serializable]
    public class Answere : ObservableObject
    {
        /// <summary>
        /// Текст вопроса
        /// </summary>
        public string AnswereText { get; set; }

        /// <summary>
        /// Тег ответа
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Флаг выбора
        /// </summary>
        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropsChanged("IsChecked");
                Checked?.Invoke(this, null);
            }
        }

        [JsonIgnore]
        public EventHandler Checked { get; set; }
        [JsonIgnore]
        public EventHandler Removed { get; set; }

        public void Init()
        {
            DeleteAnswereCommand = new ActionCommand(() =>
            {
                Removed?.Invoke(this, null);
            });
        }
        #region commands
        [JsonIgnore]
        public ActionCommand DeleteAnswereCommand { get; set; }
        #endregion
    }
}
