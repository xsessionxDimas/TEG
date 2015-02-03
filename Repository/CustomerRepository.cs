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
    public class CustomerRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("dbo.APP_SAVE_NEW_CUSTOMER") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader = DBClass.ExecuteReader(cmd);
                while(reader.Read())
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
                    var cmd = DBClass.GetStoredProcedureCommand("dbo.APP_UPDATE_CUSTOMER") as SqlCommand;
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
                        var cmd = DBClass.GetStoredProcedureCommand("dbo.APP_DELETE_CUSTOMER") as SqlCommand;
                        DBClass.AddSimpleParameter(cmd, "@CustomerId", id);
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
            var result     = new List<Customer>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_CUSTOMER") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var customer             = new Customer();
                    customer.CustomerID      = int.Parse(reader[0].ToString());
                    customer.FirstName       = StringManipulation.UppercaseFirst(reader[1].ToString());
                    customer.LastName        = StringManipulation.UppercaseFirst(reader[2].ToString());
                    customer.Address         = reader[3].ToString();
                    customer.Phone           = reader[4].ToString();
                    customer.MobilePhone     = reader[5].ToString();
                    customer.Email           = reader[6].ToString();
                    customer.DepartementName = reader[7].ToString();
                    customer.StatusName      = reader[8].ToString();
                    customer.Active          = bool.Parse(reader[9].ToString());
                    result.Add(customer);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            Customer customer = null;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_CUSTOMER_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CustomerId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    customer                = new Customer();
                    customer.CustomerID     = int.Parse(reader[0].ToString());
                    customer.FirstName      = reader[1].ToString();
                    customer.LastName       = reader[2].ToString();
                    customer.Address        = reader[3].ToString();
                    customer.Phone          = reader[4].ToString();
                    customer.MobilePhone    = reader[5].ToString();
                    customer.Email          = reader[6].ToString();
                    customer.DepartementID  = int.Parse(reader[7].ToString());
                    customer.StatusId       = int.Parse(reader[8].ToString());
                    customer.Active         = bool.Parse(reader[9].ToString());
                    customer.CreditFeature  = bool.Parse(reader[10].ToString());
                    customer.CreditLimit    = decimal.Parse(reader[11].ToString());
                    customer.DueDate        = int.Parse(reader[12].ToString());
                }
            }
            return customer as T;
        }

        public T FindbyName(string name)
        {
            Customer customer = null;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_CUSTOMER_BY_NAME") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@Name", name);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    customer = new Customer();
                    customer.CustomerID     = int.Parse(reader[0].ToString());
                    customer.FirstName      = StringManipulation.UppercaseFirst(reader[1].ToString());
                    customer.LastName       = StringManipulation.UppercaseFirst(reader[2].ToString());
                    customer.Address        = reader[3].ToString();
                    customer.Phone          = reader[4].ToString();
                    customer.MobilePhone    = reader[5].ToString();
                    customer.Email          = reader[6].ToString();
                    customer.DepartementID  = int.Parse(reader[7].ToString());
                    customer.StatusId       = int.Parse(reader[8].ToString());
                    customer.Active         = bool.Parse(reader[9].ToString());
                    customer.CreditLimit    = decimal.Parse(reader[10].ToString());
                }
            }
            return customer as T;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_CUSTOMER_NAME_AVAILABLE") as SqlCommand;
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
                var cmd = DBClass.GetStoredProcedureCommand("APP_CUSTOMER_NAME_AVAILABLE2") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 0;
                }
            }
            return result;
        }

        public bool IsCustomerExist(string fullName)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_CUSTOMER_NAME_AVAILABLE3") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@Fullname", fullName);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = int.Parse(reader[0].ToString()) == 1;
                }
            }
            return result;
        }

        public bool IsCostumerCanCredit(int id)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_CUSTOMER_CAN_CREDIT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CustomerId", id);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = bool.Parse(reader[0].ToString());
                }
            }
            return result;
        }

        public decimal CostumerCreditLimit(int id)
        {
            decimal result = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_CUSTOMER_CREDIT_LIMIT") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@CustomerId", id);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    result = decimal.Parse(reader[0].ToString());
                }
            }
            return result;
        }

        public IEnumerable<T> DashboardCreditPassAlert(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Customer>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_CUSTOMER_LIMITCREDIT_PASS_ALERT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var customer             = new Customer();
                    customer.DepartementName = reader[0].ToString();
                    customer.FirstName       = StringManipulation.UppercaseFirst(reader[1].ToString());
                    customer.LastName        = StringManipulation.UppercaseFirst(reader[2].ToString());
                    customer.CreditLimit     = decimal.Parse(reader[3].ToString());
                    customer.CreditTotal     = decimal.Parse(reader[4].ToString());
                    result.Add(customer);
                }
            }
            return result as List<T>;
        }
    }
}
