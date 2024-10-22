namespace ruleta.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController(IUserServices UserServices) : ControllerBase {
        private readonly IUserServices _userServices = UserServices;

        [HttpGet("get-users")]
        public async Task<ActionResult<ServiceResponse<List<User>>>> GetUsers() {
            ServiceResponse<List<User>> response = new();
            var users = await _userServices.GetUsers();

            if (users.Count == 0) {
                response.Success = false;
                response.Message = "No existen usuarios en el sistema.";
            }

            return Ok(new ServiceResponse<List<User>> {
                Success = true,
                Data = users
            });
        }

        [HttpGet("get-user")]
        public async Task<ActionResult<ServiceResponse<User>>> GetUser([FromQuery] string Name) {
            ServiceResponse<User> response = new();
            var user = await _userServices.GetUser(Name);

            if (user == null) {
                response.Success = false;
                response.Message = $"No existen usuario con el nombre {Name}";
            }

            return Ok(new ServiceResponse<User> {
                Success = true,
                Data = user
            });
        }

        [HttpPost("create-user")]
        public async Task<ActionResult<ServiceResponse<User>>> CreateUser([FromBody] SaveUserDto userCreate) {

            var response = await _userServices.CreateUser(userCreate);

            if (response == null) {
                return StatusCode(500, new ServiceResponse<User> {
                    Success = false,
                    Message = "Ocurrió un error al procesar la solicitud."
                });
            }

            return Ok(new ServiceResponse<User> {
                Success = true,
                Data = response
            });
        }
    }
}

