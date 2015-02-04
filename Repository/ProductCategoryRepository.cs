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
    public class ProductCategoryRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PRODUCT_CATEGORY");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                var reader     = DBClass.ExecuteReader(cmd);
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
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PRODUCT_CATEGORY");
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
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_PRODUCT_CATEGORY");
                        cmd.Parameters.AddWithValue("@CategoryId", id);
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
            var result     = new List<ProductCategory>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PRODUCT_CATEGORY");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var category          = new ProductCategory();
                    category.CategoryID   = int.Parse(reader[0].ToString());
                    category.CategoryName = reader[1].ToString();
                    category.Active       = bool.Parse(reader[2].ToString());
                    result.Add(category);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var category = new ProductCategory();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PRODUCT_CATEGORY_BY_ID");
                cmd.Parameters.AddWithValue("@CategoryId", id);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    category.CategoryID   = int.Parse(reader[0].ToString());
                    category.CategoryName = reader[1].ToString();
                    category.Active       = bool.Parse(reader[2].ToString());
                }
            }
            return category as T;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_PRODUCT_CATEGORY_NAME_AVAILABLE");
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
                var cmd = DBClass.GetStoredProcedureCommand("APP_PRODUCT_CATEGORY_NAME_AVAILABLE2");
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
