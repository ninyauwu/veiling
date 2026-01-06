import { useEffect, useState } from "react";
import { X, TrendingUp, Clock, Package } from "lucide-react";
import "./KavelHistoryWidget.css";

interface HistorischePrijs {
  prijs: number | null;
  datum: string;
  veilingNaam: string;
  leverancierNaam?: string | null;
}

interface PrijsStatistieken {
  gemiddeldePrijs: number | null;
  laatste10Prijzen: HistorischePrijs[];
}

interface KavelHistoryData {
  kavelId: number;
  kavelNaam: string;
  leverancierNaam: string;
  leverancierStatistieken: PrijsStatistieken;
  totaalStatistieken: PrijsStatistieken;
}

interface KavelHistoryWidgetProps {
  kavelId: number;
  onClose: () => void;
}

export function KavelHistoryWidget({ kavelId, onClose }: KavelHistoryWidgetProps) {
  const [data, setData] = useState<KavelHistoryData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let cancelled = false;

    async function fetchHistory() {
      try {
        setLoading(true);
        setError(null);

        const response = await fetch(`/api/kavelhistory/${kavelId}`);

        if (!response.ok) {
          throw new Error(`API fout (${response.status})`);
        }

        const json = await response.json();

        if (!cancelled) {
          setData(json);
        }
      } catch (err) {
        if (!cancelled) {
          console.error("KavelHistory error:", err);
          setError("Kon prijsgeschiedenis niet laden");
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    if (kavelId != null) {
      fetchHistory();
    }

    return () => {
      cancelled = true;
    };
  }, [kavelId]);

  const formatPrijs = (prijs: number | null | undefined) => {
    if (prijs == null) return "—";
    return new Intl.NumberFormat("nl-NL", {
      style: "currency",
      currency: "EUR",
    }).format(prijs);
  };

  const formatDatum = (datumString: string) => {
    const d = new Date(datumString);
    if (isNaN(d.getTime())) return "—";
    return new Intl.DateTimeFormat("nl-NL", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    }).format(d);
  };

  /* ---------------- LOADING ---------------- */

  if (loading) {
    return (
      <div className="kavel-history-overlay">
        <div className="kavel-history-modal">
          <p>Geschiedenis laden…</p>
        </div>
      </div>
    );
  }

  /* ---------------- ERROR ---------------- */

  if (error || !data) {
    return (
      <div className="kavel-history-overlay">
        <div className="kavel-history-modal">
          <div className="kavel-history-header">
            <h2>Fout</h2>
            <button onClick={onClose} className="close-button">
              <X size={24} />
            </button>
          </div>
          <p>{error ?? "Geen data beschikbaar"}</p>
        </div>
      </div>
    );
  }

  const leverancierPrijzen =
    data.leverancierStatistieken?.laatste10Prijzen ?? [];
  const totaalPrijzen =
    data.totaalStatistieken?.laatste10Prijzen ?? [];

  /* ---------------- UI ---------------- */

  return (
    <div className="kavel-history-overlay">
      <div
        className="kavel-history-modal"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="kavel-history-header">
          <Package size={28} />
          <div>
            <h2>{data.kavelNaam}</h2>
            <p>Prijsgeschiedenis</p>
          </div>
          <button onClick={onClose} className="close-button">
            <X size={24} />
          </button>
        </div>

        {/* Leverancier */}
        <section>
          <h3>
            <TrendingUp size={16} /> {data.leverancierNaam}
          </h3>
          <p>
            Gemiddelde prijs:{" "}
            <strong>
              {formatPrijs(data.leverancierStatistieken?.gemiddeldePrijs)}
            </strong>
          </p>

          {leverancierPrijzen.length === 0 ? (
            <p>Geen verkopen</p>
          ) : (
            leverancierPrijzen.map((p, i) => (
              <div key={i}>
                <Clock size={14} /> {formatDatum(p.datum)} —{" "}
                {p.veilingNaam} — {formatPrijs(p.prijs)}
              </div>
            ))
          )}
        </section>

        {/* Totaal */}
        <section>
          <h3>
            <TrendingUp size={16} /> Alle leveranciers
          </h3>
          <p>
            Gemiddelde prijs:{" "}
            <strong>
              {formatPrijs(data.totaalStatistieken?.gemiddeldePrijs)}
            </strong>
          </p>

          {totaalPrijzen.length === 0 ? (
            <p>Geen verkopen</p>
          ) : (
            totaalPrijzen.map((p, i) => (
              <div key={i}>
                <Clock size={14} /> {formatDatum(p.datum)} —{" "}
                {p.leverancierNaam ?? "Onbekend"} — {p.veilingNaam} —{" "}
                {formatPrijs(p.prijs)}
              </div>
            ))
          )}
        </section>
      </div>
    </div>
  );
}
