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
    public class ProductionPackagingRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCTION_PACKAGING");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
                var listMaterial    = (param as ProductionPackaging).Materials;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCTION_PACKAGING_MATERIAL");
                        cmd.Parameters.AddWithValue("@ProductionPackagingId", id);
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
            var productionPackaging  = param as ProductionPackaging;
            var result     = default(int);
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PRODUCTION_PACKAGING");
                cmd.Parameters.AddWithValue("@VoucherCode", productionPackaging.VoucherCode);
                cmd.Parameters.AddWithValue("@DepartementId", productionPackaging.DepartementID);
                cmd.Parameters.AddWithValue("@DestinationProduct", productionPackaging.DestinationProduct);
                cmd.Parameters.AddWithValue("@DestinationQty", productionPackaging.DestinationQty);
                cmd.Parameters.AddWithValue("@Note", productionPackaging.Note);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                cmd.Parameters.AddWithValue("@ProductionPackagingDate", productionPackaging.ProductionPackagingDate);
                cmd.Parameters.AddWithValue("@ProductionPackagingId", productionPackaging.ProductionPackagingID);
                try
                {
                    DBClass.ExecuteNonQuery(cmd);
                    var listMaterial = (param as ProductionPackaging).Materials;
                    foreach (var material in listMaterial)
                    {
                        SaveRawMaterial(productionPackaging.ProductionPackagingID, material);
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PRODUCTION_PACKAGING");
                        cmd.Parameters.AddWithValue("@ProductionPackagingId", id);
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
            var result     = new List<ProductionPackaging>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PRODUCTION_PACKAGING");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader      = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var productionPackaging                       = new ProductionPackaging();
                    productionPackaging.ProductionPackagingID     = int.Parse(reader[0].ToString());
                    productionPackaging.VoucherCode               = reader[1].ToString();
                    productionPackaging.ProductionPackagingDate   = DateTime.Parse(reader[2].ToString());
                    productionPackaging.DestinationCode           = reader[3].ToString();
                    productionPackaging.DestinationName           = reader[4].ToString();
                    productionPackaging.DestinationQty            = decimal.Parse(reader[5].ToString());
                    productionPackaging.Note                      = reader[7].ToString();
                    result.Add(productionPackaging);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<RawMaterial> GetRawMaterial(int packagingId)
        {
            var result     = new List<RawMaterial>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PRODUCTION_PACKAGING_MATERIAL");
                cmd.Parameters.AddWithValue("@ProductionPackagingId", packagingId);
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
            var productionPackaging  = new ProductionPackaging();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PRODUCTION_PACKAGING_BY_ID");
                cmd.Parameters.AddWithValue("@ProductionPackagingId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                   
                    productionPackaging.ProductionPackagingID   = int.Parse(reader[0].ToString());
                    productionPackaging.VoucherCode             = reader[1].ToString();
                    productionPackaging.DepartementID           = int.Parse(reader[2].ToString());
                    productionPackaging.DestinationProduct      = int.Parse(reader[3].ToString());
                    productionPackaging.DestinationQty          = decimal.Parse(reader[4].ToString());
                    productionPackaging.Note                    = reader[6].ToString();
                    productionPackaging.ProductionPackagingDate = DateTime.Parse(reader[7].ToString());
                }
            }
            return productionPackaging as T;
        }

        /* product code generator helper */
        public string GetNewVoucherCode()
        {
            string voucherCode = "PPAC/" + DateTime.Now.Year + "/" + StringManipulation.ChangeToRomeNumber(DateTime.Now.Month) + "/";
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("GETPRODUCTIONPACKAGINGCODENUMBER");
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    voucherCode += reader[0].ToString();
                }
            }
            return voucherCode;
        }
    }
}
