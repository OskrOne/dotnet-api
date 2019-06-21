using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DAX;
using Amazon.DynamoDBv2.Model;
using System;
using PortfolioBuffett.Model;
using Amazon;

namespace PortfolioBuffett.DataAccess
{
    internal class Asset
    {
        private readonly AmazonDynamoDBClient DbClient;
        private readonly string AssetTableName = "Assets";

        public Asset(bool local, string awsAccessKeyId = null, string awsSecretAccessKey = null)
        {
            
            if (local) {
                AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
                clientConfig.ServiceURL = "http://localhost:8000";
                DbClient = new AmazonDynamoDBClient(clientConfig);
            }
            else {
                DbClient = new AmazonDynamoDBClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.USWest2);
            }
        }

        /// <summary>
        /// Get asset using Document API
        /// </summary>
        /// <param name="contractId">ContractId Primary Key</param>
        /// <returns></returns>
        public Document Get(string contractId)
        {
            Console.WriteLine("Aca voy");
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var table = Table.LoadTable(DbClient, AssetTableName);
            var result = table.GetItemAsync(contractId);
            result.Wait();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            if (result.IsCompletedSuccessfully) {
                return result.Result;
            }

            throw new Exception("Error reading asset table", result.Exception);
        }

        /// <summary>
        /// Get item using Amazon Dynamo Accelerator (DAX), to use this function, you have to install the lambda at the 
        /// same VPC as DAX, then you could access to the Lambda using EC2 instance
        /// </summary>
        /// <param name="contractId"></param>
        /// <returns></returns>
        public Dictionary<string, AttributeValue> GetDAX(string contractId)
        {
            DaxClientConfig config = new DaxClientConfig("assetcluster.ofc1zu.clustercfg.dax.usw2.cache.amazonaws.com", 8111);
            ClusterDaxClient client = new ClusterDaxClient(config);
            GetItemRequest request = new GetItemRequest()
            {
                TableName = AssetTableName,
                Key = new Dictionary<string, AttributeValue>() {
                    {
                        "ContractId", new AttributeValue(){ S = contractId }
                    }
                }
            };

            var result = client.GetItemAsync(request);
            result.Wait();

            if (result.IsCompletedSuccessfully)
            {
                return result.Result.Item;
            }

            throw new Exception("Error reading asset table", result.Exception);
        }

        public bool FreezeMoney(Order order)
        {
            Console.WriteLine("Aca voy");
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            TransactWriteItemsRequest request = new TransactWriteItemsRequest();
            List<TransactWriteItem> transactWriteItems = new List<TransactWriteItem>();

            transactWriteItems.Add(GetInsertOrderFlow(order));
            transactWriteItems.Add(GetUpdateContract(order));

            request.TransactItems = transactWriteItems;
    
            var response = DbClient.TransactWriteItemsAsync(request);
            response.Wait();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return response.IsCompletedSuccessfully;
        }

        private TransactWriteItem GetUpdateContract(Order order)
        {
            var updateItem = new TransactWriteItem()
            {
                Update = new Update()
                {
                    TableName = "Contract",
                    Key = new Dictionary<string, AttributeValue>() {
                        { "ContractId", new AttributeValue() { S = order.ContractId } }
                    },
                    UpdateExpression = "set BuyingPower = BuyingPower - :price",
                    ConditionExpression = "BuyingPower >= :price",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>() {
                        { ":price", new AttributeValue(){ N = (order.Price * order.Quantity).ToString() } }
                    }
                }
            };

            return updateItem;
        }

        private TransactWriteItem GetInsertOrderFlow(Order order)
        {
            var item = new Dictionary<string, AttributeValue>
            {
                { "ContractId", new AttributeValue() { S = order.ContractId } },
                { "InstrumentId", new AttributeValue() { N = order.InstrumentId.ToString() } },
                { "InstrumentName", new AttributeValue() { S = order.InstrumentName } },
                { "Quantity", new AttributeValue() { N = order.Quantity.ToString() } },
                { "Price", new AttributeValue() { N = order.Price.ToString() } },
                { "Side", new AttributeValue() { S = order.Side } },
                { "Status", new AttributeValue() { S = order.Status } }
            };

            var writeItem = new TransactWriteItem()
            {
                Put = new Put()
                {
                    TableName = "OrderFlow",
                    Item = item
                }
            };

            return writeItem;
        }
    }
}
