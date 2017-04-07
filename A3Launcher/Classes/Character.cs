using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3Launcher.Classes
{
    public class Character : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string name { get; set; }
        public string @class { get; set; }
        public string karma { get; set; }
        public string clan { get; set; }
        public string clan_rank { get; set; }
        public string clan_serial { get; set; }
        public int experience { get; set; }
        public string current_area { get; set; }
        public string creation_time { get; set; }
        public string login_time { get; set; }
        public string logout_time { get; set; }
        public string locked { get; set; }
        public string sID { get; set; }
        public string gender { get; set; }
        public bool god { get; set; }
        public bool staff { get; set; }
        public bool hardcore { get; set; }
        public string Password { get; set; }
        public void save(Character Char)
        {
            string json = JsonConvert.SerializeObject(Char, Formatting.Indented);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

}
