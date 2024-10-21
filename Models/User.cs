namespace Models {

    [Index(nameof(Name), IsUnique = true)]
    public class User {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio."),
            MaxLength(30, ErrorMessage = "El nombre del usuario no puede tener más de 30 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(16, 2)")]
        public decimal Credit { get; set; } = 0m;
    }
}