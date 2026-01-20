using System.ComponentModel.DataAnnotations;

public class GeplaatstBod {
    public int? HoeveelheidContainers { get; set; }

    [Required]
    public int KavelId { get; set; }
}
