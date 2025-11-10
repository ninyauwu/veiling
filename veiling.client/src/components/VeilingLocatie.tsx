import { useState, useEffect } from "react";
import "./VeilingLocatie.css";
import SimpeleKnop from "./SimpeleKnop";

interface VeilingLocatie {
    naam: string;
    actief: boolean;
    eindTijd: Date | null; // null betekent nog niet gestart (xx:xx)
    achtergrondAfbeelding: string;
}

interface VeilingLocatieProps {
    locatie: VeilingLocatie;
    onJoin?: (locatieNaam: string) => void;
}

function VeilingLocatie({ locatie, onJoin }: VeilingLocatieProps) {
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

// Hoofd component dat alle drie de locaties toont
export default function VeilingLocatieOverzicht() {
    const locaties: VeilingLocatie[] = [
        {
            naam: "Amsterdam",
            actief: true,
            // Over 5 uur en 20 minuten 
            eindTijd: new Date(Date.now() + 5 * 60 * 60 * 1000 + 20 * 60 * 1000),
            achtergrondAfbeelding:
                "https://images.unsplash.com/photo-1534351590666-13e3e96b5017?w=500&h=300&fit=crop", 
        },
        {
            naam: "Rotterdam",
            actief: false,
            eindTijd: null, // Nog niet gestart
            achtergrondAfbeelding:
                "https://image.volkskrant.nl/224702415/width/2480/een-menigte-op-het-amsterdamse-mercatorplein-na-de-winst-van", 
        },
        {
            naam: "Delft",
            actief: true,
            // Over 4 uur en 25 minuten 
            eindTijd: new Date(Date.now() + 4 * 60 * 60 * 1000 + 25 * 60 * 1000),
            achtergrondAfbeelding:
                "https://www.discoverholland.com/product-images/948e5a43-41f5-4d1d-ae2a-4e79ecaed08a.jpg", 
        },
    ];

    const handleJoin = (locatieNaam: string) => {
        console.log(`Joining veiling in ${locatieNaam}`);
    };

    return (
        <div className="veiling-locatie-container">
            <h1 className="section-titel">Veiling locatie</h1>
            <div className="locatie-grid">
                {locaties.map((locatie) => (
                    <VeilingLocatie
                        key={locatie.naam}
                        locatie={locatie}
                        onJoin={handleJoin}
                    />
                ))}
            </div>
        </div>
    );
}