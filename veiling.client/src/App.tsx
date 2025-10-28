import { useState } from 'react';
import LocationDropdown from './components/location_dropdown.tsx';

function App() {
    const [selectedLocation, setSelectedLocation] = useState('');

    return (
        <div className="max-w-[1280px] mx-auto p-8 text-center">
            <h1 className="text-4xl font-bold mb-8">Veiling Test</h1>

            <div className="flex justify-center mb-6">
                <LocationDropdown
                    value={selectedLocation}
                    onChange={setSelectedLocation}
                />
            </div>

            {selectedLocation && (
                <div className="mt-6">
                    <p className="text-lg">
                        Je hebt gekozen: <strong className="font-semibold">{selectedLocation}</strong>
                    </p>
                </div>
            )}
        </div>
    );
}

export default App;