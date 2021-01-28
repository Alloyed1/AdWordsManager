using AdWordsManager.Providers.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AdWordsManager.Service.GoogleSpreadSheets
{
    public sealed class GoogleSpreadSheetService : IGoogleSpreadSheetService
    {
        private readonly IAdProvider _adProvider;
        public GoogleSpreadSheetService(IAdProvider adProvider)
        {
            _adProvider = adProvider;
        }

        public async Task Push()
        {
           //Сюда пиши код!!!!
        }
    }
}
