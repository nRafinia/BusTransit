using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Data.Dapper
{
    internal class BaseRepository<T> : IDisposable
        where T : BaseEntity
    {
        //private BaseContext _context;
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction = null;
        private bool _autoTransaction;

        public int Timeout { get; set; } = 45;

        #region Constructor

        protected BaseRepository(IDbConnection connection, bool autoTransaction = false)
        {
            //_context = context;
            _connection = connection;
            _connection.Open();
            _autoTransaction = autoTransaction;
            if (_autoTransaction)
                OpenTransaction();
        }

        public void Dispose()
        {
            try
            {
                if (_autoTransaction)
                    CommitTransaction();
                else
                    RollbackTransaction();
            }
            catch
            {
                RollbackTransaction();
            }

            try
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();
            }
            catch
            {
                //
            }

            _connection?.Dispose();
        }

        #endregion

        #region Transaction

        public void OpenTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction?.Commit();
                _transaction = null;
            }

            _autoTransaction = false;
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction?.Rollback();
                _transaction = null;
            }

            _autoTransaction = false;
        }

        #endregion

        #region Select

        public T Get(object id)
        {
            return _connection.Get<T>(id, _transaction, Timeout);
        }

        public async Task<T> GetAsync(object id, CancellationToken cancellationToken = default)
        {
            return await _connection.GetAsync<T>(id, _transaction, Timeout, cancellationToken);
        }

        public IEnumerable<T> GetList(object whereConditions)
        {
            return _connection.GetList<T>(whereConditions, _transaction, Timeout);
        }

        public async Task<IEnumerable<T>> GetListAsync(object whereConditions,
            CancellationToken cancellationToken = default)
        {
            return await _connection.GetListAsync<T>(whereConditions, _transaction, Timeout, cancellationToken);
        }

        public IEnumerable<T> GetList(string conditions, object parameters = null)
        {
            return _connection.GetList<T>(conditions, parameters, _transaction, Timeout);
        }

        public async Task<IEnumerable<T>> GetListAsync(string conditions, object parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _connection.GetListAsync<T>(conditions, parameters, _transaction, Timeout, cancellationToken);
        }

        public IEnumerable<T> GetList()
        {
            return _connection.GetList<T>();
        }

        public async Task<IEnumerable<T>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return await _connection.GetListAsync<T>(cancellationToken);
        }

        public IEnumerable<T> GetListPaged(int pageNumber, int rowsPerPage, string conditions, string orderBy,
            object parameters = null)
        {
            return _connection.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderBy, parameters, _transaction,
                Timeout);
        }

        public async Task<IEnumerable<T>> GetListPagedAsync(int pageNumber, int rowsPerPage, string conditions,
            string orderBy, object parameters = null, CancellationToken cancellationToken = default)
        {
            return await _connection.GetListPagedAsync<T>(pageNumber, rowsPerPage, conditions, orderBy, parameters,
                _transaction, Timeout, cancellationToken);
        }

        #endregion

        #region Insert

        public int? Insert(T entityToInsert)
        {
            //entityToInsert.ProcessFields();
            return _connection.Insert(entityToInsert, _transaction, Timeout);
        }

        public async Task<int?> InsertAsync(T entityToInsert, CancellationToken cancellationToken = default)
        {
            //entityToInsert.ProcessFields();
            return await _connection.InsertAsync(entityToInsert, _transaction, Timeout, cancellationToken);
        }

        public TKey Insert<TKey>(T entityToInsert)
        {
            //entityToInsert.ProcessFields();
            return _connection.Insert<TKey, T>(entityToInsert, _transaction, Timeout);
        }

        public async Task<TKey> InsertAsync<TKey>(T entityToInsert, CancellationToken cancellationToken = default)
        {
            //entityToInsert.ProcessFields();
            return await _connection.InsertAsync<TKey, T>(entityToInsert, _transaction, Timeout, cancellationToken);
        }

        #endregion

        #region Update

        public int Update(T entityToUpdate)
        {
            //entityToUpdate.ProcessFields();
            return _connection.Update(entityToUpdate, _transaction, Timeout);
        }

        public async Task<int> UpdateAsync(T entityToUpdate, CancellationToken cancellationToken = default)
        {
            //entityToUpdate.ProcessFields();
            return await _connection.UpdateAsync(entityToUpdate, _transaction, Timeout, cancellationToken);
        }

        #endregion

        #region Delete

        public int Delete(T entityToDelete)
        {
            return _connection.Delete(entityToDelete, _transaction, Timeout);
        }

        public async Task<int> DeleteAsync(T entityToDelete, CancellationToken cancellationToken = default)
        {
            return await _connection.DeleteAsync(entityToDelete, _transaction, Timeout, cancellationToken);
        }

        public int Delete(object id)
        {
            return _connection.Delete<T>(id, _transaction, Timeout);
        }

        public async Task<int> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            return await _connection.DeleteAsync<T>(id, _transaction, Timeout, cancellationToken);
        }

        public int DeleteList(object whereConditions)
        {
            return _connection.DeleteList<T>(whereConditions, _transaction, Timeout);
        }

        public async Task<int> DeleteListAsync(object whereConditions, CancellationToken cancellationToken = default)
        {
            return await _connection.DeleteListAsync<T>(whereConditions, _transaction, Timeout, cancellationToken);
        }

        public int DeleteList(string conditions, object parameters = null)
        {
            return _connection.DeleteList<T>(conditions, parameters, _transaction, Timeout);
        }

        public async Task<int> DeleteListAsync(string conditions, object parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _connection.DeleteListAsync<T>(conditions, parameters, _transaction, Timeout,
                cancellationToken);
        }

        #endregion

        #region Count

        public int RecordCount(string conditions = "", object parameters = null)
        {
            return _connection.RecordCount<T>(conditions, parameters, _transaction, Timeout);
        }

        public async Task<int> RecordCountAsync(string conditions = "", object parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _connection.RecordCountAsync<T>(conditions, parameters, _transaction, Timeout,
                cancellationToken);
        }

        public int RecordCount(object whereConditions)
        {
            return _connection.RecordCount<T>(whereConditions);
        }

        public async Task<int> RecordCountAsync(object whereConditions,
            CancellationToken cancellationToken = default)
        {
            return await _connection.RecordCountAsync<T>(whereConditions, cancellationToken: cancellationToken);
        }

        #endregion

        #region Execute

        public void ExecuteNone(string procedureName, object parameters = null)
        {
            _connection.ExecuteNone(procedureName, parameters, _transaction, Timeout);
        }

        public async Task ExecuteNoneAsync(string procedureName, object parameters = null,
            CancellationToken cancellationToken = default)
        {
            await _connection.ExecuteNoneAsync(procedureName, parameters, _transaction, Timeout, cancellationToken);
        }

        public T Execute(string procedureName, object parameters = null)
        {
            return _connection.ExecuteSingle<T>(procedureName, parameters, _transaction, Timeout);
        }

        public int ExecuteScalar(string procedureName, object parameters = null)
        {
            return _connection.ExecuteScalar(procedureName, parameters, _transaction, Timeout);
        }

        public async Task<int> ExecuteScalarAsync(string procedureName, object parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _connection.ExecuteScalarAsync(procedureName, parameters, _transaction, Timeout,
                cancellationToken);
        }

        public async Task<T> ExecuteAsync(string procedureName, object parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _connection.ExecuteSingleAsync<T>(procedureName, parameters, _transaction, Timeout,
                cancellationToken);
        }

        public IEnumerable<T> ExecuteList(string procedureName, object parameters = null)
        {
            return _connection.ExecuteList<T>(procedureName, parameters, _transaction, Timeout);
        }

        public async Task<IEnumerable<T>> ExecuteListAsync(string procedureName, object parameters = null,
            CancellationToken cancellationToken = default)
        {
            return await _connection.ExecuteListAsync<T>(procedureName, parameters, _transaction, Timeout,
                cancellationToken);
        }

        public void ExecuteScript(string script, object parameters = null)
        {
            _connection.ExecuteScript(script, parameters, _transaction, Timeout);
        }

        public IEnumerable<T> ExecuteFunction(string selectParameter, object whereConditions)
        {
            return _connection.ExecuteFunction<T>(selectParameter, whereConditions, _transaction, Timeout);
        }

        public async Task<IEnumerable<T>> ExecuteFunctionAsync(string selectParameter, object whereConditions,
            CancellationToken cancellationToken = default)
        {
            return await _connection.ExecuteFunctionAsync<T>(selectParameter, whereConditions, _transaction, Timeout,
                cancellationToken);
        }

        #endregion
    }
}