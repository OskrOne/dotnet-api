using System;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PortfolioBuffett.DataAccess;
using PortfolioBuffett.Model;

namespace GBM.Portfolio.API.Controllers
{
    [Route("api/money/freeze")]
    [ApiController]
    public class MoneyController : ControllerBase
    {
        private IConfiguration Config;

        public MoneyController(IConfiguration configuration) {
            Config = configuration;
        }

        [HttpPost]
        public ActionResult<Document> Freeze([FromBody] Order order) {
            Asset asset = new Asset(false, Config["AWS:AccessKeyId"], Config["AWS:SecretAccessKey"]);

            if (asset.FreezeMoney(order))
            {
                return asset.Get(order.ContractId);
            }
            else {
                throw new Exception("Something happened");
            }
        }
    }
}