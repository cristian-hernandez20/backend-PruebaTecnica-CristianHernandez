namespace ruleta.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ResultController(IResultServices ResultServices) : ControllerBase {
        private readonly IResultServices _resultServices = ResultServices;

        [HttpGet("ramdom-result")]
        public async Task<ActionResult<ServiceResponse<ResultRandomDtos>>> GenerateRandomResult() {

            var results = await _resultServices.GenerateRandomResult();
            return Ok(new ServiceResponse<ResultRandomDtos> {
                Success = true,
                Data = results
            });
        }
        [HttpPost("validate-result")]
        public ActionResult<ServiceResponse<ResultDtos>> ProcessResults([FromQuery] ResultRandomDtos randomResult, [FromBody] UserBetDto userBet) {

            var results = _resultServices.ProcessResults(userBet, randomResult);
            return Ok(new ServiceResponse<ResultDtos> {
                Success = true,
                Data = results,
                Message = $"El monto apostado fue {results.BetAmount}, el monto ganado fue {results.Reward}"
            });
        }

    }
}

