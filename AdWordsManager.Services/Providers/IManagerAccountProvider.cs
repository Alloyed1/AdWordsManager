using AdWordsManager.Data.DTO;
using AdWordsManager.Data.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AdWordsManager.Providers.BaseProvider;

namespace AdWordsManager.Providers.Providers
{
    public interface IManagerAccountProvider : IBaseModelProvider<ManagerAccounts>
    {
        
    }
    public class ManagerAccountProvider : BaseModelProvider<ManagerAccounts>, IManagerAccountProvider
    {

    }
}
