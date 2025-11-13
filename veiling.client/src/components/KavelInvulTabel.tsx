import { useState } from 'react';
import './KavelInvulTabel.css';

interface KavelInvulTabelProps {
    onDataChange: (data: any, isValid: boolean) => void;
}

function KavelInvulTabel({ onDataChange }: KavelInvulTabelProps) {
    const [formData, setFormData] = useState({
    naam: '',
    prijs: '',
    aantal: '',
    ql: '',
    plaats: '',
    stadium: '',
    lengte: '',
    kleur: '',
    fustcode: ''
    });

    const [errors, setErrors] = useState<{ [key: string]: string }>({});

    const isWholeNumber = (value: string) => /^[0-9]+$/.test(value);
    const isDecimal = (value: string) => /^\d+(\.\d{1,2})?$/.test(value); // 12 or 12.34

    const validateField = (field: string, value: string): string => {
        if (!value.trim()) return 'Dit veld is verplicht.';

        if (field === 'aantal' && !isWholeNumber(value)) {
            return 'Geheel getal.';
        }

        if (field === 'prijs' && !isDecimal(value)) {
            return 'Geldig decimaal astublieft(bijv. 12.50).';
        }

        return '';
    };

    const validateForm = (data: typeof formData) => {
        return Object.values(data).every(v => v.trim() !== '') &&
        isWholeNumber(data.aantal) &&
        isDecimal(data.prijs);
    };

    const handleInputChange = (field: string, value: string) => {
        const updatedData = { ...formData, [field]: value };
        setFormData(updatedData);

        const fieldError = validateField(field, value);
        const newErrors = { ...errors, [field]: fieldError };
        setErrors(newErrors);

        const isValid = validateForm(updatedData);
        onDataChange(updatedData, isValid);
    };

    const renderInput = (label: string, field: keyof typeof formData, placeholder: string) => (
        <div className="kavel-invul-row">
        <div className="kavel-invul-label">{label}</div>
        <input
            type="text"
            className={`kavel-invul-input ${errors[field] ? 'input-error' : ''}`}
            placeholder={placeholder}
            value={formData[field]}
            onChange={(e) => handleInputChange(field, e.target.value)}
        />
        {errors[field] && <div className="error-text">{errors[field]}</div>}
        </div>
    );

    return (
        <div className="kavel-invul-container">
            <div className="kavel-invul-header">Invullen</div>

            {renderInput('Naam', 'naam', 'Naam')}
            {renderInput('Prijs (€)', 'prijs', 'Bijv. 12.50')}
            {renderInput('Aantal', 'aantal', 'Bijv. 25')}
            {renderInput('Ql', 'ql', 'Kwaliteit van product')}
            {renderInput('Plaats van verkoop', 'plaats', 'Bijv. Aalsmeer')}
            {renderInput('Stadium', 'stadium', 'Stadium')}
            {renderInput('Lengte x Gewicht', 'lengte', 'Bijv. 70 x 250g')}
            {renderInput('Kleur', 'kleur', 'Kleur')}
            {renderInput('Fustcode', 'fustcode', 'Fustcode')}
        </div>
    );
}

export default KavelInvulTabel;
