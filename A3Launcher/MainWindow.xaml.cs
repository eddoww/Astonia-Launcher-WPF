using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using A3Launcher.Properties;
using Newtonsoft.Json;
using A3Launcher.Classes;
using Squirrel;
using MessageBox = System.Windows.MessageBox;
using MySql.Data.MySqlClient;

namespace A3Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        BackgroundWorker Worker = new BackgroundWorker();
        
        ObservableCollection<Account> accounts =
            JsonConvert.DeserializeObject<ObservableCollection<Account>>(File.ReadAllText(AccountJsonFile));

        public static string ResourcePath = Directory.GetCurrentDirectory() + "\\Resources\\";
        public static string AccountJsonFile = ResourcePath + "accounts.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadAccounts();
#if !DEBUG

            try
            {
                Update();
            }
            catch (Exception er)
            {

                logger("Error with updating: " + er);
            }
#endif
            LblVersionLbl.Content = "V" + Assembly.GetEntryAssembly().GetName().Version;
        }

        static async void Update()
        {
            using (var mgr = new UpdateManager("http://ugaris.com/launcher/"))
            {
                await mgr.UpdateApp();
            }
        }

        private void LoadAccounts()
        {
            AccountTree.ItemsSource = accounts;
            AccountsDataGrid.ItemsSource = accounts;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(AccountJsonFile, json);
            //Properties.Settings.Default.Save();
        }

        private void OnItemSelect(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Character character = (Character) AccountTree.SelectedItem;
                LblCharNameValue.Content = character.name;
                LblCharLevelValue.Content = character.level;
                LblCharClassValue.Content = character.@class2;
                Lblhardcore.Content = character.IsHardcore;
                LblCharCreationDateValue.Content = character.creation_time_dt.ToLocalTime();
                LblCharLastSeenValue.Content = character.login_time_dt.ToLocalTime();
                LblCharClanRankValue.Content = character.clan_rank;
                LblCharClanNameValue.Content = character.clan;
            }
            catch (Exception er)
            {
                logger(er.ToString());
            }
        }

        private void BtnAddAccount_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                AddAccount AddAccWindow = new AddAccount {Owner = this};
                AddAccWindow.ShowDialog();
                if (AddAccWindow.DialogResult.HasValue && AddAccWindow.DialogResult.Value)
                {
                    string apiurl = "";
                    string supersecretstring = "";
                    switch (AddAccWindow.CmbServer.Text)
                    {
                        case "Ugaris":
                            apiurl = "https://ugaris.com/api/launcher.php";
                            supersecretstring = "839hqfiugbf8792398ahdoland8y23fg76481920jenaghbxnmkz9978g";
                            break;
                        case "A3Res":
                            apiurl = "http://a3res.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                        case "Reborn":
                            apiurl = "http://www.astoniareborn.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                        case "Invicta":
                            apiurl = "https://www.a3invicta.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                        default:
                            apiurl = "https://ugaris.com/api/launcher.php";
                            supersecretstring = "839hqfiugbf8792398ahdoland8y23fg76481920jenaghbxnmkz9978g";
                            break;
                    }
                    Account NewAccount = new Account();
                    string passhash =
                        Convert.ToBase64String(
                            new System.Security.Cryptography.SHA256Managed().ComputeHash(
                                Encoding.UTF8.GetBytes(AddAccWindow.TxtAccPassword.Text)));

                    NewAccount.CharacterList =
                        GetAccountCharactersJson(string.Format(
                            "{0}?Login={1}&password={2}&sprsecret={3}", apiurl,
                            AddAccWindow.TxtAccId.Text, passhash, supersecretstring));
                    NewAccount.LoginId = Int32.Parse(AddAccWindow.TxtAccId.Text);
                    NewAccount.Password = AddAccWindow.TxtAccPassword.Text;
                    NewAccount.Name = AddAccWindow.TxtAccName.Text;
                    NewAccount.Email = AddAccWindow.TxtAccEmail.Text;
                    NewAccount.AddedOn = DateTime.Now;
                    NewAccount.Server = AddAccWindow.CmbServer.Text;

                    accounts.Add(NewAccount);
                }
                else
                {
                    MessageBox.Show("User clicked Cancel");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("You must use a number as Login ID");
                logger(ex.ToString());
            }
            catch (WebException er)
            {
                MessageBox.Show(
                    "There was an error with the API, Contact the server owner or check your internet connection");
                logger(er.ToString());
            }
            catch (Exception er)
            {
                MessageBox.Show(
                    "An error occured, please try again. If error persists please contact Server Owner. error: " + er);
                logger(er.ToString());
            }
        }

        private void BtnEditAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Account EditAcc = (Account) AccountsDataGrid.SelectedItem;
                AddAccount AddAccWindow = new AddAccount
                {
                    Owner = this,
                    TxtAccName = {Text = EditAcc.Name},
                    TxtAccId = {Text = EditAcc.LoginId.ToString()},
                    TxtAccEmail = {Text = EditAcc.Email},
                    TxtAccPassword = {Text = EditAcc.Password}
                };
                AddAccWindow.ShowDialog();

                if (AddAccWindow.DialogResult.HasValue && AddAccWindow.DialogResult.Value)
                {
                    string apiurl = "";
                    string supersecretstring = "";
                    switch (AddAccWindow.CmbServer.Text)
                    {
                        case "Ugaris":
                            apiurl = "https://ugaris.com/api/launcher.php";
                            supersecretstring = "839hqfiugbf8792398ahdoland8y23fg76481920jenaghbxnmkz9978g";
                            break;
                        case "A3Res":
                            apiurl = "http://a3res.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                        case "Reborn":
                            apiurl = "http://www.astoniareborn.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                        case "Invicta":
                            apiurl = "https://www.a3invicta.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                        default:
                            apiurl = "https://ugaris.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                    }
                    string passhash =
                        Convert.ToBase64String(
                            new System.Security.Cryptography.SHA256Managed().ComputeHash(
                                Encoding.UTF8.GetBytes(AddAccWindow.TxtAccPassword.Text)));
                    Account Changedacc = new Account
                    {
                        Name = AddAccWindow.TxtAccName.Text,
                        LoginId = Int32.Parse(AddAccWindow.TxtAccId.Text),
                        Email = AddAccWindow.TxtAccEmail.Text,
                        Password = AddAccWindow.TxtAccPassword.Text,
                        CharacterList =
                            GetAccountCharactersJson(string.Format("{0}?Login={1}&password={2}&sprsecret={3}", apiurl,
                                AddAccWindow.TxtAccId.Text, passhash, supersecretstring)),
                        Server = AddAccWindow.CmbServer.Text,
                        AddedOn = DateTime.Now
                    };

                    accounts.Remove(EditAcc);
                    accounts.Add(Changedacc);
                }
                else
                {
                    logger("User Canceled");
                }
            }
            catch (Exception er)
            {
                logger(er.ToString());
            }
        }

        private void BtnRemoveAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Account RemAcc = (Account) AccountsDataGrid.SelectedItem;
                accounts.Remove(RemAcc);
            }
            catch (Exception er)
            {
                logger(er.ToString());
            }
        }

        public void logger(string message)
        {
            DateTime now = DateTime.Now;
            if (!File.Exists("log.log"))
            {
                File.WriteAllText("log.log", $"{now.ToLocalTime()} : {message}\r\n");
            }
            else
            {
                try
                {
                    File.AppendAllText("log.log", $"{now.ToLocalTime()} : {message}\r\n");
                }
                catch (Exception er)
                {
                    MessageBox.Show("Please close the logging file <3");
                    throw;
                }
            }
        }

        private List<Character> GetAccountCharactersJson(string uri)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(uri);
            webRequest.UserAgent =
                @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            webRequest.Accept = "1";
            var webResponse = (HttpWebResponse) webRequest.GetResponse();
            var reader = new StreamReader(webResponse.GetResponseStream());
            string s = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<Character>>(s);
        }

        private void BtnUgarisBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TxtUgarisPath.Text = dialog.SelectedPath;
                Settings.Default.Save();
            }
        }

        private void BtnRefreshAccounts_Click(object sender, RoutedEventArgs e)
        {
            // Worker.WorkerReportsProgress = true;
            // Worker.DoWork += Worker_DoWork;
            // Worker.ProgressChanged += Worker_ProgressChanged;
            // Worker.RunWorkerAsync();
            // Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            //Task t = new Task(RefreshAccounts);
            //t.Start();
            //t.Wait();
            RefreshAccounts();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            RefreshBar.Value = e.ProgressPercentage;
            TxBlProgress.Text = (string) e.UserState;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //RefreshBar.Value = 0;
            //TxBlProgress.Text = "";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            worker.ReportProgress(0, String.Format("Refreshing Account 1."));
            //for (int i = 0; i < 10; i++)
            //{
            //    Thread.Sleep(1000);
            //    worker.ReportProgress((i + 1) * 10, String.Format("Processing Iteration {0}.", i + 2));
            //}
            RefreshAccounts();

            worker.ReportProgress(100, "Refreshing Done");
        }

        private void RefreshAccounts()
        {
            int total = accounts.Count;
            int current = 0;
            foreach (Account account in accounts)
            {
                current++;
                string apiurl = "";
                string supersecretstring = "";
                switch (account.Server)
                {
                    case "Ugaris":
                        apiurl = "https://ugaris.com/api/launcher.php";
                        supersecretstring = "839hqfiugbf8792398ahdoland8y23fg76481920jenaghbxnmkz9978g";
                        break;
                    case "A3Res":
                        apiurl = "http://a3res.com/api/launcher.php";
                        supersecretstring = "";
                        break;
                    case "Reborn":
                        apiurl = "http://www.astoniareborn.com/api/launcher.php";
                        supersecretstring = "";
                        break;
                    case "Invicta":
                        apiurl = "https://www.a3invicta.com/api/launcher.php";
                        supersecretstring = "";
                        break;
                    default:
                        apiurl = "https://ugaris.com/api/launcher.php";
                        supersecretstring = "";
                        break;
                }
                try
                {
                    string passhash =
                        Convert.ToBase64String(
                            new System.Security.Cryptography.SHA256Managed().ComputeHash(
                                Encoding.UTF8.GetBytes(account.Password)));
                    account.CharacterList =
                        GetAccountCharactersJson(string.Format("{0}?Login={1}&password={2}&sprsecret={3}",
                            apiurl, account.LoginId, passhash, supersecretstring));
                }
                catch (Exception er)
                {
                    logger("Error in refreshing: " + er);
                }
                //Worker.ReportProgress((current * 100) / total, String.Format("Refreshing Account {0}.", current + 2));
            }
        }

        private void BtnFullScreenFix_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo _processStartInfo = new ProcessStartInfo
            {
                FileName = @"sdbinst.exe",
                Arguments = $"-q \"Resources\fullscreenfix.sdb\""
            };
            logger(_processStartInfo.Arguments);
            try
            {
                Process myProcess = Process.Start(_processStartInfo);
            }
            catch (Exception er)
            {
                logger(er.ToString());
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(TxtUgarisPath.Text + "/moac.exe"))
            {
                try
                {
                    Character character = (Character) AccountTree.SelectedItem;
                    //System.Diagnostics.Process.Start(txta3path.Text + "/moac.exe", string.Format("-u {0} -p {1}", "eddow", "123456"));
                    ProcessStartInfo _processStartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = TxtUgarisPath.Text,
                        FileName = @"moac.exe",
                        Arguments = string.Format("-u {0} -p {1}", character.name, character.Password)
                    };
                    //select path based on server
                    //MessageBox.Show(_processStartInfo + character.name + character.sID + accinfo);
                    Process myProcess = Process.Start(_processStartInfo);
                }
                catch (Exception ex)
                {
                    logger(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not find moac.exe in specified folder.");
            }
        }

        private void BtnPlayWindowed_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(TxtUgarisPath.Text + "/moac.exe"))
            {
                try
                {
                    Character character = (Character) AccountTree.SelectedItem;
                    //System.Diagnostics.Process.Start(txta3path.Text + "/moac.exe", string.Format("-u {0} -p {1}", "eddow", "123456"));
                    ProcessStartInfo _processStartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = TxtUgarisPath.Text,
                        FileName = @"moac.exe",
                        Arguments = string.Format("-u {0} -p {1} -w", character.name, character.Password)
                    };
                    //select path based on server
                    //MessageBox.Show(_processStartInfo + character.name + character.sID + accinfo);
                    Process myProcess = Process.Start(_processStartInfo);
                }
                catch (Exception ex)
                {
                    logger(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not find moac.exe in specified folder.");
            }
        }

        private double ExpToLevel(int experience)
        {
            return Math.Floor(Math.Pow(experience + 1, 0.25));
        }

        private void BtnPlayWindowed_Multi_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(TxtUgarisPath.Text + "/moac.exe"))
            {
                try
                {
                    Character character = (Character) AccountTree.SelectedItem;
                    //System.Diagnostics.Process.Start(txta3path.Text + "/moac.exe", string.Format("-u {0} -p {1}", "eddow", "123456"));
                    ProcessStartInfo _processStartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = TxtUgarisPath.Text,
                        FileName = @"moac.exe",
                        Arguments = string.Format("-u {0} -p {1} -w -x", character.name, character.Password)
                    };
                    //select path based on server
                    //MessageBox.Show(_processStartInfo + character.name + character.sID + accinfo);
                    Process myProcess = Process.Start(_processStartInfo);
                }
                catch (Exception ex)
                {
                    logger(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not find moac.exe in specified folder.");
            }
        }

        private void BtnPlay_Multi_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(TxtUgarisPath.Text + "/moac.exe"))
            {
                try
                {
                    Character character = (Character) AccountTree.SelectedItem;
                    //System.Diagnostics.Process.Start(txta3path.Text + "/moac.exe", string.Format("-u {0} -p {1}", "eddow", "123456"));
                    ProcessStartInfo _processStartInfo = new ProcessStartInfo
                    {
                        WorkingDirectory = TxtUgarisPath.Text,
                        FileName = @"moac.exe",
                        Arguments = string.Format("-u {0} -p {1} -x", character.name, character.Password)
                    };
                    //select path based on server
                    //MessageBox.Show(_processStartInfo + character.name + character.sID + accinfo);
                    Process myProcess = Process.Start(_processStartInfo);
                }
                catch (Exception ex)
                {
                    logger(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not find moac.exe in specified folder.");
            }
        }
    }
}