import { useEffect, useState } from 'react';
import { X, TrendingUp, Clock, Package } from 'lucide-react';
import './KavelHistoryWidget.css';

interface HistorischePrijs {
  prijs: number;
  datum: string;
  veilingNaam: string;
  leverancierNaam?: string;
}

interface PrijsStatistieken {
  gemiddeldePrijs: number;
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
    const fetchHistory = async () => {
      try {
        setLoading(true);
        setError(null);

        console.log(`Fetching history for kavel ${kavelId}...`);
        const response = await fetch(`/api/kavelhistory/${kavelId}`);
        
        console.log('Response status:', response.status);
        console.log('Response ok:', response.ok);
        
        if (!response.ok) {
          const errorData = await response.json().catch(() => null);
          console.error('Error response:', errorData);
          throw new Error(errorData?.message || errorData?.detail || `Server fout: ${response.status}`);
        }

        const historyData = await response.json();
        console.log('History data received:', historyData);
        setData(historyData);
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'Er is een onbekende fout opgetreden';
        console.error('Error fetching kavel history:', err);
        setError(errorMessage);
      } finally {
        setLoading(false);
      }
    };

    fetchHistory();
  }, [kavelId]);

  const formatPrijs = (prijs: number) => {
    return new Intl.NumberFormat('nl-NL', {
      style: 'currency',
      currency: 'EUR',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(prijs);
  };

  const formatDatum = (datumString: string) => {
    const datum = new Date(datumString);
    return new Intl.DateTimeFormat('nl-NL', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(datum);
  };

  if (loading) {
    return (
      <div className="kavel-history-overlay" onClick={onClose}>
        <div className="kavel-history-modal" onClick={(e) => e.stopPropagation()}>
          <div className="kavel-history-loading">
            <div className="loading-spinner"></div>
            <p>Geschiedenis laden...</p>
          </div>
        </div>
      </div>
    );
  }

  if (error || !data) {
    return (
      <div className="kavel-history-overlay" onClick={onClose}>
        <div className="kavel-history-modal" onClick={(e) => e.stopPropagation()}>
          <div className="kavel-history-header">
            <h2>Fout</h2>
            <button onClick={onClose} className="close-button">
              <X size={24} />
            </button>
          </div>
          <div className="kavel-history-error">
            <p>{error || 'Geen data beschikbaar'}</p>
            <button onClick={onClose} className="btn-primary">
              Sluiten
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="kavel-history-overlay" onClick={onClose}>
      <div className="kavel-history-modal" onClick={(e) => e.stopPropagation()}>
        <div className="kavel-history-header">
          <div className="header-content">
            <Package size={28} />
            <div>
              <h2>{data.kavelNaam}</h2>
              <p className="subtitle">Prijsgeschiedenis</p>
            </div>
          </div>
          <button onClick={onClose} className="close-button">
            <X size={24} />
          </button>
        </div>

        <div className="kavel-history-content">
          {/* Leverancier Statistieken */}
          <div className="stats-section">
            <div className="section-header">
              <TrendingUp size={20} />
              <h3>Deze Leverancier: {data.leverancierNaam}</h3>
            </div>
            
            <div className="gemiddelde-prijs">
              <span className="label">Gemiddelde prijs:</span>
              <span className="prijs-groot">
                {formatPrijs(data.leverancierStatistieken.gemiddeldePrijs)}
              </span>
            </div>

            {data.leverancierStatistieken.laatste10Prijzen.length > 0 ? (
              <div className="prijzen-lijst">
                <h4><Clock size={16} /> Laatste 10 verkopen</h4>
                <div className="prijzen-tabel">
                  {data.leverancierStatistieken.laatste10Prijzen.map((prijs) => (
                    <div key={`${prijs.datum}-${prijs.veilingNaam}`} className="prijs-rij">
                      <span className="datum">{formatDatum(prijs.datum)}</span>
                      <span className="veiling">{prijs.veilingNaam}</span>
                      <span className="prijs">{formatPrijs(prijs.prijs)}</span>
                    </div>
                  ))}
                </div>
              </div>
            ) : (
              <div className="geen-data">
                <p>Geen eerdere verkopen van deze leverancier</p>
              </div>
            )}
          </div>

          {/* Totaal Statistieken */}
          <div className="stats-section stats-totaal">
            <div className="section-header">
              <TrendingUp size={20} />
              <h3>Alle Leveranciers</h3>
            </div>
            
            <div className="gemiddelde-prijs">
              <span className="label">Gemiddelde prijs:</span>
              <span className="prijs-groot">
                {formatPrijs(data.totaalStatistieken.gemiddeldePrijs)}
              </span>
            </div>

            {data.totaalStatistieken.laatste10Prijzen.length > 0 ? (
              <div className="prijzen-lijst">
                <h4><Clock size={16} /> Laatste 10 verkopen</h4>
                <div className="prijzen-tabel">
                  {data.totaalStatistieken.laatste10Prijzen.map((prijs) => (
                    <div key={`${prijs.datum}-${prijs.veilingNaam}`} className="prijs-rij">
                      <span className="datum">{formatDatum(prijs.datum)}</span>
                      <span className="leverancier">{prijs.leverancierNaam || 'Onbekend'}</span>
                      <span className="veiling">{prijs.veilingNaam}</span>
                      <span className="prijs">{formatPrijs(prijs.prijs)}</span>
                    </div>
                  ))}
                </div>
              </div>
            ) : (
              <div className="geen-data">
                <p>Geen eerdere verkopen beschikbaar</p>
              </div>
            )}
          </div>
        </div>

        <div className="kavel-history-footer">
          <button onClick={onClose} className="btn-secondary">
            Sluiten
          </button>
        </div>
      </div>
    </div>
  );
}