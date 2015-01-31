using System;
using System.Data;
using System.Data.SqlClient;
using Repository.DBClass;
using Repository.abstraction;

namespace Repository
{
    public class KartuDokterReport : RepositoryBase
    {
        public DataSet[] GetCustomerControlReportData(int departementId, int customerId, string month, string year, DateTime startingDate, DateTime printDate)
        {
            DataSet[] dataSetArray = new DataSet[2];
            DataSet dataSetResult;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("KARTU_DOKTER_HEADER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@CustomerId", customerId);
                DBClass.AddSimpleParameter(cmd, "@Month", month);
                DBClass.AddSimpleParameter(cmd, "@Year", year);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                dataSetResult          = new DataSet();
                adapter.Fill(dataSetResult, "KartuDokterHeader");
                dataSetArray[0]        = dataSetResult;
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("KARTUDOKTER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@startingDate", startingDate);
                DBClass.AddSimpleParameter(cmd, "@CustomerId", customerId);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                dataSetResult           = new DataSet();
                adapter.Fill(dataSetResult, "KartuDokter");
                dataSetArray[1]         = dataSetResult;
            }
            return dataSetArray;
        }

        public DataSet GetCustomerControlForExcell(int departementId, int customerId, string month, string year, DateTime startingDate, DateTime printDate)
        {
            DataSet dataSetResult = new DataSet();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("KARTU_DOKTER_HEADER_EXCELL") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", departementId);
                DBClass.AddSimpleParameter(cmd, "@CustomerId", customerId);
                DBClass.AddSimpleParameter(cmd, "@Month", month);
                DBClass.AddSimpleParameter(cmd, "@Year", year);
                DBClass.AddSimpleParameter(cmd, "@PrintDate", printDate);
                SqlDataAdapter adapter  = new SqlDataAdapter(cmd);
                var table               = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("KARTUDOKTER") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@startingDate", startingDate);
                DBClass.AddSimpleParameter(cmd, "@CustomerId", customerId);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                var table              = new DataTable();
                adapter.Fill(table);
                dataSetResult.Tables.Add(table);
            }
            return dataSetResult;
        }
    }
}
