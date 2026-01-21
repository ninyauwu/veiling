import { useState, useEffect } from "react";
import "./VeilingLocatie.css";
import SimpeleKnop from "./SimpeleKnop";
import { useNavigate } from "react-router-dom";
import { authFetch } from "../utils/AuthFetch";

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
  Aalsmeer:
    "https://www.visitaalsmeer.nl/wp-content/uploads/2024/02/historische_tuin_aalsmeer_01-1920x1280.jpg",
  Naaldwijk:
    "https://www.naaldwijkwinkelrijk.nl/media/djllsmbl/foto-plein.jpg",
  Rijnsburg:
        "https://dynamic-media-cdn.tripadvisor.com/media/photo-o/2a/d2/25/71/caption.jpg?w=600&h=-1&s=1",
  Eelde:
        "https://images.fd.nl/v0s0N2Yyzp5oGTP4Rts2BwuarfE.jpg?auto=format&w=1280&q=45",
  RheinMaas:
        "https://www.veilingrheinmaas.com/fileadmin/_processed_/6/3/csm_Inkoopkanalen_en_diensten_bij_Veiling_Rhein_Maas_2892109269.png",
};

// Inactieve locatie afbeeldingen
const inactieveAfbeeldingen: Record<string, string> = {
    Aalsmeer:
        "https://radioaalsmeer.nl/wp-content/uploads/2016/09/aalsmeer-nacht-5371-1200x630.jpg",
    Naaldwijk:
        "https://images.ad.nl/NDcyYzQ0MmFmMjQxMDM5NzllMTUvZGlvLzI2NjIzMjYwNy9maXQtd2lkdGgvMTIwMA/het-gemeentehuis-aan-de-verdilaan-in-naaldwijk-kleurt-oranje-vanwege-de-wereldwijde-campagne-orange-the-world-die-zich-richt-op-geweld-tegen-vrouwen-en-meisjes",
    Rijnsburg:
        "https://www.ovrijnsburg.nl/wp-content/uploads/2021/04/kerk-background-rijnsburg.jpg",
    Eelde:
        "https://avondwinkel.nu/wp-content/uploads/2024/01/Avondwinkel-mpg_plaats.jpg",
    "Rhein-Maas":
        "https://mp.reshift.nl/zoom/7c47c652-3827-45df-8413-dd1e30d7d325-donkere-wolken-boven-de-maas.jpg?w=1200",
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
        const locatiesResponse = await authFetch("/api/locaties");
        if (!locatiesResponse.ok) {
          throw new Error(`HTTP error! status: ${locatiesResponse.status}`);
        }
        const locatiesData: Locatie[] = await locatiesResponse.json();

        // Haal actieve veilingen op
        const actieveVeilingenResponse = await authFetch("/api/veilingen/actief");
        let actieveVeilingenData: VeilingData[] = [];
        if (actieveVeilingenResponse.ok) {
          actieveVeilingenData = await actieveVeilingenResponse.json();
        }

        // Haal alle veilingen op voor toekomstige veilingen
        const alleVeilingenResponse = await authFetch("/api/veilingen");
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
