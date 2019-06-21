using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PortfolioBuffett.DataAccess;

namespace GBM.Portfolio.API.Controllers
{
    [Route("api/contract/")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private IConfiguration Config;

        public PositionController(IConfiguration configuration)
        {
            Config = configuration;
        }

        [HttpGet("{contractId}/position")]
        public ActionResult<Document> Get(string contractId) {
            Asset asset = new Asset(false, Config["AWS:AccessKeyId"], Config["AWS:SecretAccessKey"]);
            var document = asset.Get(contractId);
            return document;
        }
    }
}
