using AdWordsManager.Data.Models;
using System;
using System.Threading.Tasks;
using AdWordsManager.Context.Contexts;
using LinqToDB;
using LinqToDB.Data;
using System.Collections.Generic;
using System.Linq;

namespace AdWordsManager.Services.Services
{
    public interface IAdService
    {
        Task<List<NormalizeAd>> GetAds();
        Task AddAd(NormalizeAd ad);
        Task<NormalizeAd> FindAdByNameAndAccountId(string name, string accountname);
        Task UpdateAd(NormalizeAd ad);
        Task UpdateAdMetric(NormalizeAd ad);
    }
    public class AdService : IAdService
    {
        public async Task AddAd(NormalizeAd ad)
        {
            using(var db = new AdsDb())
            {
                var adDb = await FindAdByNameAndAccountId(ad.Name, ad.AccountNumber);
                if (adDb != null)
                {
                    await UpdateAd(ad);

                    return;
                }

                await db.InsertAsync(ad);
            }
        }

        public async Task<NormalizeAd> FindAdByNameAndAccountId(string name, string accountname)
        {
            using (var db = new AdsDb())
            {
                return await db.Ads.FirstOrDefaultAsync(f => f.Name == name && f.AccountNumber == accountname);
            }
        }

        public async Task<List<NormalizeAd>> GetAds()
        {
            using (var db = new AdsDb())
            {
                
                return await db.Ads.ToListAsync();
            }
        }

        public async Task UpdateAd(NormalizeAd ad)
        {
            using(var db = new AdsDb())
            {
                await db.Ads.Where(w => w.Name == ad.Name)
                        .Set(s => s.Link, ad.Link)
                        .Set(s => s.PokazCount, ad.PokazCount)
                        .Set(s => s.Status, ad.Status)
                        .Set(s => s.View, ad.View)
                        .Set(s => s.CPM, ad.CPM)
                        .UpdateAsync();
            }
        }
        public async Task UpdateAdMetric(NormalizeAd ad)
        {
            using (var db = new AdsDb())
            {
                await db.Ads.Where(w => w.Name == ad.Name)
                        .Set(s => s.MetricView, ad.MetricView)
                        .Set(s => s.MetricBudget, ad.MetricBudget)
                        .UpdateAsync();
            }
        }
    }
}
