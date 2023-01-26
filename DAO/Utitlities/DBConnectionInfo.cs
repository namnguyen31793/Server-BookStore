using System;
using System.Collections.Generic;
using System.Text;

namespace DAO.Utitlities
{
    public static class DBConnectionInfo
    {
        public static string Sql_AccountSystem_ConnectionString = "Data Source=" + ConfigDb.SQL_CONNECTION + ";Initial Catalog=AccountSystem;Persist Security Info=True;" + ConfigDb.SQL_PASS;

    }
}
