using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using CustomTypes.Objects;
using Repository.DBClass;
using Repository.Enums;
using Repository.abstraction;
using Repository.tools;

namespace Repository
{
    public class BankRepository : RepositoryBase, IUniqueValidation, IRepository<Bank>
    {
        public int SaveRow(Bank param, string createdBy)
        {
            int objId      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_BANK") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    objId = int.Parse(reader[0].ToString());
                }
            }
            return objId;
        }

        public int UpdateRow(Bank param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_BANK") as SqlCommand;
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
                    using (DbTransaction txn = DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_BANK") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@BankId", id);
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

        public IEnumerable<Bank> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Bank>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_GET_ALL_BANK") as SqlCommand;
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
            return result;
        }

        public Bank FindbyId(int id)
        {
            var bank       = new Bank();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_GET_BANK_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@BankId", id);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    bank.BankID   = int.Parse(reader[0].ToString());
                    bank.BankName = reader[1].ToString();
                }
            }
            return bank;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_BANK_NAME_AVAILABLE") as SqlCommand;
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
                var cmd    = DBClass.GetStoredProcedureCommand("APP_BANK_NAME_AVAILABLE2") as SqlCommand;
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
