using AdWordsManager.Data.POCO;
using LinqToDB;
using System;

namespace AdWordsManager.Context.Contexts
{
    public class AdsDb : LinqToDB.Data.DataConnection
    {
        public AdsDb() : base(ProviderName.PostgreSQL95, @"Host=195.66.114.19;Port=5432;Database=db;Username=postgres;Password=cwcw4242") { }

        public ITable<NormalizeAd> Ads => GetTable<NormalizeAd>();
    }
}
