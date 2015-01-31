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
    public class SupplierRepository : RepositoryBase, IUniqueValidation, IRepository<Supplier>
    {
        public int SaveRow(Supplier param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_SUPPLIER") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
            }
            return objID;
        }

        public int UpdateRow(Supplier param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_SUPPLIER") as SqlCommand;
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
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
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_SUPPLIER") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@DepartementId", id);
                        DBClass.AddSimpleParameter(cmd, "@LastUpdatedBy", updatedBy);
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

        public IEnumerable<Supplier> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Supplier>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SUPPLIER") as SqlCommand;
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var suplier          = new Supplier();
                    suplier.SupplierID   = int.Parse(reader[0].ToString());
                    suplier.SupplierName = reader[1].ToString();
                    suplier.Address      = reader[2].ToString();
                    suplier.Phone        = reader[3].ToString();
                    suplier.MobilePhone  = reader[4].ToString();
                    suplier.Active       = bool.Parse(reader[5].ToString());
                    result.Add(suplier);
                }
            }
            return result;
        }

        public Supplier FindbyId(int id)
        {
            var suplier    = new Supplier();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_SUPPLIER_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@SupplierId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    suplier.SupplierID   = int.Parse(reader[0].ToString());
                    suplier.SupplierName = reader[1].ToString();
                    suplier.Address      = reader[2].ToString();
                    suplier.Phone        = reader[3].ToString();
                    suplier.MobilePhone  = reader[4].ToString();
                    suplier.Active       = bool.Parse(reader[5].ToString());
                }
            }
            return suplier;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_SUPPLIER_NAME_AVAILABLE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

        public bool UniqueNameAvailableExcept(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_SUPPLIER_NAME_AVAILABLE2") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

    }
}
