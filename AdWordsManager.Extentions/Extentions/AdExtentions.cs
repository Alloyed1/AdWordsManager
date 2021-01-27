using AdWordsManager.Data.DTo;
using AdWordsManager.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

namespace AdWordsManager.Extentions.Extentions
{
    public static class AdExtentions
    {
        public static NormalizeAd Normalize(this Ad self)
        {
            var ad = new NormalizeAd()
            {
                AccountNumber = self.AccountNumber,
                CPM = self.CPM,
                Link = self.Link,
                MetricBudget = self.MetricBudget,
                Status = self.Status,
                MetricView = self.MetricView,
                Name = self.Name,
                Id = self.Id,

            };

            var str = self.Budget.Replace(" ", "");
            str = str.Remove(str.Length - 1, 1);

            ad.Budget = decimal.Parse(str, CultureInfo.InvariantCulture);

            str = HttpUtility.HtmlEncode(self.View).Replace("&#160;", "");
            ad.View = int.Parse(str);

            str = HttpUtility.HtmlEncode(self.PokazCount).Replace("&#160;", "");
            ad.PokazCount = int.Parse(str);

            return ad;
        }
        public static List<NormalizeAd> Normalize(this List<Ad> selfs)
        {

            var adList = new List<NormalizeAd>();

            foreach(var self in selfs)
            {
                adList.Add(self.Normalize());
            }

            return adList;
        }
    }
}
