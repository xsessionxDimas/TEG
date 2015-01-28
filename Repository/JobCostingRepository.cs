using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using CustomTypes.Base;
using CustomTypes.Objects;
using Repository.DBClass;
using Repository.Enums;
using Repository.abstraction;
using Repository.tools;

namespace Repository
{
    public class JobCostingRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_JOB_COSTING");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
                var listMaterial    = (param as JobCosting).Materials;
                foreach (var material in listMaterial)
                {
                    SaveRawMaterial(objID, material);
                }
            }
            return objID;
        }

        private void SaveRawMaterial(int id, RawMaterial material)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    try
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_JOB_COSTING_MATERIAL");
                        cmd.Parameters.AddWithValue("@JobCostingId", id);
                        cmd.Parameters.AddWithValue("@ProductId", material.ProductID);
                        cmd.Parameters.AddWithValue("@Qty", material.Qty);
                        cmd.Parameters.AddWithValue("@Type", material.Type);
                        cmd.Parameters.AddWithValue("@MaterialDate", material.Date);    
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                    catch (Exception)
                    {
                        txn.Rollback();
                        throw;
                    }
                }
            }
        }

        public int UpdateRow(T param, string updatedBy)
        {
            var jobCosting = param as JobCosting;
            var result     = default(int);
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_JOB_COSTING");
                cmd.Parameters.AddWithValue("@VoucherCode", jobCosting.VoucherCode);
                cmd.Parameters.AddWithValue("@DepartementId", jobCosting.DepartementID);
                cmd.Parameters.AddWithValue("@DestinationProduct", jobCosting.DestinationProduct);
                cmd.Parameters.AddWithValue("@DestinationQty", jobCosting.DestinationQty);
                cmd.Parameters.AddWithValue("@DestinationDate", jobCosting.DestinationDate);
                cmd.Parameters.AddWithValue("@Note", jobCosting.Note);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("@JobCostingDate", jobCosting.JobCostingDate);
                cmd.Parameters.AddWithValue("@JobCostingId", jobCosting.JobCostingID);
                try
                {
                    DBClass.ExecuteNonQuery(cmd);
                    var listMaterial = (param as JobCosting).Materials;
                    foreach (var material in listMaterial)
                    {
                        SaveRawMaterial(jobCosting.JobCostingID, material);
                    }
                }
                catch (Exception)
                {
                    result = 1;
                }
            }
            return result;
        }

        public int DeleteRow(int id, string updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_JOB_COSTING");
                        cmd.Parameters.AddWithValue("@JobCostingId", id);
                        cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                }
            }
            catch (Exception)
            {
                result = 1;
            }
            return result;
        }

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<JobCosting>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd  = DBClass.GetStoredProcedureCommand("APP_GET_ALL_JOB_COSTING");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var jobCosting             = new JobCosting();
                    jobCosting.JobCostingID    = int.Parse(reader[0].ToString());
                    jobCosting.VoucherCode     = reader[1].ToString();
                    jobCosting.JobCostingDate  = DateTime.Parse(reader[2].ToString());
                    jobCosting.DestinationCode = reader[3].ToString();
                    jobCosting.DestinationName = reader[4].ToString();
                    jobCosting.DestinationQty  = decimal.Parse(reader[5].ToString());
                    jobCosting.DestinationDate = DateTime.Parse(reader[6].ToString());
                    jobCosting.Note            = reader[7].ToString();
                    result.Add(jobCosting);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<RawMaterial> GetRawMaterial(int jobCostingId)
        {
            var result     = new List<RawMaterial>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_JOB_COSTING_MATERIAL");
                cmd.Parameters.AddWithValue("@JobCostingId", jobCostingId);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var item         = new RawMaterial();
                    item.ProductID   = int.Parse(reader[0].ToString());
                    item.ProductCode = reader[1].ToString();
                    item.ProductName = reader[2].ToString();
                    item.Qty         = decimal.Parse(reader[3].ToString());
                    item.Type        = int.Parse(reader[4].ToString());
                    item.Date        = DateTime.Parse(reader[5].ToString());
                    item.UnitName    = reader[6].ToString();
                    result.Add(item);
                }
            }
            return result;
        }

        public T FindbyId(int id)
        {
            var jobCosting  = new JobCosting();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_JOB_COSTING_BY_ID");
                cmd.Parameters.AddWithValue("@JobCostingId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                   
                    jobCosting.JobCostingID       = int.Parse(reader[0].ToString());
                    jobCosting.VoucherCode        = reader[1].ToString();
                    jobCosting.DepartementID      = int.Parse(reader[2].ToString());
                    jobCosting.DestinationProduct = int.Parse(reader[3].ToString());
                    jobCosting.DestinationQty     = decimal.Parse(reader[4].ToString());
                    jobCosting.DestinationDate    = DateTime.Parse(reader[5].ToString());
                    jobCosting.Note               = reader[6].ToString();
                    jobCosting.JobCostingDate     = DateTime.Parse(reader[7].ToString());
                    
                }
            }
            return jobCosting as T;
        }

        /* product code generator helper */
        public string GetNewVoucherCode()
        {
            string ProformaCode = "JBC/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETJOBCOSTINGCODENUMBER");
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    ProformaCode += reader[0].ToString();
                }
            }
            return ProformaCode;
        }
    }
}
