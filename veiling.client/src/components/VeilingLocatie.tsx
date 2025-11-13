// veiling.client/src/components/VeilingLocatie.tsx
import { useState, useEffect } from "react";
import "./VeilingLocatie.css";
import SimpeleKnop from "./SimpeleKnop";

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
    naam: string;
    actief: boolean;
    eindTijd: Date | null;
    achtergrondAfbeelding: string;
}

interface VeilingLocatieProps {
    locatie: VeilingLocatie;
    onJoin?: (locatieNaam: string) => void;
}

function VeilingLocatieCard({ locatie, onJoin }: VeilingLocatieProps) {
    const [tijdOver, setTijdOver] = useState<TimeRemaining | null>(null);

    useEffect(() => {
        if (!locatie.eindTijd) {
            setTijdOver(null);
            return;
        }

        const updateTimer = () => {
            const now = new Date();
            const verschil = locatie.eindTijd!.getTime() - now.getTime();

            if (verschil <= 0) {
                setTijdOver({ uren: 0, minuten: 0 });
                return;
            }

            const uren = Math.floor(verschil / (1000 * 60 * 60));
            const minuten = Math.floor((verschil % (1000 * 60 * 60)) / (1000 * 60));

            setTijdOver({ uren, minuten });
        };

        updateTimer();
        const interval = setInterval(updateTimer, 1000);

        return () => clearInterval(interval);
    }, [locatie.eindTijd]);

    const formatTijd = () => {
        if (!locatie.eindTijd) return "xx:xx";
        if (!tijdOver) return "0:00";
        return `${tijdOver.uren}:${tijdOver.minuten.toString().padStart(2, "0")}`;
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
                        <div className="tijd-display">{formatTijd()}</div>
                        <SimpeleKnop
                            label="Join"
                            onClick={() => onJoin?.(locatie.naam)}
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
}

const locatieAfbeeldingen: Record<string, string> = {
    "Amsterdam": "https://images.unsplash.com/photo-1534351590666-13e3e96b5017?w=500&h=300&fit=crop",
    "Rotterdam": "https://image.volkskrant.nl/224702415/width/2480/een-menigte-op-het-amsterdamse-mercatorplein-na-de-winst-van",
    "Delft": "https://www.discoverholland.com/product-images/948e5a43-41f5-4d1d-ae2a-4e79ecaed08a.jpg",
};

export default function VeilingLocatieOverzicht() {
    const [locaties, setLocaties] = useState<VeilingLocatie[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function fetchData() {
            try {
                // Haal locaties op
                const locatiesResponse = await fetch('/api/locaties');
                if (!locatiesResponse.ok) {
                    throw new Error(`HTTP error! status: ${locatiesResponse.status}`);
                }
                const locatiesData: Locatie[] = await locatiesResponse.json();

                // Haal actieve veilingen op
                const veilingenResponse = await fetch('/api/veilingen/actief');
                let veilingenData: VeilingData[] = [];
                if (veilingenResponse.ok) {
                    veilingenData = await veilingenResponse.json();
                }

                const veilingLocaties: VeilingLocatie[] = locatiesData.map((loc) => {
                    // Zoek actieve veiling voor deze locatie
                    const actieveVeiling = veilingenData.find(v => v.locatieId === loc.id);

                    return {
                        naam: loc.naam,
                        actief: loc.actief && !!actieveVeiling, // Alleen actief als er een veiling is
                        eindTijd: actieveVeiling
                            ? new Date(actieveVeiling.endTijd)
                            : null,
                        achtergrondAfbeelding: locatieAfbeeldingen[loc.naam] || "https://via.placeholder.com/500x300",
                    };
                });

                setLocaties(veilingLocaties);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'Er is een fout opgetreden');
                console.error('Fout bij ophalen data:', err);
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, []);

    const handleJoin = (locatieNaam: string) => {
        console.log(`Joining veiling in ${locatieNaam}`);
    };

    if (loading) {
        return (
            <div className="veiling-locatie-container">
                <h1 className="section-titel">Kies veiling locatie</h1>
                <p style={{ textAlign: 'center', color: 'white' }}>Laden...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="veiling-locatie-container">
                <h1 className="section-titel">Kies veiling locatie</h1>
                <p style={{ textAlign: 'center', color: 'red' }}>Fout: {error}</p>
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
                        onJoin={handleJoin}
                    />
                ))}
            </div>
        </div>
    );
}