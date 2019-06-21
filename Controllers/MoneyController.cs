using System;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using PortfolioBuffett.DataAccess;
using PortfolioBuffett.Model;

namespace GBM.Portfolio.API.Controllers
{
    [Route("api/money/freeze")]
    [ApiController]
    public class MoneyController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Document> Freeze([FromBody] Order order) {
            Asset asset = new Asset(false);
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