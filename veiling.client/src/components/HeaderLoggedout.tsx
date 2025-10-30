import { useState } from "react";
import { useNavigate } from "react-router-dom";
import LocationDropdown from "./LocationDropdown";
import SimpeleKnop from "./SimpeleKnop";
import bloomifyLogo from "../assets/bloomify_naam_logo.png";

interface HeaderProps {
  showLocationDropdown?: boolean;
}

function Header({ showLocationDropdown = true }: HeaderProps) {
  const [selectedLocation, setSelectedLocation] = useState("");
  const navigate = useNavigate();

  return (
    <div
      style={{
        width: "100%",
        height: "80px",
        backgroundColor: "white",
        display: "flex",
        alignItems: "center",
        justifyContent: "space-between",
        padding: "0 40px",
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
        position: "fixed",
        top: 0,
        left: 0,
        zIndex: 1000,
      }}
    >
      <div style={{ display: "flex", alignItems: "center", gap: "30px" }}>
        <img
          src={bloomifyLogo}
          alt="Bloomify"
          style={{
            height: "80px",
            cursor: "pointer",
            marginTop: "4px",
          }}
          onClick={() => navigate("/home")}
        />

        {showLocationDropdown && (
          <div>
            <LocationDropdown
              value={selectedLocation}
              onChange={setSelectedLocation}
            />
          </div>
        )}
      </div>

      <div style={{ display: "flex", alignItems: "center", gap: "20px" }}>
        <SimpeleKnop
          label="Login"
          onClick={() => navigate("/login")}
          appearance="secondary"
        />
        <SimpeleKnop
          label="Registreer"
          onClick={() => {}}
          appearance="primary"
        />
      </div>
    </div>
  );
}

export default Header;

