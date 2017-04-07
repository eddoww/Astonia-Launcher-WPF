using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3Launcher.Classes
{
    public class Account
    {
        public int Id { get; set; }
        public int LoginId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Character> CharacterList { get; set; }
        public DateTime AddedOn { get; set; }
        public string Server { get; set; }
            public int Characters
            {
                get { return CharacterList != null ? CharacterList.Count : 0; }
                set {  }
            }
    }
}
