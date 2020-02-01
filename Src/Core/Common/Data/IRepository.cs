﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Containers;

namespace Common.Data
{
    public interface IRepository : ITransient, IDisposable
    {
        Task SaveChanges();

        #region Get

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        Task<T> Get<T>(string id) where T : DbEntity;

        Task<IEnumerable<T>> Get<T>(IEnumerable<string> ids) where T : DbEntity;

        /// <summary>
        /// Get document by id and include related document
        /// </summary>
        /// <typeparam name="T">Type of document</typeparam>
        /// <typeparam name="TIncType">Type of included document</typeparam>
        /// <param name="id">Id of document</param>
        /// <param name="field">Field with have related document id</param>
        /// <param name="targetField">Target field for set related document</param>
        /// <returns>T</returns>
        Task<T> Get<T, TIncType>(string id, Expression<Func<T, string>> field,
            Expression<Func<T, TIncType>> targetField) where T : DbEntity;

        /// <summary>
        /// Get document by id and include related documents
        /// </summary>
        /// <typeparam name="T">Type of document</typeparam>
        /// <typeparam name="TIncType">Type of list of included document</typeparam>
        /// <param name="id">Id of document</param>
        /// <param name="field">Field with have list of related document id</param>
        /// <param name="targetField">Target field for set related document</param>
        /// <returns>T</returns>
        Task<T> Get<T, TIncType>(string id, Expression<Func<T, IEnumerable<string>>> field,
            Expression<Func<T, IEnumerable<TIncType>>> targetField) where T : DbEntity;
        
        /// <summary>
        /// Get document by id and include related documents
        /// </summary>
        /// <typeparam name="T">Type of document</typeparam>
        /// <typeparam name="TIncType1">Type of included document 1</typeparam>
        /// <typeparam name="TIncType2">Type of included document 2</typeparam>
        /// <param name="id">Id of document</param>
        /// <param name="field1">Field 1 with have related document id</param>
        /// <param name="targetField1">Target field 1 for set related document</param>
        /// <param name="field2">Field 2 with have related document id</param>
        /// <param name="targetField2">Target field 2 for set related document</param>
        /// <returns>T</returns>
        Task<T> Get<T, TIncType1, TIncType2>(string id, Expression<Func<T, string>> field1,
            Expression<Func<T, TIncType1>> targetField1, Expression<Func<T, string>> field2,
            Expression<Func<T, TIncType2>> targetField2) where T : DbEntity;

        #endregion


        #region Insert

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        Task<T> Add<T>(T entity) where T : DbEntity;

        /// <summary>
        /// Bulk add document
        /// <b>No need save change</b>
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entities">Items</param>
        /// <returns>Task</returns>        
        Task Add<T>(IEnumerable<T> entities) where T : DbEntity;

        #endregion

        #region Update

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        Task<T> Update<T>(T entity) where T : DbEntity;

        /// <summary>
        /// Bulk update document
        /// <b>No need save change</b>
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="entities">Items</param>
        /// <returns>Task</returns>        
        Task Update<T>(IEnumerable<T> entities) where T : DbEntity;

        Task<bool> Update<T,TField>(T entity, Expression<Func<T, TField>> field, TField value) where T : DbEntity;

        Task<bool> Update<T,TField>(Expression<Func<T, bool>> filter,
           Expression<Func<T, TField>> field, TField value) where T : DbEntity;


        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity from the repository by its ObjectId.
        /// </summary>
        /// <param name="id">The ObjectId of the entity.</param>
        void Delete<T>(string id) where T : DbEntity;

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete<T>(T entity) where T : DbEntity;

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="filter">The expression.</param>
        Task Delete<T>(Expression<Func<T, bool>> filter) where T : DbEntity;

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        Task DeleteAll<T>() where T : DbEntity;

        #endregion

        #region Find

        Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter) where T : DbEntity;

        Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter,
           Expression<Func<T, object>> order,
           int pageIndex, int size) where T : DbEntity;

        Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter, int pageIndex, int size) where T : DbEntity;

        Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter,
           Expression<Func<T, object>> order,
           int pageIndex, int size, bool isDescending) where T : DbEntity;

        #endregion

        #region Util

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        Task<long> Count<T>() where T : DbEntity;

        Task<long> Count<T>(Expression<Func<T, bool>> filter) where T : DbEntity;

        Task<bool> Any<T>() where T : DbEntity;

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="filter">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        Task<bool> Any<T>(Expression<Func<T, bool>> filter) where T : DbEntity;

        #endregion

        #region Max/Min

        Task<T> Max<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>> field) where T : DbEntity;

        Task<T> Max<T>(Expression<Func<T, object>> field) where T : DbEntity;

        Task<T> Min<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>> field) where T : DbEntity;

        Task<T> Min<T>(Expression<Func<T, object>> field) where T : DbEntity;

        #endregion

    }
}