using AdWordsManager.Data.DTO;
using AdWordsManager.Data.POCO;
using AdWordsManager.Extentions.Extentions;
using AdWordsManager.Helper.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AdWordsManager.Providers.Providers;

namespace AdWordsManager.Bot
{
    public static class SeleniumBot
    {
        private static IWebDriver _driver;
        private static List<string> _campaningUrls = new List<string>();
        
        private static readonly IAdProvider _adProvider;
        private static readonly IManagerAccountProvider _managerAccountProvider;

        private static ManagerAccounts _managerAccount;
        
        private static string _mainUrl;


        static SeleniumBot()
        {
            _adProvider = new AdProvider();
            _managerAccountProvider = new ManagerAccountProvider();
        }


        public static async Task<IEnumerable<ManagerAccounts>> GetAllManagerAccounts()
        {
            return await _managerAccountProvider.Select<ManagerAccounts>(w => !w.IsBusy, s => s);
        }

        public static void SetManagerAccount(ManagerAccounts managerAccounts)
        {

            _managerAccount = managerAccounts;
        }
        
        public static async Task Initialize()
        {

            var options = new ChromeOptions();
            options.AddArgument("--lang=ru");
            _driver = new ChromeDriver(options);
            _driver.Navigate().GoToUrl("https://ads.google.com/nav/login?subid=ru-ru-et-g-aw-a-tools-ma-awhp_xin1!o2");

            //await Work();
        }

        public static void Dispose()
        {
            _driver.Close();
            _driver.Dispose();
            
        }

        public static async Task ChangeBusyAccountManager(bool busy)
        {
            if (_managerAccount == null) return;
            _managerAccount.IsBusy = busy;
            try
            {
                await _managerAccountProvider.Update(_managerAccount);
            }
            catch(Exception ex)
            {

            }
            
        }

        private static async Task ParseAd(List<IWebElement> listData)
        {

            foreach (var li in listData)
            {
                try
                {
                    var cells = li.FindElements(By.TagName("ess-cell"));
                    var ad = new Ad();
                    try
                    {
                        ad.Status = cells.FirstOrDefault(f => f.GetAttribute("essfield") == "status").FindElement(By.ClassName("aw-status")).GetAttribute("class").Contains("enabled") ? AdStatus.Start : AdStatus.Stop;
                    }
                    catch { }

                    //if (ad.Status == AdStatus.Stop) continue;
                    ad.Name = cells.FirstOrDefault(f => f.GetAttribute("essfield") == "name").FindElement(By.ClassName("ess-cell-link")).GetAttribute("textContent");
                    ad.View = HttpUtility.HtmlDecode(cells.FirstOrDefault(f => f.GetAttribute("essfield") == "stats.video_views").FindElement(By.TagName("div")).GetAttribute("textContent")).Replace(" ", "");
                    ad.CPM = cells.FirstOrDefault(f => f.GetAttribute("essfield") == "stats.average_cpv").GetAttribute("textContent");
                    ad.PokazCount = HttpUtility.HtmlDecode(cells.FirstOrDefault(f => f.GetAttribute("essfield") == "stats.impressions").GetAttribute("textContent")).Replace(" ", "");
                    ad.Budget = HttpUtility.HtmlDecode(cells.FirstOrDefault(f => f.GetAttribute("essfield") == "stats.cost").FindElement(By.TagName("div")).GetAttribute("textContent")).Replace("₽", "").Replace(",", ".").Replace(" ", "");
                    ad.AccountNumber = cells.FirstOrDefault(f => f.GetAttribute("essfield") == "entity_owner_info.descriptive_name").FindElement(By.ClassName("ess-cell-link")).GetAttribute("textContent");
                    var normAd = ad.Normalize();
                    var dbAd = await _adProvider.FindAdByNameAndIdAndManagerAccount(ad.Name, ad.AccountNumber, _managerAccount.Id);

                    if (dbAd != null)
                    {
                        if ((normAd.View >= dbAd.MetricView && dbAd.MetricView != 0) || (normAd.Budget >= dbAd.MetricBudget && dbAd.MetricBudget != 0) && ad.Status == AdStatus.Start)
                        {
                            cells.FirstOrDefault(f => f.GetAttribute("essfield") == "status").Click();
                            await Task.Delay(2000);
                            _driver.FindElements(By.TagName("span")).FirstOrDefault(f => f.Text == "Приостановить").Click();
                            await Task.Delay(5000);

                            ad.Status = AdStatus.Stop;
                        }
                    }
                    normAd.ManagerAccountId = _managerAccount.Id;
                    await _adProvider.Create(normAd);

                }
                catch
                {
                }
                
            }
        }
        private static async Task CheckPageCountList()
        {
            
            var countStrok = _driver.FindElements(By.TagName("material-dropdown-select")).ToList();
            if (countStrok.Count > 1)
            {
                try
                {
                    var el = countStrok[1].FindElement(By.TagName("dropdown-button"));
                    el.Click();
                    await Task.Delay(1000);
                    _driver.FindElements(By.TagName("material-select-dropdown-item"))?.Last().Click();
                    await Task.Delay(5 * 1000);
                }
                catch { }
                
            }
        }
        
        public static async Task Work()
        {
            await ChangeBusyAccountManager(true);
            await ChangeBusyAccountManager(false);
            _mainUrl = _driver.Url;
            while (true)
            {
                try
                {
                    await Task.Delay(10 * 1000);

                    var campanings = _driver.FindElements(By.ClassName("ess-cell-link"));
                    var urlCamp = _driver.Url;

                    await CheckPageCountList();

                    var listData = _driver.FindElements(By.ClassName("particle-table-row")).Reverse().Skip(2);
                    await ParseAd(listData.ToList());

                    _driver.Navigate().Refresh();

                }
                catch {
                    _driver.Navigate().GoToUrl(_mainUrl);
                    await Task.Delay(8 * 1000);
                }
            }
            
        }
    }
}
