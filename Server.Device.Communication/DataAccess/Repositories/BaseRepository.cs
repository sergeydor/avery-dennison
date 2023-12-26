using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using Common.Domain;

namespace Server.Device.Communication.DataAccess.Repositories
{
	public class BaseRepository<TEntity> where TEntity : DBEntityBase
	{
        public virtual int BatchSize { get; set; } = 3000;

        protected IMongoDatabase _database;

        protected virtual IMongoCollection<TEntity> Collection
        {
            get
            {
                return _database.GetCollection<TEntity>(this.CollectionName);
            }
        }

		protected BaseRepository(IMongoDatabase database)
		{
            _database = database;
            //this.Collection = database.GetCollection<TEntity>(this.CollectionName);
        }

		public virtual Task<TEntity> GetOneAsync(ObjectId id)
		{
			return this.FindOneAsync(a => a.Id == id);
		}

		public virtual Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return this.Collection.Find(predicate).Limit(1).FirstOrDefaultAsync();
		}

		public virtual async Task<ICollection<TEntity>> GetAllAsync()
		{
			return await this.Collection.Find(a => true).ToListAsync().ConfigureAwait(false);
		}

		public virtual Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return this.Collection.Find(predicate).CountAsync();
		}

		public virtual async Task<ICollection<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return await this.Collection.Find(predicate).ToListAsync().ConfigureAwait(false);
		}

        public virtual List<TEntity> Find(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> sort = null, bool desc=true, int? skip=null, int? count=null)
        {
            var query = this.Collection.Find(predicate);
            if(sort != null)
            {
                if (desc)
                    query = query.SortByDescending(sort);
                else
                    query = query.SortBy(sort);
            }
            if(skip.HasValue)
            {
                query = query.Skip(skip);
            }
            if(count.HasValue)
            {
                query = query.Limit(count);
            }
            return query.ToList();
        }

        public virtual async Task<ICollection<TNewProjection>> FindAsync<TNewProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TNewProjection>> projection)
		{
			return await this.Collection.Find(predicate).Project(projection).ToListAsync().ConfigureAwait(false);
		}

		public virtual IFindFluent<TEntity, TEntity> FindQueryAsync(Expression<Func<TEntity, bool>> predicate)
		{
			return this.Collection.Find(predicate );
		}

		public virtual async Task<TEntity> CreateAsync(TEntity entity)
		{
            System.Diagnostics.Debug.Assert(entity.Id != default(ObjectId));
			await this.Collection.InsertOneAsync(entity).ConfigureAwait(false);
			return entity;
		}

        public virtual TEntity Create(TEntity entity)
        {
            System.Diagnostics.Debug.Assert(entity.Id != default(ObjectId));
            this.Collection.InsertOne(entity);
            return entity;
        }

        private List<List<TEntity>> SplitToBatches(IEnumerable<TEntity> entities)
        {
            List<List<TEntity>> result = new List<List<TEntity>>();
            List<TEntity> current = new List<TEntity>();

            foreach (var e in entities)
            {
                current.Add(e);

                if(current.Count == BatchSize)
                {
                    result.Add(current);
                    current = new List<TEntity>();
                }
            }
            if (current.Count() > 0)
            {
                result.Add(current);
            }

            return result;
        }

		public async Task CreateBatchAsync(IEnumerable<TEntity> entities)
		{
            List<List<TEntity>> batches = SplitToBatches(entities);

            foreach (List<TEntity> batch in batches)
            {
                await this.Collection.InsertManyAsync(batch, new InsertManyOptions { IsOrdered = true });
            }
        }

		public virtual async Task<bool> UpdateAsync(TEntity entity)
		{
			var result = await this.Collection.ReplaceOneAsync(a => a.Id == entity.Id, entity).ConfigureAwait(false);
			return result.MatchedCount == 1;
		}

        public virtual bool Update(TEntity entity)
        {
            var result = this.Collection.ReplaceOne(a => a.Id == entity.Id, entity);
            return result.MatchedCount == 1;
        }

        public virtual async Task<bool> DeleteAsync(TEntity entity)
		{
			var result = await this.Collection.DeleteOneAsync(a => a.Id == entity.Id).ConfigureAwait(false);
			return result.DeletedCount == 1;
		}

		public virtual async Task<bool> DeleteByIdAsync(ObjectId entityId)
		{
			var result = await this.Collection.DeleteOneAsync(a => a.Id == entityId).ConfigureAwait(false);
			return result.DeletedCount == 1;
		}

		protected virtual string CollectionName
		{
			get { return typeof(TEntity).Name; }
		}
	}
}