namespace Mappers {
    public class UserMap : Profile {
        public UserMap() {
            CreateMap<SaveUserDto, User>();
        }
    }
}
