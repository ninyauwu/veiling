import { useState } from "react";
import { useNavigate } from "react-router-dom";
import LocationDropdown from "./LocationDropdown";
import SimpeleKnop from "./SimpeleKnop";
import bloomifyLogo from "../assets/bloomify_naam_logo.png";

interface HeaderLoggedOutProps {
  showLocationDropdown?: boolean;
}

function HeaderLoggedOut({ showLocationDropdown = true }: HeaderLoggedOutProps) {
  const [selectedLocation, setSelectedLocation] = useState("");
  const navigate = useNavigate();

  return (
    <header>
      <div style={containerStyle}>
        <div style={{ display: "flex", alignItems: "center", gap: "30px" }}>
          <img
            src={bloomifyLogo}
            alt="Naar Bloomify hoofdpagina"
            style={logoStyle}
            onClick={() => navigate("/")}
          />

          {showLocationDropdown && (
            <LocationDropdown
              value={selectedLocation}
              onChange={setSelectedLocation}
            />
          )}
        </div>

        <div style={{ display: "flex", gap: "20px" }}>
          <SimpeleKnop
            label="Login"
            onClick={() => navigate("/login")}
            appearance="secondary"
          />
          <SimpeleKnop
            label="Registreer"
            onClick={() => navigate("/registreer")}
            appearance="primary"
          />
        </div>
      </div>
    </header>
  );
}

const containerStyle = {
  width: "100%",
  height: "80px",
  backgroundColor: "white",
  display: "flex",
  alignItems: "center",
  justifyContent: "space-between",
  padding: "0 40px",
  boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
  position: "fixed" as const,
  top: 0,
  left: 0,
  zIndex: 1000,
};

const logoStyle = {
  height: "80px",
  cursor: "pointer",
  marginTop: "4px",
};

export default HeaderLoggedOut;
