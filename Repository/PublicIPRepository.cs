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
    public class PublicIPRepository<T> : RepositoryBase, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_PUBLIC_IP");
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
                    SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_PUBLIC_IP");
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_IP_PUBLIC");
                        cmd.Parameters.AddWithValue("@PublicIPID", id);
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
            var result     = new List<PublicIP>();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_PUBLIC_IP");
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var publicIP        = new PublicIP();
                    publicIP.PublicIPID = int.Parse(reader[0].ToString());
                    publicIP.IPAddress  = reader[1].ToString();
                    result.Add(publicIP);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            throw new NotImplementedException();
        }

        public T FindbyId(string IpAddress)
        {
            var publicIP = new PublicIP();
            using (DBClass = new MSSQLDatabase())
            {
                SqlCommand cmd = DBClass.GetStoredProcedureCommand("APP_GET_PUBLIC_IP_BY_ID");
                cmd.Parameters.AddWithValue("@IpAddress", IpAddress);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    publicIP.PublicIPID = int.Parse(reader[0].ToString());
                    publicIP.IPAddress = reader[1].ToString();
                }
            }
            return publicIP as T;
        }        
    }
}
