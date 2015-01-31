using System;
using System.Data;
using System.Data.SqlClient;
using Repository.DBClass;
using Repository.abstraction;

namespace Repository
{
    public class AdjustmentReport : RepositoryBase
    {
        public DataSet[] GetCashAdjustmentReportData(int departementId, int cashId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_ADJUSTMENT_REPORT_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "AdjustmentHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_CASH_ADJUSTMENT_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CashId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "AdjustmentList");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetBankAdjustmentReportData(int departementId, int cashBankId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_BANKADJUSTMENT_REPORT_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@BankAccountId", cashBankId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "AdjustmentHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_BANK_ADJUSTMENT_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@BankAccountId", cashBankId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "AdjustmentList");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetStockAdjustmentReportData(int departementId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_ADJUSTMENT_REPORT_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "AdjustmentHeader");
                dataSetArray[0]         = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("REPORT_STOCK_ADJUSTMENT_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "AdjustmentList");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }
    }
}
