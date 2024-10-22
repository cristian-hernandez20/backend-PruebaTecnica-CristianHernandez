namespace Services {
    public interface IUserServices {
        Task<User> CreateUser(SaveUserDto userCreate);
        Task<User> GetUser(string Name);
        Task<List<User>> GetUsers();
    }

    public class UserServices(DataContext context, IMapper mapper) : IUserServices {
        private readonly DataContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<List<User>> GetUsers() {
            try {

                List<User> response;
                var dbUsers = await _context.User.ToListAsync();
                response = dbUsers;
                return response;
            }
            catch (Exception) { throw; }
        }
        public async Task<User> GetUser(string Name) {
            ArgumentNullException.ThrowIfNull(Name);
            try {
                User userDb = await _context.User
                    .FirstOrDefaultAsync(u => u.Name.ToLower().Equals(Name.ToLower()));
                return userDb;
            }
            catch (Exception) { throw; }
        }
        public async Task<User> CreateUser(SaveUserDto userCreate) {

            ArgumentNullException.ThrowIfNull(userCreate);

            try {
                var existingUser = await _context.User
                    .FirstOrDefaultAsync(u => u.Name.ToLower().Equals(userCreate.Name.ToLower()));

                User userMapper = null;

                if (existingUser != null) {
                    existingUser.Credit = userCreate.Credit;
                    _context.Update(existingUser);
                }
                else {
                    userMapper = _mapper.Map<User>(userCreate);
                    userMapper.Name = CapitalizeName(userCreate.Name);
                    _context.Add(userMapper);
                }
                await _context.SaveChangesAsync();

                return existingUser ?? userMapper;
            }
            catch (Exception) { throw; }
        }
        private static string CapitalizeName(string name) {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(name.ToLower());
        }
    }
}
