using System;
using System.Data;
using System.Data.SqlClient;
using Repository.DBClass;
using Repository.abstraction;

namespace Repository
{
    public class DepositSalesReport : RepositoryBase
    {
        public DataSet[] GetReportData(DateTime startingDate, DateTime endDate, DateTime printDate)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_DEPOSIT_SALES_HISTORY_HEADER");
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "DepositSalesHistoryHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_DEPOSIT_SALES_HISTORY");
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "DepositSalesHistory");
                dataSetArray[1]        = dataSetResult;
            }
            return dataSetArray;
        }
    }
}
