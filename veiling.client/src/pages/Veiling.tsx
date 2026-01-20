import { useState, useCallback } from "react";
import SimpeleKnop from "../components/SimpeleKnop.tsx";
import "../components/KavelInfo.css";
import KavelInfo from "../components/KavelInfo";
import HeaderLoggedout from "../components/HeaderLoggedout";
import { KavelHistoryWidget } from "../components/KavelHistoryWidget";
import { useParams } from "react-router-dom";

function Veiling() {
  const [showHistory, setShowHistory] = useState(false);
  const [activeKavel, setActiveKavel] = useState<number | null>(null);

  const handleSelectKavel = useCallback((k: number) => {
    setActiveKavel(k);
  }, []);
  const { locatieId } = useParams<{ locatieId?: string }>();

  if (!locatieId) {
    return <div>Locatie niet gevonden</div>;
  }

  const locatie = parseInt(locatieId, 10);

  return (
    <div>
      <HeaderLoggedout />
      <div style={{ height: "96px" }} />

      <KavelInfo locatieId={locatie} onSelectKavel={handleSelectKavel} />

      <div style={{ height: "80px" }} />

      <SimpeleKnop
        onClick={() => setShowHistory(true)}
        appearance="primary"
      >
        Toon prijs-geschiedenis
      </SimpeleKnop>

      {showHistory && activeKavel && (
        <KavelHistoryWidget
          kavelId={activeKavel}
          onClose={() => setShowHistory(false)}
        />
      )}
    </div>
  );
}

export default Veiling;