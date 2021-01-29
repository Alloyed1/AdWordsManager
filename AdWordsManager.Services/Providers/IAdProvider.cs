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
        Task<NormalizeAd> FindAdByNameAndIdAndManagerAccount(string name, string accountname, int managetAccountId);
        Task UpdateAdMetric(NormalizeAd ad);
    }

    public sealed class AdProvider : BaseModelProvider<NormalizeAd>, IAdProvider
    {
        public override async Task Create(NormalizeAd ad)
        {
            using(var db = new AdsDb())
            {
                var adDb = await FindAdByNameAndIdAndManagerAccount(ad.Name, ad.AccountNumber, ad.ManagerAccountId);
                if (adDb != null)
                {
                    await Update(ad);
                    return;
                }
                await db.InsertAsync(ad);
            }
        }

        public async Task<NormalizeAd> FindAdByNameAndIdAndManagerAccount(string name, string accountname, int managetAccountId)
        {
            using (var db = new AdsDb())
            {
                return await FirstOrDefault(f => f.Name == name && f.AccountNumber == accountname && f.ManagerAccountId == managetAccountId);
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
