using System;
using System.Data;
using System.Data.SqlClient;
using Repository.DBClass;
using Repository.abstraction;

namespace Repository
{
    public class SalesReportRepository : RepositoryBase
    {
        public DataSet[] GetSalesByItemReportData(int departementId, int productId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_BY_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesHeader");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_SALES_BY_ITEM_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@ProductId", productId);
                DBClass.AddSimpleParameter(cmd, "@ByPassProduct", productId == 0 ? 1 : 0);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesByItem");
                dataSetArray[1] = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetSalesByOutletReportData(int departementId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_BY_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesHeader");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_SALES_BY_OUTLET_DATA") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesByOutlet");
                dataSetArray[1] = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet[] GetSalesByMarketingReportData(int departementId, int marketingId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_BY_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesHeader");
                dataSetArray[0] = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd         = DBClass.GetStoredProcedureCommand("REPORT_SALES_BY_OUTLET_MARKETING") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@MarketingId", marketingId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                var adapter     = new SqlDataAdapter(cmd);
                dataSetResult   = new DataSet();
                adapter.Fill(dataSetResult, "SalesByMarketing");
                dataSetArray[1] = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet GetSalesByMarketingForExcell(int departementId, int marketingId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            var dataSetResult = new DataSet();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_BY_MARKETING_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@MarketingId", marketingId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                var adapter = new SqlDataAdapter(cmd);
                var table   = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("REPORT_SALES_BY_OUTLET_MARKETING_EXCELL") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@MarketingId", marketingId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                var adapter = new SqlDataAdapter(cmd);
                var table   = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            return dataSetResult;
        }

        public DataSet GetSalesByOutletForExcell(int departementId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            var dataSetResult = new DataSet();
            using (DBClass    = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_BY_OUTLET_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                var adapter = new SqlDataAdapter(cmd);
                var table   = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("REPORT_SALES_BY_OUTLET_EXCELL") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                var adapter = new SqlDataAdapter(cmd);
                var table   = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            var dataRows = dataSetResult.Tables[1].Select();
            foreach (var dataRow in dataRows)
            {
                using (DBClass = new MSSQLDatabase())
                {
                    var cmd         = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_INVOICE_ITEM_AND_PRESENT_DATA") as SqlCommand;
                    DBClass.AddSimpleParameter(cmd, "@InvoiceId", dataRow[0]);
                    var adapter     = new SqlDataAdapter(cmd);
                    var table       = new DataTable();
                    table.TableName = "INVOICE-" + dataRow[0];
                    adapter.Fill(table);
                    dataSetResult.Tables.Add(table);
                }
            }
            return dataSetResult;
        }

        public DataSet GetSalesByProductForExcell(int departementId, int productId, DateTime printDate, DateTime dateStart, DateTime dateEnd)
        {
            var dataSetResult = new DataSet();
            using (DBClass    = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("REPORT_GET_SALES_BY_OUTLET_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                var adapter = new SqlDataAdapter(cmd);
                var table   = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("REPORT_SALES_BY_ITEM_EXCELL") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                DBClass.AddSimpleParameter(cmd, "@ProductId", productId);
                DBClass.AddSimpleParameter(cmd, "@ByPassProduct", productId == 0 ? 1 : 0);
                var adapter = new SqlDataAdapter(cmd);
                var table   = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            var dataRows = dataSetResult.Tables[1].Select();
            foreach (var dataRow in dataRows)
            {
                using (DBClass = new MSSQLDatabase())
                {
                    var cmd         = DBClass.GetStoredProcedureCommand("REPORT_SALES_BY_ITEM_DATA_EXCELL") as SqlCommand;
                    DBClass.AddSimpleParameter(cmd, "@ProductId", dataRow[0]);
                    DBClass.AddSimpleParameter(cmd, "@DateStart", dateStart);
                    DBClass.AddSimpleParameter(cmd, "@DateEnd", dateEnd);
                    var adapter     = new SqlDataAdapter(cmd);
                    var table       = new DataTable();
                    table.TableName = "PRODUCT-" + dataRow[0];
                    adapter.Fill(table);
                    dataSetResult.Tables.Add(table);
                }
            }
            return dataSetResult;
        }
    }
}
