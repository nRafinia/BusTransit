﻿using System;

namespace Common.Data
{
    /// <summary>
    /// Optional Table attribute.
    /// You can use the System.ComponentModel.DataAnnotations version in its place to specify the table name of a poco
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Optional Table attribute.
        /// </summary>
        /// <param name="tableName"></param>
        public TableAttribute(string tableName)
        {
            Name = tableName;
        }

        /// <summary>
        /// Name of the table
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Name of the schema
        /// </summary>
        public string Schema { get; set; }
    }
}