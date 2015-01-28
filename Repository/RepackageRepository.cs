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
    public class RepackageRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_STOCK_REPACKAGE");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
            }
            return objID;
        }

        public int UpdateRow(T param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_STOCK_REPACKAGE");
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            /* bypass compiler error need to be updated soon */
            return 0;
        }

        public int DeleteRow(int id, string updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction) DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_STOCK_REPACKAGE");
                        cmd.Parameters.AddWithValue("@RepackageId", id);
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
            var result     = new List<Repackage>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_STOCK_REPACKAGE");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var repackage             = new Repackage();
                    repackage.RepackageID     = int.Parse(reader[0].ToString());
                    repackage.VoucherCode     = reader[1].ToString();
                    repackage.RepackageDate   = DateTime.Parse(reader[2].ToString());
                    repackage.SourceName      = reader[3].ToString();
                    repackage.SourceQty       = decimal.Parse(reader[4].ToString());
                    repackage.DestinationName = reader[5].ToString();
                    repackage.DestinationQty  = decimal.Parse(reader[6].ToString());
                    repackage.Note            = reader[7].ToString();
                    result.Add(repackage);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var repackage  = new Repackage();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_STOCK_REPACKAGE_BY_ID");
                cmd.Parameters.AddWithValue("@RepackageId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    repackage.RepackageID        = int.Parse(reader[0].ToString());
                    repackage.VoucherCode        = reader[1].ToString();
                    repackage.RepackageDate      = DateTime.Parse(reader[2].ToString());
                    repackage.DepartementId      = int.Parse(reader[3].ToString());
                    repackage.SourceProduct      = int.Parse(reader[4].ToString());
                    repackage.SourceQty          = decimal.Parse(reader[5].ToString());
                    repackage.DestinationProduct = int.Parse(reader[6].ToString());
                    repackage.DestinationQty     = decimal.Parse(reader[7].ToString());
                    repackage.Note               = reader[8].ToString();
                }
            }
            return repackage as T;
        }

        /* product code generator helper */
        public string GetNewRepackageVoucherCode()
        {
            string ProformaCode = "RPK/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETREPACKAGINGCODENUMBER");
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    ProformaCode += reader[0].ToString();
                }
            }
            return ProformaCode;
        }
    }
}
