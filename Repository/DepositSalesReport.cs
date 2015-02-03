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
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_DEPOSIT_SALES_HISTORY_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DateStart", startingDate);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", endDate);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "DepositSalesHistoryHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_DEPOSIT_SALES_HISTORY") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DateStart", startingDate);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", endDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "DepositSalesHistory");
                dataSetArray[1]        = dataSetResult;
            }
            return dataSetArray;
        }
    }
}
