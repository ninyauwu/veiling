import { useState } from 'react';

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
        <div className="relative w-[200px]">
            <button
                className="w-full px-4 py-3 !bg-white border border-[#ddd] rounded-md cursor-pointer flex items-center gap-3 text-base font-medium transition-all duration-200 hover:!bg-[#f8f8f8] hover:border-[#999]"
                onClick={() => setIsOpen(!isOpen)}
            >
                <span
                    className={`text-xs text-[#333] inline-block transition-transform duration-200 ${
                        isOpen ? 'rotate-90' : ''
                    }`}
                >
                    ▶
                </span>
                <span className="text-[#333] flex-1 text-left">
                    {displayText}
                </span>
            </button>

            {isOpen && (
                <div className="absolute top-[calc(100%+4px)] left-0 right-0 bg-white border border-[#ddd] rounded-md shadow-[0_4px_12px_rgba(0,0,0,0.15)] overflow-hidden z-[1000] animate-[slideDown_0.2s_ease]">
                    {cities.map(city => (
                        <div
                            key={city.value}
                            className={`px-4 py-3 flex items-center cursor-pointer transition-colors duration-150 hover:bg-[#f0f0f0] ${
                                value === city.value ? 'bg-[#e8e8e8]' : ''
                            }`}
                            onClick={() => handleSelect(city.value)}
                        >
                            <span className="text-[#333] text-[15px]">
                                {city.label}
                            </span>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

export default LocationDropdown;