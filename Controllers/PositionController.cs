using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.AspNetCore.Mvc;
using PortfolioBuffett.DataAccess;

namespace GBM.Portfolio.API.Controllers
{
    [Route("api/contract/")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        [HttpGet("{contractId}/position")]
        public ActionResult<Document> Get(string contractId) {
            Asset asset = new Asset(false);
            var document = asset.Get(contractId);
            return document;
        }
    }
}
