import { useState, useRef } from "react";
import "./LocationDropdown.css";

interface LocationDropdownProps {
    value: string;
    onChange: (location: string) => void;
}

interface City {
    value: string;
    label: string;
}

function LocationDropdown({ value, onChange }: LocationDropdownProps) {
    const [isOpen, setIsOpen] = useState(false);
    const itemRefs = useRef<(HTMLDivElement | null)[]>([]);
    const buttonRef = useRef<HTMLButtonElement>(null);

    const cities: City[] = [
        { value: "Rotterdam", label: "Rotterdam" },
        { value: "Amsterdam", label: "Amsterdam" },
        { value: "Delft", label: "Delft" },
    ];

    const selectedCity = cities.find((city) => city.value === value);
    const displayText = selectedCity ? selectedCity.label : "Locaties";

    const handleSelect = (cityValue: string) => {
        onChange(cityValue);
        setIsOpen(false);
        buttonRef.current?.focus();
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
                handleSelect(cities[index].value);
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
                            key={city.value}
                            ref={(el) => { itemRefs.current[index] = el; }}
                            className={`dropdown-item ${value === city.value ? "selected" : ""}`}
                            onClick={() => handleSelect(city.value)}
                            onKeyDown={(e) => handleItemKeyDown(e, index)}
                            tabIndex={0}
                            role="option"
                            aria-selected={value === city.value}
                        >
                            <span className="city-name">{city.label}</span>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

export default LocationDropdown;