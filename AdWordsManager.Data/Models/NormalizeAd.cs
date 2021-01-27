using AdWordsManager.Helper.Enums;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AdWordsManager.Data.Models
{
    [Table("Ads")]
    public class NormalizeAd
    {

        [Key]
        [PrimaryKey, Identity]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Link")]
        public string Link { get; set; }
        [Column("cpm")]
        public string CPM { get; set; }
        [Column("Budget")]
        public decimal Budget { get; set; }
        [Column("MetricBudget")]
        public decimal MetricBudget { get; set; }
        [Column("View")]
        public int View { get; set; }
        [Column("MetricView")]
        public int MetricView { get; set; }
        [Column("AccountNumber")]
        public string AccountNumber { get; set; }
        [Column("PokazCount")]
        public int PokazCount { get; set; }
        [Column("Status")]
        public AdStatus Status { get; set; }
    }
}
