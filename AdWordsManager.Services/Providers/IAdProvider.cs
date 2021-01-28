using AdWordsManager.Data.POCO;
using System.Threading.Tasks;
using AdWordsManager.Context.Contexts;
using LinqToDB;
using System.Linq;
using AdWordsManager.Providers.BaseProvider;

namespace AdWordsManager.Providers.Providers
{
    public interface IAdProvider : IBaseModelProvider<NormalizeAd>
    {
        Task<NormalizeAd> FindAdByNameAndAccountId(string name, string accountname);
        Task UpdateAdMetric(NormalizeAd ad);
    }

    public sealed class AdProvider : BaseModelProvider<NormalizeAd>, IAdProvider
    {
        public override async Task Create(NormalizeAd ad)
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
                return await FirstOrDefault(f => f.Name == name && f.AccountNumber == accountname);
            }
        }

        public async Task UpdateAd(NormalizeAd ad)
        {
            using(var db = new AdsDb())
            {
                await db.UpdateAsync(ad);
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
