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
using AdWordsManager.Helper.Enums;

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

        private static CellFormat GetCellFormat(bool bold,bool needColor=false, AdStatus status = AdStatus.Start )
        {
            var clr = new Color();
            if (status != AdStatus.Start)               
             clr = new Color { Red = 1 };
            else
             clr = new Color { Green = 1 };
            var color = needColor ? clr : null;
            return new CellFormat
            {
                TextFormat = new TextFormat
                {
                    Bold = bold
                },
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
                },
                BackgroundColor = color
                
            };
        }
        private void DeleteAllSheets(SheetsService service,string spreadsheetId)
        {
            var SheetRequest = new List<DeleteSheetRequest>();
            var ssRequest = service.Spreadsheets.Get(spreadsheetId);
            Spreadsheet ss = ssRequest.Execute();
            foreach (Sheet sheet in ss.Sheets)
               SheetRequest.Add(new DeleteSheetRequest() { SheetId = sheet.Properties.SheetId });

            var addSheetRequest = new AddSheetRequest() 
            {

            };

            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = "Time";
            addSheetRequest.Properties.SheetId = 1337228;

            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request { AddSheet = addSheetRequest });
            var batchUpdateRequest = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
            batchUpdateRequest.Execute();

            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            foreach (var e in SheetRequest)
            {
                batchUpdateSpreadsheetRequest.Requests.Add(new Request { DeleteSheet = e });
            }
            
            batchUpdateRequest = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
            batchUpdateRequest.Execute();
        }
        private List<Request> AddRequest(List<Request> requests,List<string> dataList, int sheetId, int rowIndex,bool border, bool boldFont=false, AdStatus status = AdStatus.Start)
        {
            List<CellData> values = new List<CellData>();
            var borderFormat = border ? GetCellFormat(boldFont) :null;
            foreach (var s in dataList)
                values.Add(new CellData
                {                       
                    UserEnteredValue = new ExtendedValue
                    {
                        StringValue = s
                    },
                    UserEnteredFormat = ((dataList.IndexOf(s) == 3) && (s!="Ролик")) ? GetCellFormat(boldFont, true, status):borderFormat

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
                    Fields = "userEnteredValue, userEnteredFormat"
                }
            });
            requests.Add(new Request { 
                UpdateDimensionProperties = new UpdateDimensionPropertiesRequest
                {
                    Range = new DimensionRange
                    {
                        SheetId= sheetId,
                        Dimension = "COLUMNS",
                        StartIndex = 3,
                        EndIndex = 4
                    },
                    Properties = new DimensionProperties
                    {
                        PixelSize=450
                    },                    
                    Fields = "pixelSize"
                }
            });
            return requests;
        }
        public async Task Push()
        {
            var spreadsheetId = "1jjZXZC9i1jO3s2Zug82bP1bwPOhI_c8As-y4QFdhSsU";
            var service = GetService();
            DeleteAllSheets(service, spreadsheetId);

            var ads = await _adProvider.GetAll(x => x.ManagerAccount);
            
            var head = new List<string> { "Аккаунт", "Дата", "№", "Ролик", "Комментарий", "Просмотры", "Цена", "Бюджет", "Остаток" };
         
            var list = ads.GroupBy(x => x.ManagerAccountId).ToList();
            int newSheetId = 0;
            foreach (var managerAccount in list)
            {
                

                var nameList = ads.FirstOrDefault(f => f.ManagerAccountId == managerAccount.Key).ManagerAccount.Name;
                var addSheetRequest = new AddSheetRequest();
                addSheetRequest.Properties = new SheetProperties();
                addSheetRequest.Properties.Title = nameList;
                addSheetRequest.Properties.SheetId = newSheetId;
                BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
                batchUpdateSpreadsheetRequest.Requests = new List<Request>();
                batchUpdateSpreadsheetRequest.Requests.Add(new Request { AddSheet = addSheetRequest });
                var batchUpdateRequest = service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
                batchUpdateRequest.Execute();

                newSheetId++;
            }

            BatchUpdateSpreadsheetRequest busrq = new BatchUpdateSpreadsheetRequest();
            busrq.Requests = new List<Request>();
            busrq.Requests.Add(new Request { DeleteSheet = new DeleteSheetRequest() { SheetId = 1337228 } });
            var bt = service.Spreadsheets.BatchUpdate(busrq, spreadsheetId);
            bt.Execute();

            newSheetId = 0;
            foreach (var managerAccount in list)
            {
                List<Request> requests = new List<Request>();
                var s = managerAccount.GroupBy(x => x.AccountNumber).ToList(); int rowIndex = 0;
                foreach (var ma in managerAccount.GroupBy(x => x.AccountNumber))
                {
                    
                    requests = AddRequest(requests, head, newSheetId, rowIndex,true,true);
                    rowIndex++; var elementNumber = 1;
                    foreach (var el in ma)
                    {
                        var vals = new List<string> { el.AccountNumber, "", elementNumber.ToString(), el.Name, "", el.View.ToString(), el.Budget.ToString(), "","0" };
                                 requests = AddRequest(requests, vals, newSheetId, rowIndex,true,false,el.Status); 
                                  rowIndex++; elementNumber++;
                    }
                    requests = AddRequest(requests, new List<string> { "" }, newSheetId, rowIndex,false); rowIndex++;
                }
                BatchUpdateSpreadsheetRequest busr = new BatchUpdateSpreadsheetRequest
                {
                    Requests = requests
                };
                service.Spreadsheets.BatchUpdate(busr, spreadsheetId).Execute();
                newSheetId++;
            }    

                //newSheetId = 756598526;
                //int rowIndex = 0;
                //    foreach (var e in ads.GroupBy(x => x.AccountNumber))
                //    {

                //        requests = AddRequest(requests, head, newSheetId, rowIndex);
                //        rowIndex++;
                //        var elementNumber = 1;
                //        foreach (var el in e)
                //        {
                //            var vals = new List<string> { el.AccountNumber, "", elementNumber.ToString(), el.Name, "", el.View.ToString(), el.Budget.ToString(), "" };
                //            requests = AddRequest(requests, vals, newSheetId, rowIndex);
                //            rowIndex++; elementNumber++;
                //        }

                //        requests = AddRequest(requests, new List<string> { "" }, newSheetId, rowIndex); rowIndex++;
                //    }




             




        }
    }
}
