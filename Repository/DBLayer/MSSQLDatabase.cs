using System.Data.SqlClient;

namespace Repository.DBClass
{
    public class MSSQLDatabase : AbstractDatabase<SqlConnection, SqlCommand, SqlDataAdapter>
    {
        protected override string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["TamanEdenApp"].ConnectionString;
        }
    }
}
