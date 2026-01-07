using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Veiling.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KavelHistoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KavelHistoryController> _logger;

        public KavelHistoryController(
            IConfiguration configuration,
            ILogger<KavelHistoryController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private string GetConnectionString()
        {
            var baseConnection = _configuration.GetConnectionString("Default");

            var server = Environment.GetEnvironmentVariable("DB_SERVER");
            var username = Environment.GetEnvironmentVariable("DB_USERNAME");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            return baseConnection +
                   $"Server={server};User Id={username};Password={password};";
        }

        // GET api/kavelhistory/{kavelId}
        [HttpGet("{kavelId}")]
        public async Task<ActionResult<KavelHistoryResponse>> GetKavelHistory(int kavelId)
        {
            try
            {
                await using var connection = new SqlConnection(GetConnectionString());
                await connection.OpenAsync();

                var kavel = await GetKavelInfo(connection, kavelId);
                if (kavel == null)
                {
                    return NotFound(new { message = "Kavel niet gevonden" });
                }

                var leverancierStats = await GetLeverancierHistory(
                    connection,
                    kavel.LeverancierId,
                    kavel.Naam
                );

                var totaalStats = await GetTotaalHistory(
                    connection,
                    kavel.Naam
                );

                return Ok(new KavelHistoryResponse
                {
                    KavelId = kavel.Id,
                    KavelNaam = kavel.Naam,
                    LeverancierNaam = kavel.LeverancierNaam,
                    LeverancierStatistieken = leverancierStats,
                    TotaalStatistieken = totaalStats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching kavel history");
                return StatusCode(500, new
                {
                    message = "Fout bij ophalen kavelhistorie",
                    detail = ex.Message
                });
            }
        }

        // -------------------- Helpers --------------------

        private async Task<KavelInfo?> GetKavelInfo(SqlConnection connection, int kavelId)
        {
            const string query = @"
                SELECT
                    k.Id,
                    k.Naam,
                    k.LeverancierId,
                    ISNULL(b.Bedrijfsnaam, 'Onbekend') AS LeverancierNaam
                FROM Kavels k
                LEFT JOIN Leveranciers l ON k.LeverancierId = l.Id
                LEFT JOIN Bedrijven b ON l.BedrijfId = b.Bedrijfscode
                WHERE k.Id = @KavelId";

            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@KavelId", kavelId);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new KavelInfo
            {
                Id = reader.GetInt32(0),
                Naam = reader.GetString(1),
                LeverancierId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                LeverancierNaam = reader.GetString(3)
            };
        }

        private async Task<PrijsStatistieken> GetLeverancierHistory(
            SqlConnection connection,
            int? leverancierId,
            string kavelNaam)
        {
            if (!leverancierId.HasValue)
                return new PrijsStatistieken();

            const string query = @"
                SELECT
                    AVG(CAST(k.GekochtPrijs AS FLOAT)) AS GemiddeldePrijs
                FROM Kavels k
                WHERE k.LeverancierId = @LeverancierId
                  AND LTRIM(RTRIM(LOWER(k.Naam))) = LTRIM(RTRIM(LOWER(@Naam)))
                  AND k.GekochtPrijs > 0;

                SELECT TOP 10
                    k.GekochtPrijs,
                    v.Naam AS VeilingNaam,
                    k.Id
                FROM Kavels k
                INNER JOIN Veilingen v ON k.VeilingId = v.Id
                WHERE k.LeverancierId = @LeverancierId
                  AND LTRIM(RTRIM(LOWER(k.Naam))) = LTRIM(RTRIM(LOWER(@Naam)))
                  AND k.GekochtPrijs > 0
                ORDER BY k.Id DESC;
            ";

            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@LeverancierId", leverancierId.Value);
            cmd.Parameters.AddWithValue("@Naam", kavelNaam);

            float gemiddelde = 0;
            var prijzen = new List<HistorischePrijs>();

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync() && !reader.IsDBNull(0))
                gemiddelde = (float)reader.GetDouble(0);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                prijzen.Add(new HistorischePrijs
                {
                    Prijs = reader.GetFloat(0),
                    VeilingNaam = reader.GetString(1),
                    Datum = DateTime.MinValue
                });
            }

            return new PrijsStatistieken
            {
                GemiddeldePrijs = gemiddelde,
                Laatste10Prijzen = prijzen
            };
        }

        private async Task<PrijsStatistieken> GetTotaalHistory(
            SqlConnection connection,
            string kavelNaam)
        {
            const string query = @"
                SELECT
                    AVG(CAST(k.GekochtPrijs AS FLOAT)) AS GemiddeldePrijs
                FROM Kavels k
                WHERE LTRIM(RTRIM(LOWER(k.Naam))) = LTRIM(RTRIM(LOWER(@Naam)))
                  AND k.GekochtPrijs > 0;

                SELECT TOP 10
                    k.GekochtPrijs,
                    v.Naam AS VeilingNaam,
                    ISNULL(b.Bedrijfsnaam, 'Onbekend') AS LeverancierNaam
                FROM Kavels k
                INNER JOIN Veilingen v ON k.VeilingId = v.Id
                LEFT JOIN Leveranciers l ON k.LeverancierId = l.Id
                LEFT JOIN Bedrijven b ON l.BedrijfId = b.Bedrijfscode
                WHERE LTRIM(RTRIM(LOWER(k.Naam))) = LTRIM(RTRIM(LOWER(@Naam)))
                  AND k.GekochtPrijs > 0
                ORDER BY k.Id DESC;
            ";

            await using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Naam", kavelNaam);

            float gemiddelde = 0;
            var prijzen = new List<HistorischePrijs>();

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync() && !reader.IsDBNull(0))
                gemiddelde = (float)reader.GetDouble(0);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
            {
                prijzen.Add(new HistorischePrijs
                {
                    Prijs = reader.GetFloat(0),
                    VeilingNaam = reader.GetString(1),
                    LeverancierNaam = reader.GetString(2),
                    Datum = DateTime.MinValue
                });
            }

            return new PrijsStatistieken
            {
                GemiddeldePrijs = gemiddelde,
                Laatste10Prijzen = prijzen
            };
        }
    }

    // -------------------- DTOs --------------------

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

    internal class KavelInfo
    {
        public int Id { get; set; }
        public string Naam { get; set; } = string.Empty;
        public int? LeverancierId { get; set; }
        public string LeverancierNaam { get; set; } = string.Empty;
    }
}
