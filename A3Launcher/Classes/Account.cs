using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3Launcher.Classes
{
    public class Account : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int LoginId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        private List<Character> _CharacterList;
        public List<Character> CharacterList
        {
            get { return _CharacterList; }
            set
            {
                if (_CharacterList != value)
                {
                    _CharacterList = value;
                    NotifyPropertyChanged("CharacterList");
                }
            }
        }

        public DateTime AddedOn { get; set; }
        public string Server { get; set; }

        public int Characters
        {
            get { return CharacterList != null ? CharacterList.Count : 0; }
            set { }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}