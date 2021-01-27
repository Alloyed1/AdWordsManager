using AdWordsManager.Helper.Enums;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AdWordsManager.Data.DTo
{

    public class Ad
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string CPM { get; set; }
        public string Budget { get; set; }
        public decimal MetricBudget { get; set; }
        public string View { get; set; }
        public int MetricView { get; set; }
        public string AccountNumber { get; set; }
        public string PokazCount { get; set; }
        public AdStatus Status { get; set; }

    }
}
