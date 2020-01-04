using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace A3Launcher.Classes
{
    public class Character : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public string name { get; set; }

        public int @class { get; set; }

        public string @class2
        {
            get
            {
                if ((@class & (int) character_flag.mage) != 0 && (@class & (int) character_flag.warrior) != 0)
                {
                    if ((@class & (int) character_flag.arch) != 0)
                    {
                        return "Arch-Seyan Du";
                    }
                    else
                    {
                        return "Seyan Du";
                    }
                }
                else if ((@class & (int) character_flag.mage) != 0)
                {
                    if ((@class & (int) character_flag.arch) != 0)
                    {
                        return "Arch-Mage";
                    }
                    else
                    {
                        return "Mage";
                    }
                }
                else
                {
                    if ((@class & (int) character_flag.arch) != 0)
                    {
                        return "Arch-Warrior";
                    }
                    else
                    {
                        return "Warrior";
                    }
                }
            }
            set { }
        }

        public bool IsHardcore
        {
            get
            {
                if ((@class & (int) character_flag.hardcore) != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { }
        }

        public string karma { get; set; }
        public string clan { get; set; }
        public string clan_rank { get; set; }
        public string clan_serial { get; set; }

        public int experience { get; set; }

        public int level
        {
            get { return (int) ExpToLevel(experience); }
            set { }
        }

        private double ExpToLevel(int experience)
        {
            return Math.Floor(Math.Pow(experience + 1, 0.25));
        }

        public string current_area { get; set; }

        public string creation_time { get; set; }

        public DateTime creation_time_dt
        {
            get
            {
                double timestamp = double.Parse(creation_time);

                // Format our new DateTime object to start at the UNIX Epoch
                DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

                // Add the timestamp (number of seconds since the Epoch) to be converted
                dateTime = dateTime.AddSeconds(timestamp);
                return dateTime;
            }
        }

        public DateTime login_time_dt
        {
            get
            {
                double timestamp = double.Parse(login_time);

                // Format our new DateTime object to start at the UNIX Epoch
                DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

                // Add the timestamp (number of seconds since the Epoch) to be converted
                dateTime = dateTime.AddSeconds(timestamp);
                return dateTime;
            }
        }

        public DateTime logout_time_dt
        {
            get
            {
                double timestamp = double.Parse(logout_time);

                // Format our new DateTime object to start at the UNIX Epoch
                DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

                // Add the timestamp (number of seconds since the Epoch) to be converted
                dateTime = dateTime.AddSeconds(timestamp);
                return dateTime;
            }
        }

        public string login_time { get; set; }

        public string logout_time { get; set; }
        public string locked { get; set; }
        public string sID { get; set; }

        public string gender { get; set; }

        public bool god { get; set; }

        public bool staff { get; set; }

        public bool hardcore { get; set; }
        public string Password { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        [Flags]
        public enum character_flag
        {
            None = 0,
            immortal = 1 << 1,
            god = 1 << 2,
            player = 1 << 3,
            staff = 1 << 4,
            invisible = 1 << 5,
            shutup = 1 << 6,
            kicked = 1 << 7,
            update = 1 << 8,
            reserved0 = 1 << 9,
            reserved1 = 1 << 10,
            dead = 1 << 11,
            items = 1 << 12,
            respawn = 1 << 13,
            male = 1 << 14,
            female = 1 << 15,
            warrior = 1 << 16,
            mage = 1 << 17,
            arch = 1 << 18,
            reserved2 = 1 << 19,
            noattack = 1 << 20,
            hasname = 1 << 21,
            questitem = 1 << 22,
            infrared = 1 << 23,
            pk = 1 << 24,
            itemdeath = 1 << 25,
            nodeath = 1 << 26,
            nobody = 1 << 27,
            edemon = 1 << 28,
            fdemon = 1 << 29,
            idemon = 1 << 30,
            nogive = 1 << 31,
            playerlike = 1 << 32,
            reserved3 = 1 << 33,
            paid = 1 << 34,
            prof = 1 << 35,
            alive = 1 << 36,
            demon = 1 << 37,
            undead = 1 << 38,
            hardkill = 1 << 39,
            nobless = 1 << 40,
            areachange = 1 << 41,
            lag = 1 << 42,
            reserved4 = 1 << 43,
            thiefmode = 1 << 44,
            notell = 1 << 45,
            infravision = 1 << 46,
            nomagic = 1 << 47,
            nonomagic = 1 << 48,
            oxygen = 1 << 49,
            noplratt = 1 << 50,
            allowswap = 1 << 51,
            lwmaster = 1 << 52,
            hardcore = 1 << 53,
            nonotify = 1 << 54,
            smallupdate = 1 << 55,
            nowho = 1 << 56,
            won = 1 << 57,
            noexp = 1 << 58,
            developer = 1 << 59,
            eventmaster = 1 << 60
        }
    }
}