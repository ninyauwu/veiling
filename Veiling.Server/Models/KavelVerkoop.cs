namespace Veiling.Server.Models;

public class KavelVeiling {
    public int Id;

    public DateTime Start;
    public int DurationMs;
    
    public required int KavelId { get; set; }
    public required Kavel Kavel { get; set; }
}
