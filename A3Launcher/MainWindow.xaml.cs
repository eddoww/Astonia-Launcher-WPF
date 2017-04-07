using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using A3Launcher.Properties;
using Newtonsoft.Json;
using A3Launcher.Classes;
using Squirrel;
using MessageBox = System.Windows.MessageBox;

namespace A3Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        BackgroundWorker Worker = new BackgroundWorker();
        ObservableCollection<Account> accounts = JsonConvert.DeserializeObject<ObservableCollection<Account>>(File.ReadAllText(AccountJsonFile));
        public static string ResourcePath = Directory.GetCurrentDirectory() + "\\Resources\\";
        public static string AccountJsonFile = ResourcePath + "accounts.json";
        public MainWindow()
        {
            InitializeComponent();
            LoadAccounts();
#if !DEBUG
     Update();
#endif
            LblVersionLbl.Content = "V" +Assembly.GetEntryAssembly().GetName().Version;
        }

        static async void Update()
        {
            using (var mgr = new UpdateManager("http://ugaris.com/launcher/"))
            {
                await mgr.UpdateApp();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(TxtUgarisPath.Text + "/moac.exe"))
            {
                try
                {
                    Character character = (Character)AccountTree.SelectedItem;
                    //System.Diagnostics.Process.Start(txta3path.Text + "/moac.exe", string.Format("-u {0} -p {1}", "eddow", "123456"));
                    ProcessStartInfo _processStartInfo = new ProcessStartInfo();
                    _processStartInfo.WorkingDirectory = TxtUgarisPath.Text;  //select path based on server
                    _processStartInfo.FileName = @"moac.exe";
                    _processStartInfo.Arguments = string.Format("-u {0} -p {1}", character.name, character.Password);
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
        private void LoadAccounts()
        {
            AccountTree.ItemsSource = accounts;
            AccountsDataGrid.ItemsSource = accounts;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(AccountJsonFile, json);
            Properties.Settings.Default.Save();
        }
        private void OnItemSelect(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                Character character = (Character)AccountTree.SelectedItem;
                LblCharNameValue.Content = character.name;
                LblCharLevelValue.Content = character.experience;
                LblCharClassValue.Content = character.@class;
                LblCharLocationValue.Content = character.current_area;
                LblCharCreationDateValue.Content = character.creation_time;
                LblCharLastSeenValue.Content = character.login_time;
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
                AddAccount AddAccWindow = new AddAccount();
                AddAccWindow.Owner = this;
                AddAccWindow.ShowDialog();
                if (AddAccWindow.DialogResult.HasValue && AddAccWindow.DialogResult.Value)
                {
                    string apiurl = "";
                    string supersecretstring = "";
                    switch (AddAccWindow.CmbServer.Text)
                    {
                        case "Ugaris":
                            apiurl = "http://ugaris.com/api/launcher.php";
                            supersecretstring = "";
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
                            apiurl = "http://ugaris.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                    }
                    Account NewAccount = new Account();
                    NewAccount.CharacterList =
                        GetAccountCharactersJson(string.Format(
                            "{0}?Login={1}&password={2}&sprsecret={3}", apiurl,
                            AddAccWindow.TxtAccId.Text, AddAccWindow.TxtAccPassword.Text, supersecretstring));
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
                MessageBox.Show("There was an error with the API, Contact the server owner or check your internet connection");
                logger(er.ToString());
            }
            catch (Exception er)
            {
                MessageBox.Show("An error occured, please try again. If error persists please contact Server Owner. error: " + er);
                logger(er.ToString());
            }
        }

        private void BtnEditAccount_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Account EditAcc = (Account)AccountsDataGrid.SelectedItem;
                AddAccount AddAccWindow = new AddAccount();
                AddAccWindow.Owner = this;
                AddAccWindow.TxtAccName.Text = EditAcc.Name;
                AddAccWindow.TxtAccId.Text = EditAcc.LoginId.ToString();
                AddAccWindow.TxtAccEmail.Text = EditAcc.Email;
                AddAccWindow.TxtAccPassword.Text = EditAcc.Password;
                AddAccWindow.ShowDialog();

                if (AddAccWindow.DialogResult.HasValue && AddAccWindow.DialogResult.Value)
                {
                    string apiurl = "";
                    string supersecretstring = "";
                    switch (AddAccWindow.CmbServer.Text)
                    {
                        case "Ugaris":
                            apiurl = "http://ugaris.com/api/launcher.php";
                            supersecretstring = "";
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
                            apiurl = "http://ugaris.com/api/launcher.php";
                            supersecretstring = "";
                            break;
                    }
                    Account Changedacc = new Account();
                    Changedacc.Name = AddAccWindow.TxtAccName.Text;
                    Changedacc.LoginId = Int32.Parse(AddAccWindow.TxtAccId.Text);
                    Changedacc.Email = AddAccWindow.TxtAccEmail.Text;
                    Changedacc.Password = AddAccWindow.TxtAccPassword.Text;
                    Changedacc.CharacterList = GetAccountCharactersJson(string.Format("{0}?Login={1}&password={2}&sprsecret={3}", apiurl, AddAccWindow.TxtAccId.Text, AddAccWindow.TxtAccPassword.Text, supersecretstring));
                    Changedacc.Server = AddAccWindow.CmbServer.Text;
                    Changedacc.AddedOn = DateTime.Now;

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
                Account RemAcc = (Account)AccountsDataGrid.SelectedItem;
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
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            var webResponse = (HttpWebResponse)webRequest.GetResponse();
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
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            Worker.WorkerReportsProgress = true;
            Worker.DoWork += Worker_DoWork;
            Worker.ProgressChanged += Worker_ProgressChanged;
            Worker.RunWorkerAsync();
        }
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            RefreshBar.Value = e.ProgressPercentage;
            TxBlProgress.Text = (string)e.UserState;
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
                        apiurl = "http://ugaris.com/api/launcher.php";
                        supersecretstring = "";
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
                        apiurl = "http://ugaris.com/api/launcher.php";
                        supersecretstring = "";
                        break;
                }
                try
                {
                    account.CharacterList =
                GetAccountCharactersJson(string.Format("{0}?Login={1}&password={2}&sprsecret={3}",
                    apiurl, account.LoginId, account.Password, supersecretstring));
                }
                catch (Exception er)
                {

                    logger("Error in refreshing: " + er);
                }
                Worker.ReportProgress((current * 100) / total, String.Format("Refreshing Account {0}.", current + 2));
            }
        }

        private void BtnFullScreenFix_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo _processStartInfo = new ProcessStartInfo();
            _processStartInfo.FileName = @"sdbinst.exe";
            _processStartInfo.Arguments = $"-q \"Resources\fullscreenfix.sdb\"";
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
    }
}
