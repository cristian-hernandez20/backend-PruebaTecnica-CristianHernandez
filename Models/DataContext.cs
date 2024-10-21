namespace Models {
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options) {

        public DbSet<User> User { get; set; }
        protected override void OnModelCreating(ModelBuilder ModelBuilder) {
        }
    }
}