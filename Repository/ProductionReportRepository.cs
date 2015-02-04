using System;
using System.Data;
using System.Data.SqlClient;
using Repository.DBClass;
using Repository.abstraction;

namespace Repository
{
    public class ProductionReportRepository : RepositoryBase
    {
        public DataSet[] GetProductionReportData(int departementId, DateTime startingDate, DateTime endDate, DateTime printDate)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_PRODUCTION_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "ProductionHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_PRODUCTION_ITEMS");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "ProductionDetail");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetUsedSupplyReportData(int departementId, DateTime startingDate, DateTime endDate, DateTime printDate)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_PRODUCTION_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "ProductionHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_SUPPLIES_USING_ITEMS");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "ProductionDetail");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet GetProductionReportForExcell(int departementId, DateTime startingDate, DateTime endDate, DateTime printDate)
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass        = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_PRODUCTION_HEADER_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_PRODUCTION_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", startingDate);
                cmd.Parameters.AddWithValue("@DateEnd", endDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            var dataRows = dataSetResult.Tables[1].Select();
            foreach (var dataRow in dataRows)
            {
                using (DBClass = new MSSQLDatabase())
                {
                    SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_PRODUCTION_ITEMS_EXCELL");
                    cmd.Parameters.AddWithValue("@ProductionId", dataRow[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    var table              = new DataTable {TableName = "PRODUCTION-" + dataRow[0]};
                    adapter.Fill(table);
                    dataSetResult.Tables.Add(table);
                }
            }
            return dataSetResult;
        }
    }
}
