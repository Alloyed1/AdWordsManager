using AdWordsManager.Providers.Providers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdWordsManager.Data.DTO;

namespace AdWordsManager.Service.GoogleSpreadSheets
{
    public sealed class GoogleSpreadSheetService : IGoogleSpreadSheetService
    {
        private readonly IAdProvider _adProvider;
        public GoogleSpreadSheetService(IAdProvider adProvider)
        {
            _adProvider = adProvider;
        }
        private SheetsService GetService()
        {
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            var ApplicationName = "Google Sheets API .NET Quickstart";
            UserCredential credential;
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {

                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return  service;
        }
        public async Task Push()
        {
            var ads = await _adProvider.GetAll();
            String spreadsheetId = "1jjZXZC9i1jO3s2Zug82bP1bwPOhI_c8As-y4QFdhSsU";
            String range = "A:I";
            //var service = GetService();
           
            //List<Request> requests = new List<Request>();

         

            //BatchUpdateSpreadsheetRequest busr = new BatchUpdateSpreadsheetRequest
            //{
            //    Requests = requests
            //};

            //service.Spreadsheets.BatchUpdate(busr, spreadsheetId).Execute();
        }
    }
}
