import { useState, useEffect } from "react";
import "./VeilingLocatie.css";
import SimpeleKnop from "./SimpeleKnop";
import { useNavigate } from "react-router-dom";

interface Locatie {
  id: number;
  naam: string;
  klokId: number;
  actief: boolean;
}

interface VeilingData {
  id: number;
  naam: string;
  startTijd: string;
  endTijd: string;
  locatieId: number;
}

interface VeilingLocatie {
  id: number;
  naam: string;
  actief: boolean;
  eindTijd: Date | null;
  startTijd: Date | null;
  achtergrondAfbeelding: string;
}

interface VeilingLocatieProps {
  locatie: VeilingLocatie;
  onJoin?: (locatieNaam: string) => void;
}

function VeilingLocatieCard({ locatie, onJoin }: VeilingLocatieProps) {
  const [tijdOver, setTijdOver] = useState<TimeRemaining | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    // eindTijd voor actieve veilingen, startTijd voor inactieve
    const targetTime = locatie.actief ? locatie.eindTijd : locatie.startTijd;

    if (!targetTime) {
      setTijdOver(null);
      return;
    }

    const updateTimer = () => {
      const now = new Date();
      const verschil = targetTime.getTime() - now.getTime();

      if (verschil <= 0) {
        setTijdOver({ uren: 0, minuten: 0, seconden: 0 });
        return;
      }

      const uren = Math.floor(verschil / (1000 * 60 * 60));
      const minuten = Math.floor((verschil % (1000 * 60 * 60)) / (1000 * 60));
      const seconden = Math.floor((verschil % (1000 * 60)) / 1000);

      setTijdOver({ uren, minuten, seconden });
    };

    updateTimer();
    const interval = setInterval(updateTimer, 1000);

    return () => clearInterval(interval);
  }, [locatie.eindTijd, locatie.startTijd, locatie.actief]);

  const formatTijd = () => {
    const targetTime = locatie.actief ? locatie.eindTijd : locatie.startTijd;
    if (!targetTime) return "xx:xx:xx";
    if (!tijdOver) return "0:00:00";
    return `${tijdOver.uren}:${tijdOver.minuten.toString().padStart(2, "0")}:${tijdOver.seconden.toString().padStart(2, "0")}`;
  };

  const getTimerLabel = () => {
    return locatie.actief ? "Veiling eindigt in:" : "Veiling begint in:";
  };

  return (
    <div className="veiling-locatie">
      <div
        className="achtergrond"
        style={{ backgroundImage: `url(${locatie.achtergrondAfbeelding})` }}
      >
        <div className="overlay" />
        <div className="content">
          <div className="header">
            <h2 className="locatie-naam">{locatie.naam}</h2>
            <span className="locatie-status">
              {locatie.actief ? "Actief" : "Non-Actief"}
            </span>
          </div>

          <div className="footer">
            <div className="timer-label">{getTimerLabel()}</div>
            <div className="tijd-display">{formatTijd()}</div>
            <SimpeleKnop
              label="Join"
              onClick={() =>
                onJoin == null
                  ? navigate(`/veiling/${locatie.id}`)
                  : onJoin?.(locatie.naam)
              }
              disabled={!locatie.actief}
              appearance="primary"
            />
          </div>
        </div>
      </div>
    </div>
  );
}

interface TimeRemaining {
  uren: number;
  minuten: number;
  seconden: number;
}

// Actieve locatie afbeeldingen
const locatieAfbeeldingen: Record<string, string> = {
  Amsterdam:
    "https://images.unsplash.com/photo-1534351590666-13e3e96b5017?w=500&h=300&fit=crop",
  Rotterdam:
    "https://www.spido.nl/wp-content/smush-webp/2025/02/Spido-zonsondergang3-scaled.jpeg.webp",
  Delft:
    "https://www.discoverholland.com/product-images/948e5a43-41f5-4d1d-ae2a-4e79ecaed08a.jpg",
};

// Inactieve locatie afbeeldingen
const inactieveAfbeeldingen: Record<string, string> = {
  Amsterdam:
    "https://images.ad.nl/N2E0NzY5ZTY1NzQwZTM1YjRjMDgvZGlvLzI1MDUzMTE0Ni9maXQtd2lkdGgvMTIwMA",
  Rotterdam:
    "https://image.volkskrant.nl/224702415/width/2480/een-menigte-op-het-amsterdamse-mercatorplein-na-de-winst-van",
  Delft:
    "https://redactie.rtl.nl/sites/default/files/content/images/2021/01/24/politie%20auto.jpg?itok=Ij_a8ZWx&offsetX=0&offsetY=31&cropWidth=1917&cropHeight=1078&width=2048&height=1152&impolicy=dynamic",
};

interface VeilingLocatieOverzichtProps {
  onJoin?: (locatieNaam: string) => void;
}

export default function VeilingLocatieOverzicht({
  onJoin,
}: VeilingLocatieOverzichtProps) {
  const [locaties, setLocaties] = useState<VeilingLocatie[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchData() {
      try {
        // Haal locaties op
        const locatiesResponse = await fetch("/api/locaties");
        if (!locatiesResponse.ok) {
          throw new Error(`HTTP error! status: ${locatiesResponse.status}`);
        }
        const locatiesData: Locatie[] = await locatiesResponse.json();

        // Haal actieve veilingen op
        const actieveVeilingenResponse = await fetch("/api/veilingen/actief");
        let actieveVeilingenData: VeilingData[] = [];
        if (actieveVeilingenResponse.ok) {
          actieveVeilingenData = await actieveVeilingenResponse.json();
        }

        // Haal alle veilingen op voor toekomstige veilingen
        const alleVeilingenResponse = await fetch("/api/veilingen");
        let alleVeilingenData: VeilingData[] = [];
        if (alleVeilingenResponse.ok) {
          alleVeilingenData = await alleVeilingenResponse.json();
        }

        const now = new Date();

        const veilingLocaties: VeilingLocatie[] = locatiesData.map((loc) => {
          // Zoek een actieve veiling voor deze locatie
          const actieveVeiling = actieveVeilingenData.find(
            (v) => v.locatieId === loc.id,
          );

          let startTijd: Date | null = null;
          let eindTijd: Date | null = null;

          if (loc.actief) {
            if (actieveVeiling) {
              eindTijd = new Date(actieveVeiling.endTijd + "Z");
            } else {
              const toekomstigeVeilingen = alleVeilingenData
                .filter(
                  (v) =>
                    v.locatieId === loc.id && new Date(v.endTijd + "Z") > now,
                )
                .sort(
                  (a, b) =>
                    new Date(a.endTijd + "Z").getTime() -
                    new Date(b.endTijd + "Z").getTime(),
                );

              if (toekomstigeVeilingen.length > 0) {
                eindTijd = new Date(toekomstigeVeilingen[0].endTijd + "Z");
              }
            }
          } else {
            const toekomstigeVeilingen = alleVeilingenData
              .filter(
                (v) =>
                  v.locatieId === loc.id && new Date(v.startTijd + "Z") > now,
              )
              .sort(
                (a, b) =>
                  new Date(a.startTijd + "Z").getTime() -
                  new Date(b.startTijd + "Z").getTime(),
              );

            if (toekomstigeVeilingen.length > 0) {
              startTijd = new Date(toekomstigeVeilingen[0].startTijd + "Z");
            }
          }

          return {
            id: loc.id,
            naam: loc.naam,
            actief: loc.actief,
            eindTijd: eindTijd,
            startTijd: startTijd,
            achtergrondAfbeelding: loc.actief
              ? locatieAfbeeldingen[loc.naam] ||
                "https://via.placeholder.com/500x300"
              : inactieveAfbeeldingen[loc.naam] ||
                "https://via.placeholder.com/500x300/666666/999999?text=Inactief",
          };
        });

        setLocaties(veilingLocaties);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Er is een fout opgetreden",
        );
        console.error("Fout bij ophalen data:", err);
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, []);

  if (loading) {
    return (
      <div className="veiling-locatie-container">
        <h1 className="section-titel">Kies veiling locatie</h1>
        <p style={{ textAlign: "center", color: "white" }}>Laden...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="veiling-locatie-container">
        <h1 className="section-titel">Kies veiling locatie</h1>
        <p style={{ textAlign: "center", color: "red" }}>Fout: {error}</p>
      </div>
    );
  }

  return (
    <div className="veiling-locatie-container">
      <h1 className="section-titel">Kies veiling locatie</h1>
      <div className="locatie-grid">
        {locaties.map((locatie) => (
          <VeilingLocatieCard
            key={locatie.naam}
            locatie={locatie}
            onJoin={onJoin}
          />
        ))}
      </div>
    </div>
  );
}
