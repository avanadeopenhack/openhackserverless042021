using IceCreams.Ratings.Infra.Dto;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
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
            if(item != null)
            {
                return item.Resource;
            }
            return default;
        }

        //public async Task<IQueryable<T>> QueryEntitiesAsync<T>(Expression<Func<T, bool>> predicate, string partitionKey)
        //{
        //    var requst
        //}

        public async Task InsertAsync<T>(T entity, string partitionKey)
        {
            //await _container.CreateItemAsync(entity, new PartitionKey(partitionKey));
            try
            {
                //var item = await _container.ReadItemAsync<T>("648df6db-6786-4418-b370-01cb1c5f7625", new PartitionKey(partitionKey));
                var result = _container.CreateItemAsync(entity, new PartitionKey(partitionKey)).Result;
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Add Family items to the container
        /// </summary>
        public async Task AddItemsToContainerAsync(Rating item)
        {
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Rating> itemResponse = await this._container.ReadItemAsync<Rating>(item.Id, new PartitionKey(item.UserId));
                //Console.WriteLine("Item in database with id: {0} already exists\n", andersenFamilyResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<Rating> itemResponse = await this._container.CreateItemAsync<Rating>(item, new PartitionKey(item.UserId));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemResponse.Resource.Id, itemResponse.RequestCharge);
            }
        }
    }

}
