import { useState, useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./LocationDropdown.css";
import { authFetch } from "../utils/AuthFetch";

interface LocationDropdownProps {
  value: string;
  onChange: (location: string) => void;
}

interface City {
  id: number;
  naam: string;
}

function LocationDropdown({ value, onChange }: LocationDropdownProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [cities, setCities] = useState<City[]>([]);
  const [loading, setLoading] = useState(true);
  const itemRefs = useRef<(HTMLDivElement | null)[]>([]);
  const buttonRef = useRef<HTMLButtonElement>(null);
  const navigate = useNavigate();

  useEffect(() => {
    async function fetchLocaties() {
      try {
        const response = await authFetch("/api/locaties");
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data: City[] = await response.json();
        setCities(data);
      } catch (err) {
        console.error("Fout bij ophalen locaties:", err);
      } finally {
        setLoading(false);
      }
    }

    fetchLocaties();
  }, []);

  const selectedCity = cities.find((city) => city.naam === value);
  const displayText = selectedCity ? selectedCity.naam : "Locaties";

  const handleSelect = (city: City) => {
    onChange(city.naam);
    setIsOpen(false);
    buttonRef.current?.focus();

    // Navigate to the veiling page for this location
    navigate(`/veiling/${city.id}`);
  };

  const handleButtonKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "ArrowDown") {
      e.preventDefault();
      if (!isOpen) {
        setIsOpen(true);
      }
      setTimeout(() => itemRefs.current[0]?.focus(), 0);
    } else if (e.key === "Escape" && isOpen) {
      setIsOpen(false);
    }
  };

  const handleItemKeyDown = (e: React.KeyboardEvent, index: number) => {
    switch (e.key) {
      case "ArrowDown":
        e.preventDefault();
        if (index < cities.length - 1) {
          itemRefs.current[index + 1]?.focus();
        }
        break;
      case "ArrowUp":
        e.preventDefault();
        if (index > 0) {
          itemRefs.current[index - 1]?.focus();
        } else {
          buttonRef.current?.focus();
        }
        break;
      case "Enter":
      case " ":
        e.preventDefault();
        handleSelect(cities[index]);
        break;
      case "Escape":
        e.preventDefault();
        setIsOpen(false);
        buttonRef.current?.focus();
        break;
      case "Tab":
        setIsOpen(false);
        break;
    }
  };

  if (loading) {
    return (
      <div className="custom-dropdown">
        <button className="dropdown-button" disabled>
          <span className="dropdown-text">Laden...</span>
        </button>
      </div>
    );
  }

  return (
    <div className="custom-dropdown">
      <button
        ref={buttonRef}
        className="dropdown-button"
        onClick={() => setIsOpen(!isOpen)}
        onKeyDown={handleButtonKeyDown}
        aria-expanded={isOpen}
        aria-haspopup="listbox"
      >
        <span className={`arrow ${isOpen ? "open" : ""}`}>▶</span>
        <span className="dropdown-text">{displayText}</span>
      </button>
      {isOpen && (
        <div className="dropdown-menu" role="listbox">
          {cities.map((city, index) => (
            <div
              key={city.id}
              ref={(el) => {
                itemRefs.current[index] = el;
              }}
              className={`dropdown-item ${value === city.naam ? "selected" : ""}`}
              onClick={() => handleSelect(city)}
              onKeyDown={(e) => handleItemKeyDown(e, index)}
              tabIndex={0}
              role="option"
              aria-selected={value === city.naam}
            >
              <span className="city-name">{city.naam}</span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default LocationDropdown;

