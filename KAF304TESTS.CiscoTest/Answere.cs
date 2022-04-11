using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace CiscoTest
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
        public bool IsChecked { get { return isChecked; } 
            set { 
                isChecked = value; 
                OnPropsChanged("IsChecked");
                Checked?.Invoke(this, null);
            } 
        }

        [JsonIgnore]
        public EventHandler Checked { get; set; }
    }
}
