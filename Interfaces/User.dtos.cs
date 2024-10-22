namespace Interfaces {
    public class SaveUserDto {

        [Required(ErrorMessage = "El nombre es obligatorio."),
            MaxLength(30, ErrorMessage = "El nombre del usuario no puede tener más de 30 caracteres.")]
        public string Name { get; set; }

        [Column(TypeName = "decimal(16, 2)"),]
        public decimal Credit { get; set; }
    }

    public class UserBetDto : ResultRandomDtos {
        public decimal BetAmount { get; set; }
        public bool? IsEven { get; set; }
    }
}
