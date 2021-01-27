
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdWordsManager.Extentions.Extentions;
using AdWordsManager.Services.Services;



namespace AdWordsManager.Bot
{
    public partial class BotForm : Form
    {
        private static IAdService _adService = new AdService();
        public BotForm()
        {
            InitializeComponent();

        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() => SeleniumBot.Work());
        }

        private async void BotForm_Load(object sender, EventArgs e)
        {
            //var html = HttpUtility.HtmlDecode("18 500").Replace(" ","");
            //var ads = await _adService.GetAds();
            _ = Task.Run(() => SeleniumBot.Initialize());
           
        }
    }
}
