using AdWordsManager.Data.DTo;
using AdWordsManager.Extentions.Extentions;
using AdWordsManager.Helper.Enums;
using AdWordsManager.Services.Services;
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

namespace AdWordsManager.Bot
{
    public static class SeleniumBot
    {
        private static IWebDriver _driver;
        private static List<string> _campaningUrls = new List<string>();
        private static IAdService _adService = new AdService();
        private static string _mainUrl;
        public static async Task Initialize()
        {
            
            var options = new ChromeOptions();
            options.AddArgument("--lang=ru");
            _driver = new ChromeDriver(options);
            _driver.Navigate().GoToUrl("https://ads.google.com/nav/login?subid=ru-ru-et-g-aw-a-tools-ma-awhp_xin1!o2");

            //await Work();
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
                    var dbAd = await _adService.FindAdByNameAndAccountId(ad.Name, ad.AccountNumber);

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

                    await _adService.AddAd(normAd);





                }
                catch(Exception ex)
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
            _mainUrl = _driver.Url;
            while (true)
            {
                //await Task.Delay(25* 1000);
                try
                {
                    
                    //_driver.FindElements(By.TagName("a")).FirstOrDefault(f => f.GetAttribute("minerva-id") == "Campaigns-tab").Click();
                    await Task.Delay(10 * 1000);

                    var campanings = _driver.FindElements(By.ClassName("ess-cell-link"));
                    var urlCamp = _driver.Url;

                    await CheckPageCountList();

                    var listData = _driver.FindElements(By.ClassName("particle-table-row")).Reverse().Skip(2);
                    await ParseAd(listData.ToList());

                    //foreach (var camp in campanings)
                    //{
                    //    camp.Click();
                    //    await Task.Delay(17 * 1000);
                    //    var groups = _driver.FindElements(By.ClassName("ess-cell-link"));
                    //    var urlGroup = _driver.Url;
                    //    foreach (var group in groups)
                    //    {
                    //        group.Click();
                    //        await Task.Delay(17 * 1000);
                    //        _driver.Navigate().GoToUrl(urlGroup);
                    //    }
                    //    _driver.Navigate().GoToUrl(urlCamp);
                    //    await Task.Delay(10 * 1000);
                    //}
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
