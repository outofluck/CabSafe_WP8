using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabSafe.DataModel
{

    public class ContactNum
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ContactId { get; set; }

        public string ContactNumber { get; set; }
    }

}
