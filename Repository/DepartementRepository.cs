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
    public class DepartementRepository<T> : RepositoryBase, IUniqueValidation, IRepository<T> where T : BaseEntityObject
    {
        public int SaveRow(T param, string createdBy)
        {
            int objID      = 0;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_SAVE_NEW_DEPARTEMENT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, param, CRUDType.Insert);
                DBClass.AddSimpleParameter(cmd, "@CreatedBy", createdBy);
                var reader      = DBClass.ExecuteReader(cmd);
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
                    var cmd = DBClass.GetStoredProcedureCommand("APP_UPDATE_DEPARTEMENT") as SqlCommand;
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
                        var cmd = DBClass.GetStoredProcedureCommand("APP_DELETE_DEPARTEMENT") as SqlCommand;
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

        public IEnumerable<T> FindAll(List<Dictionary<string, object>> keyValueParam)
        {
            var result       = new List<Departement>();
            using (DBClass   = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_DEPARTEMENT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var departement             = new Departement();
                    departement.DepartementID   = int.Parse(reader[0].ToString());
                    departement.DepartementName = reader[1].ToString();
                    departement.CompanyID       = int.Parse(reader[2].ToString());
                    departement.CompanyName     = reader[3].ToString();
                    departement.Address         = reader[4].ToString();
                    departement.Phone           = reader[5].ToString();
                    departement.Head            = reader[6].ToString();
                    departement.WarehouseID     = string.IsNullOrEmpty(reader[7].ToString()) ? (int?)null : int.Parse(reader[7].ToString());
                    departement.WarehouseName   = reader[8].ToString();
                    departement.Active          = bool.Parse(reader[9].ToString());
                    result.Add(departement);
                }
            }
            return result as List<T>;
        }

        public T FindbyId(int id)
        {
            var departement = new Departement();
            using (DBClass  = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_DEPARTEMENT_BY_ID") as SqlCommand;
                DBClass.AddSimpleParameter(cmd, "@DepartementId", id);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    departement.DepartementID   = int.Parse(reader[0].ToString());
                    departement.DepartementName = reader[1].ToString();
                    departement.CompanyID       = int.Parse(reader[2].ToString());
                    departement.CompanyName     = reader[3].ToString();
                    departement.Address         = reader[4].ToString();
                    departement.Phone           = reader[5].ToString();
                    departement.Head            = reader[6].ToString();
                    departement.SupervisorID    = string.IsNullOrEmpty(reader[7].ToString()) ? (int?)null : int.Parse(reader[7].ToString());
                    departement.SupervisorName  = reader[8].ToString();
                    departement.WarehouseID     = string.IsNullOrEmpty(reader[9].ToString()) ? (int?)null : int.Parse(reader[9].ToString());
                    departement.WarehouseName   = reader[10].ToString();
                    departement.IsSupervisor    = bool.Parse(reader[11].ToString());
                    departement.IsTreasurer     = bool.Parse(reader[12].ToString());
                    departement.IsWarehouse     = bool.Parse(reader[13].ToString());
                    departement.IsOutlet        = bool.Parse(reader[14].ToString());
                    departement.Active          = bool.Parse(reader[15].ToString());
                }
            }
            return departement as T;
        }

        public IEnumerable<T> FindDepartement(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Departement>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_DEPARTEMENT") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var departement             = new Departement();
                    departement.DepartementID   = int.Parse(reader[0].ToString());
                    departement.DepartementName = reader[1].ToString();
                    result.Add(departement);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindAllWarehouse(List<Dictionary<string, object>> keyValueParam)
        {
            var result       = new List<Departement>();
            using (DBClass   = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_WAREHOUSE") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var departement             = new Departement();
                    departement.DepartementID   = int.Parse(reader[0].ToString());
                    departement.DepartementName = reader[1].ToString();
                    result.Add(departement);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindAllOutlet(List<Dictionary<string, object>> keyValueParam)
        {
            var result       = new List<Departement>();
            using (DBClass   = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_OUTLET") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var departement             = new Departement();
                    departement.DepartementID   = int.Parse(reader[0].ToString());
                    departement.DepartementName = reader[1].ToString();
                    result.Add(departement);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindAllSupervisor()
        {
            var result     = new List<Departement>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_SUPERVISOR") as SqlCommand;
                var reader     = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var departement             = new Departement();
                    departement.DepartementID   = int.Parse(reader[0].ToString());
                    departement.DepartementName = reader[1].ToString();
                    result.Add(departement);
                }
            }
            return result as List<T>;
        }

        public IEnumerable<T> FindAllExceptSupervisor(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = new List<Departement>();
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_GET_ALL_DEPARTEMENT_EXCEPT_SUPERVISOR") as SqlCommand;
                RoutinesParameterSetter.Set(ref cmd, keyValueParam);
                var reader = DBClass.ExecuteReader(cmd);
                while (reader.Read())
                {
                    var departement             = new Departement();
                    departement.DepartementID   = int.Parse(reader[0].ToString());
                    departement.DepartementName = reader[1].ToString();
                    result.Add(departement);
                }
            }
            return result as List<T>;
        }

        public bool UniqueNameAvailable(List<Dictionary<string, object>> keyValueParam)
        {
            var result     = false;
            using (DBClass = new MSSQLDatabase())
            {
                var cmd = DBClass.GetStoredProcedureCommand("APP_DEPARTEMENT_NAME_AVAILABLE") as SqlCommand;
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
                var cmd = DBClass.GetStoredProcedureCommand("APP_DEPARTEMENT_NAME_AVAILABLE2") as SqlCommand;
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
