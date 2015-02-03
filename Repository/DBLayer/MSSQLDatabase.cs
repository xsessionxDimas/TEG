using System.Data.Common;

namespace Repository.DBClass
{
    public class MSSQLDatabase : AbstractDatabase
    {
        public MSSQLDatabase() : base(DbProviderFactories.GetFactory("System.Data.SqlClient")) { }

        protected override string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["TamanEdenApp"].ConnectionString;
        }
    }
}
