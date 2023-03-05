using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using static TIMBrowser.Settings;
using System.Text.RegularExpressions;
using System.Web;

namespace TIMBrowser
{
    public partial class TIMBrowser : Form
    {
        ChromiumWebBrowser chrome;
        Settings.SettingPar setp;
        string adress;
        public TIMBrowser()
        {
            InitializeComponent();
        }
        public void AddHistory(string site)
        {
            if (setp.saveHist)
            {
                if (setp.saveDate)
                {
                    DateTime dt = DateTime.UtcNow;
                    File.AppendAllText("browser/history.txt", "\n" + site + "\t" + dt.ToString("HH:mm dd.MM.yy"));
                }
                else 
                    File.AppendAllText("browser/history.txt", "\n" + site);
            }
        }
        private void TIMBrowser_Load(object sender, EventArgs e)
        {
            try
            {
                setp = JsonSerializer.Deserialize<SettingPar>(File.ReadAllText("browser/settings.json"));

            }
            catch(Exception ex)
            {
                setp = new SettingPar
                {
                    searchSys = "Яндекс",
                    startPage = "ya.ru",
                    saveHist = true,
                    saveType = "Адрес",
                    saveDate = false
                };
            }
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            chrome = new ChromiumWebBrowser("https://" + setp.startPage);
            chrome.AddressChanged += Chrome_AddressChanged;
            chrome.TitleChanged += Chrome_TitleChanged;
            tabControl1.SelectedTab.Controls.Add(chrome);
        }

        private void Chrome_TitleChanged(object sender, TitleChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                tabControl1.SelectedTab.Text = e.Title;
                if(setp.saveType == "Адрес")
                {
                    AddHistory(adress);
                }
                else
                {
                    AddHistory(e.Title);
                }
            }));
        }

        private void Chrome_AddressChanged(object sender, AddressChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                tabControl1.SelectedTab.Text = e.Address;
                textBox1.Text = e.Address;
                adress = e.Address;
            }));
        }

        private void TIMBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TabPage tb = new TabPage();
            tb.Text = "Новая вкладка";
            ChromiumWebBrowser chrome = new ChromiumWebBrowser("https://" + setp.startPage);
            chrome.AddressChanged += Chrome_AddressChanged;
            chrome.TitleChanged += Chrome_TitleChanged;
            tb.Controls.Add(chrome);
            tabControl1.TabPages.Add(tb);
            tabControl1.SelectTab(tabControl1.TabCount-1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBox1.Text, @"^http\w*"))
            {
                
                ChromiumWebBrowser chrome = tabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
                if (chrome != null)
                    chrome.Load(textBox1.Text);
            }
            else
            {
                if (setp.searchSys == "Яндекс")
                {
                    chrome.Load("https://yandex.ru/search/?text=" + textBox1.Text);
                }
                else if (setp.searchSys == "Google")
                {
                    chrome.Load("https://www.google.ru/search?q=" + textBox1.Text);
                }
                else if (setp.searchSys == "Mail.ru")
                {
                    chrome.Load("https://mail.ru/search?search_source=mailru_desktop_safe&text=" + textBox1.Text);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChromiumWebBrowser chrome = tabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
            if (chrome != null)
                chrome.Reload();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ChromiumWebBrowser chrome = tabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
            if (chrome != null && chrome.CanGoBack)
                chrome.Back();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ChromiumWebBrowser chrome = tabControl1.SelectedTab.Controls[0] as ChromiumWebBrowser;
            if (chrome != null && chrome.CanGoForward)
                chrome.Forward();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }
    }
}
