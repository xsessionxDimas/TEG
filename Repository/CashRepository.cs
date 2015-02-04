using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CustomTypes.Base;
using CustomTypes.Objects;
using Repository.DBClass;
using Repository.Enums;
using Repository.abstraction;
using Repository.tools;

namespace Repository
{
    public class CashRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_CASH");
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
            throw new NotImplementedException();
        }

        public int DeleteRow(int id, string updatedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result      = new List<Cash>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_CASH");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var cash             = new Cash();
                    cash.DepartementName = reader[0].ToString();
                    cash.StartingBalance = decimal.Parse(reader[1].ToString());
                    cash.CurrentBalance  = decimal.Parse(reader[2].ToString());
                    result.Add(cash);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var cash       = new Cash();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_DEPARTEMENT_CASH");
                cmd.Parameters.AddWithValue("@DepartementId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    cash.CashID = int.Parse(reader[0].ToString());
                }
            }
            return cash as T;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result      = false;
            using (DBClass  = new MSSQLDatabase())
            {
                var cmd     = DBClass.GetStoredProcedureCommand("APP_CASH_AVAILABLE");
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

        public void CashIncome(List<Dictionary<string, object>> keyValueParam)
        {
            throw new NotImplementedException();
        }

        public void CashExpense(List<Dictionary<string, object>> keyValueParam)
        {
            throw new NotImplementedException();
        }
    }
}
