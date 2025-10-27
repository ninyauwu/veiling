// src/components/location_dropdown.tsx
import { useState } from 'react';
import './location_dropdown.css';

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

    const cities: City[] = [
        { value: 'Rotterdam', label: 'Rotterdam' },
        { value: 'Amsterdam', label: 'Amsterdam' },
        { value: 'Delft', label: 'Delft' },
    ];

    const selectedCity = cities.find(city => city.value === value);
    const displayText = selectedCity ? selectedCity.label : 'Locaties';

    const handleSelect = (cityValue: string) => {
        onChange(cityValue);
        setIsOpen(false);
    };

    return (
        <div className="custom-dropdown">
            <button
                className="dropdown-button"
                onClick={() => setIsOpen(!isOpen)}
            >
                <span className={`arrow ${isOpen ? 'open' : ''}`}>▶</span>
                <span className="dropdown-text">{displayText}</span>
            </button>

            {isOpen && (
                <div className="dropdown-menu">
                    {cities.map(city => (
                        <div
                            key={city.value}
                            className={`dropdown-item ${value === city.value ? 'selected' : ''}`}
                            onClick={() => handleSelect(city.value)}
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