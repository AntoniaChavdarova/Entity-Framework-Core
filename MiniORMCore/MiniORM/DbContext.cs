using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
	public abstract class DbContext
	{
		private readonly DatabaseConnection connection;

		private readonly Dictionary<Type, PropertyInfo> dbSetProperties;

		internal static readonly Type[] AllowedSqlTypes =
		{
			typeof(string),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(decimal),
			typeof(bool),
			typeof(DateTime)
		};

		protected DbContext(string connectionString)
		{
			this.connection = new DatabaseConnection(connectionString);

			this.dbSetProperties = this.DiscoverDbSets();

			using(new ConnectionManager(connection))
			{
				this.InitializeDbSets();
			}

			this.MapAllRelations();
		}

		public void SaveChanges()
		{
			var dbSets = this.dbSetProperties
				.Select(pi => pi.Value.GetValue(this))
				.ToArray();

			foreach (IEnumerable<object> dbSet in dbSets)
			{
				var invalidEntities = dbSet
					.Where(entity => !IsObjectValid(entity))
					.ToArray();

				if (invalidEntities.Any())
				{
					throw new InvalidOperationException(
						$"{invalidEntities.Length} Invalid Entities found in {dbSet.GetType().Name}!");

				}
			}

			using(new ConnectionManager(connection))
			{
				using(var transaction = this.connection.StartTransaction())
				{
					foreach (IEnumerable dbSet in dbSets)
					{
						var dbSetType = dbSet.GetType()
							.GetGenericArguments().First();

						var persistMethod = typeof(DbContext)
							.GetMethod("Persist", BindingFlags.Instance | BindingFlags.NonPublic)
							.MakeGenericMethod(dbSetType);

						try
						{
							persistMethod.Invoke(this, new object[] { dbSet });
						}
						catch (TargetInvocationException tie)
						{

							throw tie.InnerException;
						}
						catch (InvalidOperationException)
						{
							transaction.Rollback();
							throw;
						}
						catch (SqlException)
						{
							transaction.Rollback();
							throw;
						}
					}

					transaction.Commit();
				}
			}

		}

		private void Persist<TEntity>(DbSet<TEntity> dbSet)
			where TEntity: class , new()
		{
			var tableName = GetTableName(typeof(TEntity));

			var colums = this.connection.FetchColumnNames(tableName).ToArray();

			if (dbSet.ChangeTracker.Added.Any())
			{
				this.connection.InsertEntities(dbSet.ChangeTracker.Added, tableName, colums);
			}

			var modifiedEntities = dbSet.ChangeTracker.GetModifiedEntities(dbSet).ToArray();

			if (modifiedEntities.Any())
			{
				this.connection.UpdateEntities(modifiedEntities, tableName, colums);
			}

			if (dbSet.ChangeTracker.Removed.Any())
			{
				this.connection.DeleteEntities(dbSet.ChangeTracker.Removed, tableName, colums);
			}
		}

		private void InitializeDbSets()
		{
			foreach (var dbSet in dbSetProperties)
			{
				var dbSetType = dbSet.Key;
				var dbSetProperty = dbSet.Value;

				var populateDbSetGeneric = typeof(DbContext)
					.GetMethod("PopulateDbSet", BindingFlags.Instance | BindingFlags.NonPublic)
					.MakeGenericMethod(dbSetType);

				populateDbSetGeneric.Invoke(this, new object[] { dbSetProperty });
			}
		}

		private void PopulateDbSet<TEntity>(PropertyInfo dbSet)
			where TEntity : class , new()
		{
			var entities = LoadTableEntities<TEntity>();

			var dbSetInstance = new DbSet<TEntity>(entities);

			ReflectionHelper.ReplaceBackingField(this, dbSet.Name, dbSetInstance);
		}

		private object LoadTableEntities<TEntity>() where TEntity : class, new()
		{
			throw new NotImplementedException();
		}

		private object GetTableName(Type type)
		{
			throw new NotImplementedException();
		}

		private void MapAllRelations()
		{
			foreach (var dbSetProperty in this.dbSetProperties)
			{
				var dbSetType = dbSetProperty.Key;

				var mapRelationsGeneric = typeof(DbContext)
					.GetMethod("MapRelations", BindingFlags.Instance | BindingFlags.NonPublic)
					.MakeGenericMethod(dbSetType);

				var dbSet = dbSetProperty.Value.GetValue(this);

				mapRelationsGeneric.Invoke(this, new[] { dbSet });
			}
		}
			   
		private void MapRelations<TEntity>(DbSet<TEntity> dbSet)
			where TEntity : class , new()
		{

		}
		private void InitializeDbSets()
		{
			throw new NotImplementedException();
		}

		private Dictionary<Type, PropertyInfo> DiscoverDbSets()
		{
			throw new NotImplementedException();
		}
	}
}