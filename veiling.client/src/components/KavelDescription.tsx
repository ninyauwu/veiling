import { useState } from 'react';
import './KavelDescription.css';

interface KavelDescriptionProps {
    onDescriptionChange: (description: string) => void;
}

function KavelDescription({ onDescriptionChange }: KavelDescriptionProps) {
    const [description, setDescription] = useState('');

    const handleDescriptionChange = (value: string) => {
        setDescription(value);
        
        onDescriptionChange(value);
    };
    
    return (
        <div className="kavel-container">
            <textarea
                value={description}
                onChange={(e) => handleDescriptionChange(e.target.value)}
                placeholder="Description"
                className="kavel-textarea"
                rows={4}
            />
        </div>
    );
}

export default KavelDescription;