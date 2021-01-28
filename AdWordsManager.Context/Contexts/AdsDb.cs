using AdWordsManager.Data.POCO;
using LinqToDB;
using System;
using LinqToDB.Data;
using LinqToDB.DataProvider;

namespace AdWordsManager.Context.Contexts
{
    public class AdsDb : DataConnection
    {
        public AdsDb() : base(ProviderName.PostgreSQL95, @"Host=195.66.114.19;Port=5432;Database=db;Username=postgres;Password=cwcw4242") { }
        
        
        
        public ITable<NormalizeAd> Ads => GetTable<NormalizeAd>();
        public ITable<ManagerAccounts> ManagerAccounts => GetTable<ManagerAccounts>();
    }
}
