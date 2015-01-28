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
    public class CostumerRepository<T> : RepositoryBase, ICostumerRepository, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, int createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("dbo.APP_SAVE_NEW_CUSTOMER");
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                var reader = DBClass.ExecuteReader(cmd);
                while(reader.Read())
                {
                    objID = int.Parse(reader[0].ToString());
                }
            }
            return objID;
        }

        public int UpdateRow(T param, int updatedBy)
        {
            using (DBClass = new MSSQLDatabase())
            {
                using (DbTransaction txn = DBClass.BeginTransaction())
                {
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("dbo.APP_UPDATE_OUTLET_CUSTOMER");
                    RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Update);
                    cmd.Parameters.AddWithValue("@LastUpdatedBy", updatedBy);
                    DBClass.ExecuteNonQuery(cmd, txn);
                    txn.Commit();
                }
            }
            /* bypass compiler error need to be updated soon */
            return 0;
        }

        public int DeleteRow(int id, int updatedBy)
        {
            int result = 0;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = (SqlTransaction)DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("dbo.APP_DELETE_COSTUMER");
                        cmd.Parameters.AddWithValue("Id", id);
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
            var result     = new List<Costumer>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_OUTLET_COSTUMER");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var costumer         = new Costumer(int.Parse(reader[9].ToString()));
                    costumer.CustId      = int.Parse(reader[0].ToString());
                    costumer.FirstName   = reader[1].ToString();
                    costumer.LastName    = reader[2].ToString();
                    costumer.Address     = reader[3].ToString();
                    costumer.Phone       = reader[4].ToString();
                    costumer.MobilePhone = reader[5].ToString();
                    costumer.Email       = reader[6].ToString();
                    costumer.StatusId    = int.Parse(reader[7].ToString());
                    costumer.Active      = reader[8].ToString() == "True";
                    result.Add(costumer);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            Costumer costumer = null;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_OUTLET_COSTUMER");
                cmd.Parameters.AddWithValue("@CustId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    costumer             = new Costumer(int.Parse(reader[9].ToString()));
                    costumer.CustId      = int.Parse(reader[0].ToString());
                    costumer.FirstName   = reader[1].ToString();
                    costumer.LastName    = reader[2].ToString();
                    costumer.Address     = reader[3].ToString();
                    costumer.Phone       = reader[4].ToString();
                    costumer.MobilePhone = reader[5].ToString();
                    costumer.Email       = reader[6].ToString();
                    costumer.StatusId    = int.Parse(reader[7].ToString());
                    costumer.Active      = reader[8].ToString() == "True";
                }
            }
            return costumer as T;
        }

        public bool CostumerNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd    = DBClass.GetStoredProcedureCommand("APP_IS_COSTUMER_NAME_AVAILABLE");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

        public bool CostumerNameAvailableExcept(List<Dictionary<string, object>> keyValueParam)
        {
            var result = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_IS_COSTUMER_NAME_AVAILABLE_EXCEPT");
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
