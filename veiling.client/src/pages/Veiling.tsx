import { useState, useCallback } from "react";
import SimpeleKnop from "../components/SimpeleKnop.tsx";
import "../components/KavelInfo.css";
import KavelInfo from "../components/KavelInfo";
import HeaderLoggedout from "../components/HeaderLoggedout";
import { KavelHistoryWidget } from "../components/KavelHistoryWidget";

function Veiling() {
  const [showHistory, setShowHistory] = useState(false);
  const [activeKavel, setActiveKavel] = useState<number | null>(null);

  const handleSelectKavel = useCallback((k: number) => {
    setActiveKavel(k);
  }, []);

  return (
    <div>
      <HeaderLoggedout />
      <div style={{ height: "96px" }} />

      <KavelInfo onSelectKavel={handleSelectKavel} />

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