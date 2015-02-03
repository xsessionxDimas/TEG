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
    public class BankAccountRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject 
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_BANK_ACCOUNT") as SqlCommand;
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

        public int UpdateRow(T param, string updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_BANK_ACCOUNT") as SqlCommand;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_BANK_ACCOUNT") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@BankAccountId", id);
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

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<BankAccount>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_GET_ALL_BANK_ACCOUNT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var bankAccount = new BankAccount();
                    bankAccount.BankAccountID   = int.Parse(reader[0].ToString());
                    bankAccount.DepartementName = reader[1].ToString();
                    bankAccount.BankName        = reader[2].ToString();
                    bankAccount.AccountNumber   = reader[3].ToString();
                    bankAccount.AccountName     = reader[4].ToString();
                    bankAccount.StartingBalance = decimal.Parse(reader[5].ToString());
                    bankAccount.CurrentBalance  = decimal.Parse(reader[6].ToString());
                    bankAccount.Active          = bool.Parse(reader[7].ToString());
                    result.Add(bankAccount);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var bankAccount = new BankAccount();
            using (DBClass  = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("APP_GET_BANK_ACCOUNT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@BankAccountId", id);
                var reader  = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    bankAccount.BankAccountID   = int.Parse(reader[0].ToString());
                    bankAccount.DepartementID   = int.Parse(reader[1].ToString());
                    bankAccount.BankID          = int.Parse(reader[2].ToString());
                    bankAccount.AccountNumber   = reader[3].ToString();
                    bankAccount.AccountName     = reader[4].ToString();
                    bankAccount.StartingBalance = decimal.Parse(reader[5].ToString());
                    bankAccount.CurrentBalance  = decimal.Parse(reader[6].ToString());
                    bankAccount.Active          = bool.Parse(reader[7].ToString());
                }
            }
            return bankAccount as T;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("APP_BANK_ACCOUNT_AVAILABLE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader  = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

        public bool UniqueNameAvailableExcept(List<Dictionary<string, object>> keyValueParam)
        {
            throw new NotImplementedException();
        }

        public void CashBankIncome(List<Dictionary<string, object>> keyValueParam)
        {
            throw new NotImplementedException();
        }

        public void CashBankExpense(List<Dictionary<string, object>> keyValueParam)
        {
            throw new NotImplementedException();
        }

    }
}
