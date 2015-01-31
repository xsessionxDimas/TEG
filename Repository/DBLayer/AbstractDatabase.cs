using System;
using System.Data;
using System.Data.Common;

namespace Repository.DBClass
{
    public abstract class AbstractDatabase : IDisposable
    {
        #region : Connection :

        /// <summary>Gets the Connection object associated with the current instance.</summary>
        public DbConnection Connection
        {
            get
            {
                if (internal_currentConnection == null)
                {
                    internal_currentConnection = providerFactory.CreateConnection();
                    internal_currentConnection.ConnectionString = GetConnectionString();
                }
                return internal_currentConnection;
            }
        }

        private DbProviderFactory providerFactory;
        private DbConnection internal_currentConnection;

        /// <summary>When overridden in derived classes returns the connection string for the database.</summary>
        /// <returns>The connection string for the database.</returns>
        protected abstract string GetConnectionString();

        #endregion

        #region : Commands :

        /// <summary>Gets a DbCommand object with the specified <see cref="DbCommand.CommandText"/>.</summary>
        /// <param name="sqlString">The SQL string.</param>
        /// <returns>A DbCommand object with the specified <see cref="DbCommand.CommandText"/>.</returns>
        public DbCommand GetSqlStringCommand(string sqlString)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            DbCommand cmd   = Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlString;
            return cmd;
        }

        /// <summary>Gets a DbCommand object with the specified <see cref="DbCommand.CommandText"/>.</summary>
        /// <param name="sqlStringFormat">The SQL string format.</param>
        /// <param name="args">The format arguments.</param>
        /// <returns>A DbCommand object with the specified <see cref="DbCommand.CommandText"/>.</returns>
        public DbCommand GetSqlStringCommand(string sqlStringFormat, params object[] args)
        {
            return GetSqlStringCommand(string.Format(sqlStringFormat, args));
        }

        /// <summary>Gets a DbCommand object for the specified Stored Procedure.</summary>
        /// <param name="storedProcName">The name of the stored procedure.</param>
        /// <returns>A DbCommand object for the specified Stored Procedure.</returns>
        public DbCommand GetStoredProcedureCommand(string storedProcName)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            DbCommand cmd   = Connection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = storedProcName;
            return cmd;
        }

        #region : Parameters :

        /// <summary>Adds an input parameter to the given <see cref="DbCommand"/>.</summary>
        /// <param name="cmd">The command object the parameter should be added to.</param>
        /// <param name="paramName">The identifier of the parameter.</param>
        /// <param name="paramType">The type of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The <see cref="DbParameter"/> that was created.</returns>
        public DbParameter AddInParameter(DbCommand cmd, string paramName, DbType paramType, object value)
        {
            return AddParameter(cmd, paramName, paramType, ParameterDirection.Input, value);
        }

        /// <summary>Adds an input parameter to the given <see cref="DbCommand"/>.</summary>
        /// <param name="cmd">The command object the parameter should be added to.</param>
        /// <param name="paramName">The identifier of the parameter.</param>
        /// <param name="paramType">The type of the parameter.</param>
        /// <param name="size">The maximum size in bytes, of the data table column to be affected.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The <see cref="DbParameter"/> that was created.</returns>
        public DbParameter AddInParameter(DbCommand cmd, string paramName, DbType paramType, int size, object value)
        {
            DbParameter param = AddInParameter(cmd, paramName, paramType, value);
            param.Size = size;
            cmd.Parameters.Add(param);
            return param;
        }

        /// <summary>Adds the out parameter to the given <see cref="DbCommand"/></summary>
        /// <param name="cmd">The command object the parameter should be added to.</param>
        /// <param name="paramName">The identifier of the parameter.</param>
        /// <param name="paramType">The type of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The <see cref="DbParameter"/> that was created.</returns>
        public DbParameter AddOutParameter(DbCommand cmd, string paramName, DbType paramType, object value)
        {
            return AddParameter(cmd, paramName, paramType, ParameterDirection.Output, value);
        }

        /// <summary>Adds a parameter to the given <see cref="DbCommand"/>.</summary>
        /// <param name="cmd">The command object the parameter should be added to.</param>
        /// <param name="paramName">The identifier of the parameter.</param>
        /// <param name="paramType">The type of the parameter.</param>
        /// <param name="direction"><see cref="ParameterDirection"/> of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The <see cref="DbParameter"/> that was created.</returns>
        public DbParameter AddParameter(DbCommand cmd, string paramName,
                                            DbType paramType,
                                            ParameterDirection direction,
                                            object value)
        {
            DbParameter param   = cmd.CreateParameter();
            param.DbType        = paramType;
            param.ParameterName = paramName;
            param.Value         = value;
            param.Direction     = direction;
            cmd.Parameters.Add(param);
            return param;
        }

        /// <summary>Adds a parameter to the given <see cref="DbCommand"/>.</summary>
        /// <param name="cmd">The command object the parameter should be added to.</param>
        /// <param name="paramName">The identifier of the parameter.</param>
        /// <param name="paramType">The type of the parameter.</param>
        /// <param name="direction"><see cref="ParameterDirection"/> of the parameter.</param>
        /// <param name="size">The maximum size in bytes, of the data table column to be affected.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns>The <see cref="DbParameter"/> that was created.</returns>
        public DbParameter AddParameter(DbCommand cmd, string paramName,
                                            DbType paramType,
                                            ParameterDirection direction,
                                            int size,
                                            object value)
        {
            DbParameter param = AddParameter(cmd, paramName, paramType, direction, value);
            param.Size = size;
            return param;
        }

        public void AddSimpleParameter(DbCommand cmd, String paramName, Object paramValue)
        {
            DbParameter param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;

            cmd.Parameters.Add(param);
        }

        #endregion

        #region : Executes :

        /// <summary>Executes the specified command against the current connection.</summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <returns>Result returned by the database engine.</returns>
        public int ExecuteNonQuery(DbCommand cmd)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            return cmd.ExecuteNonQuery();
        }

        /// <summary>Executes the specified command against the current connection.</summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <param name="txn">The database transaction inside which the command should be executed.</param>
        /// <returns>Result returned by the database engine.</returns>
        public int ExecuteNonQuery(DbCommand cmd, DbTransaction txn)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            cmd.Transaction = txn;
            return cmd.ExecuteNonQuery();
        }

        /// <summary>Executes the specified command against the current connection.</summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <returns>Result returned by the database engine.</returns>
        public DbDataReader ExecuteReader(DbCommand cmd)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            return cmd.ExecuteReader();
        }

        /// <summary>Executes the specified command against the current connection.</summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <param name="behavior">One of the <see cref="System.Data.CommandBehavior"/> values.</param>
        /// <returns>Result returned by the database engine.</returns>
        public DbDataReader ExecuteReader(DbCommand cmd, CommandBehavior behavior)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            return cmd.ExecuteReader(behavior);
        }

        /// <summary>Executes the specified command against the current connection.</summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <returns>Result returned by the database engine.</returns>
        public T ExecuteScalar<T>(DbCommand cmd)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            object retVal = cmd.ExecuteScalar();
            if (null == retVal || DBNull.Value == retVal)
                return default(T);

            return (T)retVal;
        }

        /// <summary>Executes the specified command against the current connection.</summary>
        /// <param name="cmd">The command to be executed.</param>
        /// <returns>Result returned by the database engine.</returns>
        public DataSet ExecuteDataSet(DbCommand cmd)
        {
            DbDataAdapter adapter = providerFactory.CreateDataAdapter();
            adapter.SelectCommand = cmd;

            var retVal = new DataSet();
            adapter.Fill(retVal);
            return retVal;
        }

        #endregion

        #endregion

        /// <summary>Begins a transaction.</summary>
        /// <returns>Created transaction.</returns>
        public DbTransaction BeginTransaction()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            return Connection.BeginTransaction();
        }

        #region : Consturction / Destruction :

        /// <summary>Initialize AbstractDatabase with ProviderFactory of choice.</summary>
        protected AbstractDatabase(DbProviderFactory providerFactory)
        {
            this.providerFactory = providerFactory;
        }

        /// <summary>Disposes the resources associated with the current database connection.</summary>
        ~AbstractDatabase()
        {
            Dispose();
        }

        #region IDisposable Members

        /// <summary>Disposes the resources associated with the current database connection.</summary>
        public void Dispose()
        {
            if (null != internal_currentConnection)
            {
                internal_currentConnection.Dispose();
                internal_currentConnection = null;
            }
        }

        #endregion

        #endregion
    }
}
