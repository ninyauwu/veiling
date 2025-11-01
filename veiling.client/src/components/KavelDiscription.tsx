import { useState } from 'react';
import './KavelDescription.css';

function KavelDescription() {
    const [description, setDescription] = useState('');
    
    return (
        <div className="kavel-container">
            <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Description"
                className="kavel-textarea"
                rows={4}
            />
        </div>
    );
}

export default KavelDescription;