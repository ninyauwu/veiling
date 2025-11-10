using Veiling.Server.Models;

public class KavelLeverancier {
    public Kavel Kavel { get; private set; }
    public Leverancier Leverancier { get; private set; }

    public KavelLeverancier(Kavel kavel, Leverancier leverancier) {
        Kavel = kavel;
        Leverancier = leverancier;
    }
}
