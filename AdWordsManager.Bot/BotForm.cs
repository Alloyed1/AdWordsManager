
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdWordsManager.Data.POCO;
using AdWordsManager.Extentions.Extentions;
using AdWordsManager.Providers.Providers;



namespace AdWordsManager.Bot
{
    public partial class BotForm : Form
    {
        private readonly IAdProvider _adService;
        public BotForm()
        {
            InitializeComponent();
            _adService = new AdProvider();
        }

        private  void startButton_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() => SeleniumBot.Work());
        }

        private async  void BotForm_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = await SeleniumBot.GetAllManagerAccounts();
            _ = Task.Run(() => SeleniumBot.Initialize());
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var account = (ManagerAccounts)comboBox1.SelectedItem;
            SeleniumBot.SetManagerAccount(account);
        }

        private async void BotForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private async void BotForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
            await SeleniumBot.ChangeBusyAccountManager(false);
        }
    }
}
