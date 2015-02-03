using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using CustomTypes.Objects;
using Repository.DBClass;
using Repository.abstraction;

namespace Repository
{
    public class UserRepository<T> : RepositoryBase, IRepository<T>
    {
        public int SaveRow(T param, string createdBy)
        {
            throw new NotImplementedException();
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
            var result = new List<UserProfile>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("aspnet_Membership_GetAllUsers") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@ApplicationName", "/");
                DBClass.AddSimpleParameter(cmd, "@PageIndex", 0);
                DBClass.AddSimpleParameter(cmd, "@PageSize", 25);
                DbDataReader reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    UserProfile profile = UserProfile.GetUserProfile(reader[0].ToString());
                    profile.UserName    = reader[0].ToString();
                    profile.IsLocked    = bool.Parse(reader[10].ToString());
                    result.Add(profile);
                }
            }
            return result as IEnumerable<T>;
        }

        public string LockUser(string userName)
        {
            string result;
            try
            {
                using (DBClass = new MSSQLDatabase())
                {
                    using (var txn = DBClass.BeginTransaction())
                    {
                        var cmd = DBClass.GetStoredProcedureCommand("LockUser") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@Username", userName);
                        DBClass.ExecuteNonQuery(cmd, txn);
                        txn.Commit();
                    }
                }
                result = "User telah di non-aktifkan!";
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return result;

        }

        public T FindbyId(int id)
        {
            throw new NotImplementedException();
        }
    }
}