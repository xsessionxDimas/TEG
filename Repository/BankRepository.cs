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
    public class BankRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_BANK");
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
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_BANK");
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
                    using (DbTransaction txn = DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_BANK");
                        cmd.Parameters.AddWithValue("@BankId", id);
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
            var result     = new List<Bank>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_BANK");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var bank      = new Bank();
                    bank.BankID   = int.Parse(reader[0].ToString());
                    bank.BankName = reader[1].ToString();
                    result.Add(bank);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var bank = new Bank();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_BANK_BY_ID");
                cmd.Parameters.AddWithValue("@BankId", id);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    bank.BankID   = int.Parse(reader[0].ToString());
                    bank.BankName = reader[1].ToString();
                }
            }
            return bank as T;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_BANK_NAME_AVAILABLE");
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
                var cmd   = DBClass.GetStoredProcedureCommand("APP_BANK_NAME_AVAILABLE2");
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
