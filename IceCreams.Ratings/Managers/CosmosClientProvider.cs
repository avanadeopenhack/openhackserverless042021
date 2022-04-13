using IceCreams.Ratings.Models.Dto;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace IceCreams.Ratings.Managers
{
    public class CosmosClientProvider
    {
        // The Cosmos client instance
        private CosmosClient _cosmosClient;

        // The container we will create.
        private Container _container;

        public CosmosClientProvider(string dbConnectionString, string databaseName, string containerName)
        {
            _cosmosClient = new CosmosClient(dbConnectionString, CreateCosmosClientOptions());
            _container = _cosmosClient.GetContainer(databaseName, containerName);
        }

        private CosmosClientOptions CreateCosmosClientOptions()
        {
            CosmosClientOptions cosmosClientOptions = new CosmosClientOptions();
            cosmosClientOptions.EnableContentResponseOnWrite = false;
            cosmosClientOptions.IdleTcpConnectionTimeout = TimeSpan.FromDays(1);
            return cosmosClientOptions;
        }

        public async Task<T> RetrieveEntityAsync<T>(string id, string partitionKey)
        {
            var item = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
            if (item != null)
            {
                return item.Resource;
            }
            return default;
        }

        public async Task<IQueryable<T>> QueryEntitiesAsync<T>(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _container.GetItemLinqQueryable<T>(true)
                .Where(predicate);

            return await Task.FromResult(query);
        }

        public async Task<IQueryable<T>> QueryEntitiesAsync<T>(Expression<Func<T, bool>> predicate, string partitionKey)
        {
            var requestOptions = new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) };
            IQueryable<T> query = _container.GetItemLinqQueryable<T>(true, requestOptions: requestOptions)
                .Where(predicate);

            return await Task.FromResult(query);
        }

        public async Task InsertAsync<T>(T entity, string partitionKey)
        {
            await _container.CreateItemAsync(entity, new PartitionKey(partitionKey));
        }
    }

}
