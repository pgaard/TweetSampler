using Microsoft.AspNetCore.Mvc;
using TweetSampler.Services;
using TweetSampler.Services.Models;

namespace TweetSampler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetStatisticsController : ControllerBase
    {
        private readonly ITwitterVolumeAPIService reader;   

        public TweetStatisticsController(ITwitterVolumeAPIService reader) 
        {
            this.reader = reader;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TweetStatistics))]
        public IActionResult Get(int? hashTagNumber)
        {
            var statistics = reader.GetCurrentStatistics(hashTagNumber ?? 10);
            return Ok(statistics);
        }
    }
}
