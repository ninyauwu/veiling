namespace Veiling.Server.Models;

public class KavelVeiling {
    public int Id { get; set; }

    public DateTime Start { get; set; }
    public int DurationMs { get; set; }
    
    public required int KavelId { get; set; }
    public required Kavel Kavel { get; set; }
}
