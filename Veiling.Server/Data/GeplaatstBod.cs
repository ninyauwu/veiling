using System.ComponentModel.DataAnnotations;

public class GeplaatstBod {
    public int? HoeveelheidContainers { get; set; }

    //[Required]
    //public required string GebruikerId { get; set; }

    [Required]
    public int KavelId { get; set; }
}
