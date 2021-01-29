using AdWordsManager.Providers.Providers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Linq;
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
                new FileStream(@"C:\credentials.json", FileMode.Open, FileAccess.Read))
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

        private static CellFormat GetCellFormat()
        {
            return new CellFormat
            {
                Borders = new Borders
                {
                    Bottom = new Border
                    {
                        Style = "SOLID",
                        Color = new Color
                        {
                            Red = 0.1f,
                            Green = 0.1f,
                            Blue = 0.1f
                        }
                    },
                    Top = new Border
                    {
                        Style = "SOLID",
                        Color = new Color
                        {
                            Red = 0.1f,
                            Green = 0.1f,
                            Blue = 0.1f
                        }
                    },
                    Left = new Border
                    {
                        Style = "SOLID",
                        Color = new Color
                        {
                            Red = 0.1f,
                            Green = 0.1f,
                            Blue = 0.1f
                        }
                    },
                    Right = new Border
                    {
                        Style = "SOLID",
                        Color = new Color
                        {
                            Red = 0.1f,
                            Green = 0.1f,
                            Blue = 0.1f
                        }
                    }
                }
            };
        }
        private List<Request> AddRequest(List<Request> requests,List<string> dataList, int sheetId, int rowIndex)
        {
            List<CellData> values = new List<CellData>();
            foreach (var s in dataList)
                values.Add(new CellData
                {                       
                    UserEnteredValue = new ExtendedValue
                    {
                        StringValue = s
                    }
                });
            requests.Add(new Request
            {
                UpdateCells = new UpdateCellsRequest
                {
                    Start = new GridCoordinate
                    {
                        SheetId = sheetId,
                        RowIndex = rowIndex,
                        ColumnIndex = 0
                    },
                    Rows = new List<RowData> { new RowData { Values = values } },
                    Fields = "userEnteredValue"
                }

            });
           
            return requests;
        }
        public async Task Push()
        {
            var ads = await _adProvider.GetAll();
            List<Request> requests = new List<Request>();
            var head = new List<string> { "Аккаунт", "Дата", "№", "Ролик", "Комментарий", "Просмотры", "Цена", "Бюджет", "Остаток" };
            int rowIndex = 0;
            foreach (var e in ads.GroupBy(x => x.AccountNumber))
            {
                List<CellData> values = new List<CellData>();

                requests = AddRequest(requests, head, 0, rowIndex);                              
                rowIndex++;

                var elementNumber = 1;
                foreach (var el in e)
                {
                    var vals = new List<string> { el.AccountNumber, "", elementNumber.ToString(), el.Name, "", el.View.ToString(), el.Budget.ToString(), "" };
                    requests= AddRequest(requests, vals, 0, rowIndex);               
                    rowIndex++; elementNumber++;
                }

                values = new List<CellData>();
                requests = AddRequest(requests, new List<string> { "" }, 0, rowIndex); rowIndex++;
            }
            String spreadsheetId = "1jjZXZC9i1jO3s2Zug82bP1bwPOhI_c8As-y4QFdhSsU";

            BatchUpdateSpreadsheetRequest busr = new BatchUpdateSpreadsheetRequest
            {
                Requests = requests
            };

            var service = GetService();
            service.Spreadsheets.BatchUpdate(busr, spreadsheetId).Execute();


        }
    }
}
