import { Plus } from "lucide-react";
import SimpeleKnop from "../components/SimpeleKnop";
import "./VerkoperDashboard.css";
  import { useNavigate } from "react-router-dom";

// Mock data – replace with API data later
const kavels = [
  { id: 1, title: "Kavel 12", location: "Amsterdam", price: 250000 },
  { id: 2, title: "Kavel 18", location: "Utrecht", price: 310000 },
];

export default function VerkoperDashboard() {
  const navigate = useNavigate();

  return (
    <div className="verkoper-page">
      <div className="verkoper-container">
        <div className="verkoper-header">
          <h1 className="verkoper-title">Mijn kavels</h1>
          const navigate = useNavigate();

          // Then update the button:
          <SimpeleKnop 
            appearance="primary"
            onClick={() => navigate("/invoer")}
          >
            <Plus size={18} />
            Nieuw kavel
          </SimpeleKnop>
        </div>

        {kavels.length === 0 ? (
          <p className="empty-text">Je hebt nog geen kavels aangemaakt.</p>
        ) : (
          <div className="kavels-grid">
            {kavels.map((kavel) => (
              <div key={kavel.id} className="kavel-card">
                <div className="kavel-content">
                  <h2 className="kavel-title">{kavel.title}</h2>
                  <p className="kavel-location">Locatie: {kavel.location}</p>
                  <p className="kavel-price">€ {kavel.price.toLocaleString()}</p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
