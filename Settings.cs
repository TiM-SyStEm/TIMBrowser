using System;
using System.IO;
using System.Windows.Forms;
using System.Text.Json;

namespace TIMBrowser
{
    public partial class Settings : Form
    {
        public class SettingPar
        {
            public string searchSys { get; set; }
            public string startPage { get; set; }
            public bool saveHist { get; set; }
            public string saveType { get; set; }
            public bool saveDate { get; set; }
        }
        public Settings()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SettingPar setp = new SettingPar
            {
                searchSys = comboBox1.Text,
                startPage = comboBox2.Text,
                saveHist = checkBox1.Checked,
                saveType = comboBox3.Text,
                saveDate = checkBox2.Checked
            };
            string json = JsonSerializer.Serialize(setp);
            File.WriteAllText("browser/settings.json", json);
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            string[] hist = File.ReadAllLines("browser/history.txt");
            listBox1.Items.AddRange(hist);
            try
            {
                SettingPar setp = JsonSerializer.Deserialize<SettingPar>(File.ReadAllText("browser/settings.json"));
                comboBox1.Text = setp.searchSys;
                comboBox2.Text = setp.startPage;
                checkBox1.Checked = setp.saveHist;
                comboBox3.Text = setp.saveType;
                checkBox2.Checked = setp.saveDate;
            }
            catch (Exception ex) { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            File.WriteAllText("browser/history.txt", "");
            listBox1.Items.Clear();
        }
    }
}
