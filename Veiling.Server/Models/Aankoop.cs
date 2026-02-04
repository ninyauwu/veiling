using System.Text.Json.Serialization;
using Veiling.Server.Models;

public class Aankoop {
    public int Id { get; set; }
    public int Hoeveelheid { get; set; }

    public int? BodId { get; set; }

    [JsonIgnore]
    public Bod? Bod { get; set; }
}
