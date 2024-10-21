namespace ruleta.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ResultController(IResultServices ResultServices) : ControllerBase {
        private readonly IResultServices _resultServices = ResultServices;

        [HttpGet("process-result")]
        public ActionResult<ServiceResponse<ResponseResultDtos>> ProcessResults() {

            var results = _resultServices.ProcessResults();
            return Ok(new ServiceResponse<ResponseResultDtos> {
                Success = true,
                Data = results
            });
        }

    }
}

