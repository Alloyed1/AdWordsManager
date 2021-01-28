using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AdWordsManager.Data.Base
{
    public abstract class PocoBase
    {
        [Key]
        [PrimaryKey, Identity]
        [Column("Id")]
        public int Id { get; set; }
    }
}
