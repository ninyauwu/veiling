import { Plus } from "lucide-react";
import SimpeleKnop from "../components/SimpeleKnop";
import "./VerkoperDashboard.css";
import { useNavigate } from "react-router-dom";
import { authFetch } from "../utils/AuthFetch";
import { useEffect, useState } from "react";

type Kavel = {
  id: number;
  title: string;
  location: string;
  price: number;
};

export default function VerkoperDashboard() {
  const navigate = useNavigate();
  const [kavels, setKavels] = useState<Kavel[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
  const retrieveKavels = async () => {
    try {
      const response = await authFetch("/api/leveranciers/mijn/kavels");

      if (!response.ok) {
        throw new Error("Kon kavels niet ophalen");
      }

      const data = await response.json();
      setKavels(data);
    } catch (err) {
      console.error("Fout bij ophalen kavels:", err);
    } finally {
      setLoading(false);
    }
  };

  retrieveKavels();
}, []);

  if (loading) {
    return <p className="loading-text">Kavels laden…</p>;
  }

  return (
    <div className="verkoper-page">
      <div className="verkoper-container">
        <div className="verkoper-header">
          <h1 className="verkoper-title">Mijn kavels</h1>

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
                  <p className="kavel-price">
                    € {kavel.price.toLocaleString()}
                  </p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
