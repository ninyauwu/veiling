import { useState } from "react";
import LocationDropdown from "./LocationDropdown";
import SimpeleKnop from "./SimpeleKnop";
import bloomifyLogo from "../assets/bloomify_logo.png";

interface HeaderProps {
  showLocationDropdown?: boolean;
}

function Header({ showLocationDropdown = true }: HeaderProps) {
  const [selectedLocation, setSelectedLocation] = useState("");

  return (
    <header className="bg-white shadow-sm fixed top-0 left-0 right-0 z-50">
      <div className="max-w-[1920px] mx-auto px-8 py-4">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-50">
            <div className="scale-350 origin-left">
              <img
                src={bloomifyLogo}
                alt="Bloomify Logo"
                className="h-16 w-auto"
              />
            </div>

            {showLocationDropdown && (
              <div className="relative z-50">
                <LocationDropdown
                  value={selectedLocation}
                  onChange={setSelectedLocation}
                />
              </div>
            )}
          </div>

          <div className="flex items-center gap-8">
            <SimpeleKnop label="Verkoop" appearance="primary">
              {null}
            </SimpeleKnop>
            <SimpeleKnop label="Login" appearance="secondary">
              {null}
            </SimpeleKnop>
          </div>
        </div>
      </div>
    </header>
  );
}

export default Header;

