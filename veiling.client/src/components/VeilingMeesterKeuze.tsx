import { useNavigate } from "react-router-dom";
import "./VeilingLocatie.css";
import SimpeleKnop from "./SimpeleKnop";

interface Keuze {
    titel: string;
    route: string;
    afbeelding: string;
}

const keuzes: Keuze[] = [
    {
        titel: "Scheduler",
        route: "/scheduler",
        afbeelding:
            "https://www.vuepilot.com/wp-content/uploads/2020/10/projectschedulemanagement.jpg",
    },
    {
        titel: "Kavel Beoordelen",
        route: "/kavel-beoordelen",
        afbeelding:
            "https://media.istockphoto.com/id/2203944828/vector/trendy-halftone-collage-banner-with-hand-showing-thumb-up-gesture-positive-hand-sign-modern.jpg?s=612x612&w=0&k=20&c=FZyO3ucMmd2QXL6JABMlSwEmTGDJD2VaGVKKbIVes0k=",
    },
    {
        titel: "Locaties",
        route: "/locaties",
        afbeelding:
            "https://d2zo35mdb530wx.cloudfront.net/_legacy/UCPthyssenkruppBAMXNL/assets.files/pictures/locaties_image_w2000_h670.jpg",
    },
];

export default function VeilingMeesterKeuze() {
    const navigate = useNavigate();

    return (
        <div className="veiling-locatie-container">
            <h1 className="section-titel">Maak uw keuze</h1>

            <div className="locatie-grid">
                {keuzes.map((keuze) => (
                    <div key={keuze.titel} className="veiling-locatie">
                        <div
                            className="achtergrond"
                            style={{ backgroundImage: `url(${keuze.afbeelding})` }}
                        >
                            <div className="overlay" />

                            <div className="content">
                                <div className="header">
                                    <h2 className="locatie-naam">{keuze.titel}</h2>
                                </div>

                                <div className="footer">
                                    <SimpeleKnop
                                        label="Open"
                                        appearance="primary"
                                        onClick={() => navigate(keuze.route)}
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
