using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace PortfolioBuffett.DataAccess
{
    internal class Connection
    {
        private readonly AmazonDynamoDBClient DbClient;
        private readonly string ConnectionTableName = "Connection";

        public Connection()
        {
            DbClient = new AmazonDynamoDBClient();
        }

        public void Add(string connectionId, string contractId)
        {
            Console.WriteLine(DateTime.Now);
            PutItemRequest request = new PutItemRequest
            {
                TableName = ConnectionTableName,
                Item = new Dictionary<string, AttributeValue>() {
                    {
                        "ContractId", new AttributeValue{ S = contractId }
                    },
                    {
                        "ConnectionId", new AttributeValue{ S = connectionId }
                    }
                }
            };

            var response = DbClient.PutItemAsync(request);
            response.Wait();
            Console.WriteLine(DateTime.Now);
        }

        public void Delete(string connectionId)
        {

            ScanRequest scanRequest = new ScanRequest()
            {
                TableName = "Connection",
                FilterExpression = "ConnectionId = :ConnectionId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>() {
                    {
                        ":ConnectionId", new AttributeValue(){ S = connectionId}
                    }
                }
            };

            var scanResponse = DbClient.ScanAsync(scanRequest);
            scanResponse.Wait();

            DeleteItemRequest request = new DeleteItemRequest
            {
                TableName = ConnectionTableName,
                Key = new Dictionary<string, AttributeValue>() {
                    {
                        "ContractId", new AttributeValue{ S = scanResponse.Result.Items[0]["ContractId"].S }
                    },
                    {
                        "ConnectionId", new AttributeValue{ S = connectionId }
                    }
                }

            };

            var response = DbClient.DeleteItemAsync(request);
            response.Wait();
        }

        public List<Dictionary<string, AttributeValue>> Get(string contractId) {
            var request = new QueryRequest()
            {
                TableName = ConnectionTableName,
                KeyConditionExpression = "ContractId = :ContractId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>() {
                    {
                        ":ContractId", new AttributeValue(){  S = contractId }
                    }
                }
            };

            var response = DbClient.QueryAsync(request);
            response.Wait();
            return response.Result.Items;
        }
    }
}
