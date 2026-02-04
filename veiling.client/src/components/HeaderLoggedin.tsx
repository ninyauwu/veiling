import { useState } from "react";
import { useNavigate } from "react-router-dom";
import LocationDropdown from "./LocationDropdown";
import AccountDropdown, { AccountDropdownLine } from "./AccountDropdown";
import bloomifyLogo from "../assets/bloomify_naam_logo.png";

interface HeaderLoggedInProps {
    email: string;
    roles: string[];
    showLocationDropdown?: boolean;
}

function HeaderLoggedIn({
    email,
    roles,
    showLocationDropdown = true,
}: HeaderLoggedInProps) {
    const [selectedLocation, setSelectedLocation] = useState("");
    const navigate = useNavigate();

    function handleLogout() {
        localStorage.removeItem("access_token");
        localStorage.removeItem("refresh_token");
        window.location.href = "/login";
    }

    const role =
        roles.includes("Veilingmeester")
            ? "Veilingmeester"
            : roles.includes("Leverancier")
                ? "Leverancier"
                : "Bedrijfsvertegenwoordiger";

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

                <AccountDropdown
                    align="right"
                    onLogout={handleLogout}
                    role={role}
                >
                    <AccountDropdownLine>
                        <strong>{email}</strong>
                    </AccountDropdownLine>

                    <AccountDropdownLine>
                        Rollen: {roles.join(", ")}
                    </AccountDropdownLine>
                </AccountDropdown>
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

export default HeaderLoggedIn;
