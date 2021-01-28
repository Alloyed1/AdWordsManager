using AdWordsManager.Data.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AdWordsManager.Data.POCO
{
    [Table("ManagerAccounts")]
    public sealed class ManagerAccounts : PocoBase
    {
        [Column("Name")]
        public string Name { get; set; }
        [Column("IsBusy")]
        public bool IsBusy { get; set; }
    }
}
