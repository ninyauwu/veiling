using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KavelHistoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KavelHistoryController> _logger;

        public KavelHistoryController(IConfiguration configuration, ILogger<KavelHistoryController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private string GetConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("Default");
            var server = Environment.GetEnvironmentVariable("DB_SERVER");
            var username = Environment.GetEnvironmentVariable("DB_USERNAME");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
            
            return connectionString + 
                   $"Server={server};" +
                   $"User Id={username};" +
                   $"Password={password};";
        }

        // GET: api/kavelhistory/{kavelId}
        [HttpGet("{kavelId}")]
        public async Task<ActionResult<KavelHistoryResponse>> GetKavelHistory(int kavelId)
        {
            try
            {
                using var connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                // Haal eerst kavel informatie op inclusief leverancier
                var kavel = await GetKavelInfo(connection, kavelId);
                if (kavel == null)
                {
                    return NotFound(new { message = "Kavel niet gevonden" });
                }

                // Haal historische data op voor deze leverancier
                var leverancierStats = await GetLeverancierHistory(
                    connection, 
                    kavel.LeverancierId, 
                    kavel.Naam
                );

                // Haal historische data op voor alle leveranciers
                var totaalStats = await GetTotaalHistory(connection, kavel.Naam);

                var response = new KavelHistoryResponse
                {
                    KavelId = kavelId,
                    KavelNaam = kavel.Naam,
                    LeverancierNaam = kavel.LeverancierNaam,
                    LeverancierStatistieken = leverancierStats,
                    TotaalStatistieken = totaalStats
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij ophalen kavel geschiedenis voor kavelId {KavelId}", kavelId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het ophalen van de gegevens" });
            }
        }

        private async Task<KavelInfo?> GetKavelInfo(SqlConnection connection, int kavelId)
        {
            const string query = @"
                SELECT 
                    k.Id,
                    k.Naam,
                    k.LeverancierId,
                    b.Bedrijfsnaam AS LeverancierNaam
                FROM Kavels k
                LEFT JOIN Leveranciers l ON k.LeverancierId = l.Id
                LEFT JOIN Bedrijven b ON l.BedrijfId = b.Bedrijfscode
                WHERE k.Id = @KavelId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@KavelId", kavelId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new KavelInfo
                {
                    Id = reader.GetInt32(0),
                    Naam = reader.GetString(1),
                    LeverancierId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                    LeverancierNaam = reader.IsDBNull(3) ? "Onbekend" : reader.GetString(3)
                };
            }

            return null;
        }

        private async Task<PrijsStatistieken> GetLeverancierHistory(
            SqlConnection connection, 
            int? leverancierId, 
            string kavelNaam)
        {
            if (!leverancierId.HasValue)
            {
                return new PrijsStatistieken
                {
                    GemiddeldePrijs = 0,
                    Laatste10Prijzen = new List<HistorischePrijs>()
                };
            }

            // Query geoptimaliseerd met indexen en CTE voor betere performance
            const string query = @"
                WITH RecenteBoden AS (
                    SELECT TOP 10
                        b.Koopprijs,
                        b.Datumtijd,
                        v.Naam AS VeilingNaam
                    FROM Boden b
                    INNER JOIN Kavels k ON b.KavelId = k.Id
                    INNER JOIN Veilingen v ON k.VeilingId = v.Id
                    WHERE k.LeverancierId = @LeverancierId
                        AND k.Naam = @KavelNaam
                        AND b.Koopprijs > 0
                    ORDER BY b.Datumtijd DESC
                )
                SELECT 
                    (SELECT AVG(CAST(b2.Koopprijs AS FLOAT))
                     FROM Boden b2
                     INNER JOIN Kavels k2 ON b2.KavelId = k2.Id
                     WHERE k2.LeverancierId = @LeverancierId
                        AND k2.Naam = @KavelNaam
                        AND b2.Koopprijs > 0) AS GemiddeldePrijs,
                    rb.Koopprijs,
                    rb.Datumtijd,
                    rb.VeilingNaam
                FROM RecenteBoden rb
                ORDER BY rb.Datumtijd DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LeverancierId", leverancierId.Value);
            command.Parameters.AddWithValue("@KavelNaam", kavelNaam);

            var laatste10 = new List<HistorischePrijs>();
            float? gemiddelde = null;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (!gemiddelde.HasValue && !reader.IsDBNull(0))
                {
                    gemiddelde = (float)reader.GetDouble(0);
                }

                laatste10.Add(new HistorischePrijs
                {
                    Prijs = reader.GetFloat(1),
                    Datum = reader.GetDateTime(2),
                    VeilingNaam = reader.GetString(3)
                });
            }

            return new PrijsStatistieken
            {
                GemiddeldePrijs = gemiddelde ?? 0,
                Laatste10Prijzen = laatste10
            };
        }

        private async Task<PrijsStatistieken> GetTotaalHistory(
            SqlConnection connection, 
            string kavelNaam)
        {
            // Query geoptimaliseerd met indexen en CTE
            const string query = @"
                WITH RecenteBoden AS (
                    SELECT TOP 10
                        b.Koopprijs,
                        b.Datumtijd,
                        v.Naam AS VeilingNaam,
                        bedrijf.Bedrijfsnaam AS LeverancierNaam
                    FROM Boden b
                    INNER JOIN Kavels k ON b.KavelId = k.Id
                    INNER JOIN Veilingen v ON k.VeilingId = v.Id
                    LEFT JOIN Leveranciers l ON k.LeverancierId = l.Id
                    LEFT JOIN Bedrijven bedrijf ON l.BedrijfId = bedrijf.Bedrijfscode
                    WHERE k.Naam = @KavelNaam
                        AND b.Koopprijs > 0
                    ORDER BY b.Datumtijd DESC
                )
                SELECT 
                    (SELECT AVG(CAST(b2.Koopprijs AS FLOAT))
                     FROM Boden b2
                     INNER JOIN Kavels k2 ON b2.KavelId = k2.Id
                     WHERE k2.Naam = @KavelNaam
                        AND b2.Koopprijs > 0) AS GemiddeldePrijs,
                    rb.Koopprijs,
                    rb.Datumtijd,
                    rb.VeilingNaam,
                    rb.LeverancierNaam
                FROM RecenteBoden rb
                ORDER BY rb.Datumtijd DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@KavelNaam", kavelNaam);

            var laatste10 = new List<HistorischePrijs>();
            float? gemiddelde = null;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (!gemiddelde.HasValue && !reader.IsDBNull(0))
                {
                    gemiddelde = (float)reader.GetDouble(0);
                }

                laatste10.Add(new HistorischePrijs
                {
                    Prijs = reader.GetFloat(1),
                    Datum = reader.GetDateTime(2),
                    VeilingNaam = reader.GetString(3),
                    LeverancierNaam = reader.IsDBNull(4) ? "Onbekend" : reader.GetString(4)
                });
            }

            return new PrijsStatistieken
            {
                GemiddeldePrijs = gemiddelde ?? 0,
                Laatste10Prijzen = laatste10
            };
        }
    }

    // Response models
    public class KavelHistoryResponse
    {
        public int KavelId { get; set; }
        public string KavelNaam { get; set; } = string.Empty;
        public string LeverancierNaam { get; set; } = string.Empty;
        public PrijsStatistieken LeverancierStatistieken { get; set; } = new();
        public PrijsStatistieken TotaalStatistieken { get; set; } = new();
    }

    public class PrijsStatistieken
    {
        public float GemiddeldePrijs { get; set; }
        public List<HistorischePrijs> Laatste10Prijzen { get; set; } = new();
    }

    public class HistorischePrijs
    {
        public float Prijs { get; set; }
        public DateTime Datum { get; set; }
        public string VeilingNaam { get; set; } = string.Empty;
        public string? LeverancierNaam { get; set; }
    }

    // Internal models
    internal class KavelInfo
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public int? LeverancierId { get; set; }
        public string LeverancierNaam { get; set; } = string.Empty;
    }
}