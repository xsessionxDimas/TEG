using System;
using System.Data;
using System.Data.SqlClient;
using Repository.DBClass;
using Repository.abstraction;

namespace Repository
{
    public class FlowRepository : RepositoryBase
    {
        public DataSet[] GetCashFlowReportData(int departementId, int cashId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_CASHFLOW_REPORT_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@CashId", cashId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "FlowHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_CASHFLOW_REPORT_DATA");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "FlowDetail");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetBankFlowReportData(int departementId, int cashBankId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_BANKFLOW_REPORT_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@BankAccountId", cashBankId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "BankFlowHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_BANKFLOW_REPORT_DATA");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@BankAccountId", cashBankId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "FlowDetail");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetStockFlowReportData(int departementId, int productId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_STOCKFLOW_REPORT_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "FlowHeader");
                dataSetArray[0]         = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_STOCKFLOW_REPORT_DATA");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@ByPassProduct", productId == 0 ? 1 : 0);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "StockFlowData");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetAllMoneyFlowReportData(int departementId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult = new DataSet();
                adapter.Fill(dataSetResult, "FlowHeader");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_ALL_MONEY");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult = new DataSet();
                adapter.Fill(dataSetResult, "MoneyFlow");
                dataSetArray[1] = dataSetResult;
            }
            return dataSetArray;
        }
        
        public DataSet GetCashFlowForExcell(int departementId, int cashId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_CASHFLOW_REPORT_HEADER_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@CashId", cashId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_CASHFLOW_REPORT_DATA_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            return dataSetResult;
        }

        public DataSet GetBankFlowExcell(int departementId, int cashBankId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_BANKFLOW_REPORT_HEADER_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@BankAccountId", cashBankId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_BANKFLOW_REPORT_DATA_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@BankAccountId", cashBankId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            return dataSetResult;
        }

        public DataSet GetStockFlowExcell(int departementId, int productId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass        = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_STOCKFLOW_REPORT_HEADER_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd          = DBClass.GetStoredProcedureCommand("REPORT_STOCKFLOW_REPORT_ITEM_EXCELL");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@ByPassProduct", productId == 0 ? 1 : 0);
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
                    SqlCommand cmd         = DBClass.GetStoredProcedureCommand("REPORT_STOCKFLOW_REPORT_DATA_EXCELL");
                    cmd.Parameters.AddWithValue("@DateStart", dateStart);
                    cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                    cmd.Parameters.AddWithValue("@StockId", dataRow[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    var table              = new DataTable();
                    table.TableName        = "STOCK-" + dataRow[0];
                    adapter.Fill(table);
                    dataSetResult.Tables.Add(table);
                }
            }
            return dataSetResult;
        }

        public DataSet GetMoneyFlowForExcell(int departementId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_HEADER");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                cmd.Parameters.AddWithValue("@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                var table = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("REPORT_ALL_MONEY");
                cmd.Parameters.AddWithValue("@DepartementId", departementId);
                cmd.Parameters.AddWithValue("@DateStart", dateStart);
                cmd.Parameters.AddWithValue("@DateEnd", dateEnd);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                var table = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            return dataSetResult;
        }
    }
}
