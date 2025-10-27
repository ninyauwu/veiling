// src/App.tsx
import { useState } from 'react';
import './App.css';
import LocationDropdown from './components/location_dropdown.tsx';

function App() {
    const [selectedLocation, setSelectedLocation] = useState('');

    return (
        <div className="app">
            <h1>Veiling Test</h1>

            <LocationDropdown
                value={selectedLocation}
                onChange={setSelectedLocation}
            />

            {selectedLocation && (
                <div className="result">
                    <p>Je hebt gekozen: <strong>{selectedLocation}</strong></p>
                </div>
            )}
        </div>
    );
}

export default App;