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
    public class PackagingRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PACKAGING");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
                var listMaterial    = (param as Packaging).Materials;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PACKAGING_MATERIAL");
                        cmd.Parameters.AddWithValue("@PackagingId", id);
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
            var packaging  = param as Packaging;
            var result     = default(int);
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PACKAGING");
                cmd.Parameters.AddWithValue("@VoucherCode", packaging.VoucherCode);
                cmd.Parameters.AddWithValue("@DepartementId", packaging.DepartementID);
                cmd.Parameters.AddWithValue("@DestinationProduct", packaging.DestinationProduct);
                cmd.Parameters.AddWithValue("@DestinationQty", packaging.DestinationQty);
                cmd.Parameters.AddWithValue("@Note", packaging.Note);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("@PackagingDate", packaging.PackagingDate);
                cmd.Parameters.AddWithValue("@PackagingId", packaging.PackagingID);
                try
                {
                    DBClass.ExecuteNonQuery(cmd);
                    var listMaterial = (param as Packaging).Materials;
                    foreach (var material in listMaterial)
                    {
                        SaveRawMaterial(packaging.PackagingID, material);
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PACKAGING");
                        cmd.Parameters.AddWithValue("@PackagingId", id);
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
            var result     = new List<Packaging>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd  = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PACKAGING");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var packaging             = new Packaging();
                    packaging.PackagingID     = int.Parse(reader[0].ToString());
                    packaging.VoucherCode     = reader[1].ToString();
                    packaging.PackagingDate   = DateTime.Parse(reader[2].ToString());
                    packaging.DestinationCode = reader[3].ToString();
                    packaging.DestinationName = reader[4].ToString();
                    packaging.DestinationQty  = decimal.Parse(reader[5].ToString());
                    packaging.Note            = reader[7].ToString();
                    result.Add(packaging);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<RawMaterial> GetRawMaterial(int packagingId)
        {
            var result     = new List<RawMaterial>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PACKAGING_MATERIAL");
                cmd.Parameters.AddWithValue("@PackagingId", packagingId);
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
            var packaging  = new Packaging();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PACKAGING_BY_ID");
                cmd.Parameters.AddWithValue("@PackagingId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                   
                    packaging.PackagingID        = int.Parse(reader[0].ToString());
                    packaging.VoucherCode        = reader[1].ToString();
                    packaging.DepartementID      = int.Parse(reader[2].ToString());
                    packaging.DestinationProduct = int.Parse(reader[3].ToString());
                    packaging.DestinationQty     = decimal.Parse(reader[4].ToString());
                    //packaging.DestinationDate  = DateTime.Parse(reader[5].ToString());
                    packaging.Note               = reader[6].ToString();
                    packaging.PackagingDate      = DateTime.Parse(reader[7].ToString());
                }
            }
            return packaging as T;
        }

        /* product code generator helper */
        public string GetNewVoucherCode()
        {
            string ProformaCode = "PAC/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETPACKAGINGCODENUMBER");
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
