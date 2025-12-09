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
    fustcode: '',
    aantalPerContainer: '',
    gewicht: '',
  });

  const [errors, setErrors] = useState<{ [key: string]: string }>({});
  
  const locations = [
    { id: '1', name: 'Amsterdam' },
    { id: '2', name: 'Rotterdam' },
    { id: '3', name: 'Delft' }
  ];

  const isWholeNumber = (value: string) => /^[0-9]+$/.test(value);
  const isDecimal = (value: string) => /^\d+(\.\d{1,2})?$/.test(value);
  const isHexColor = (value: string) =>
  /^#([A-Fa-f0-9]{3}|[A-Fa-f0-9]{4}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})$/.test(value);

  const validateField = (field: string, value: string): string => {
    if (!value.trim()) return 'Dit veld is verplicht.';

    if (['aantal', 'aantalPerContainer', 'fustcode'].includes(field) && !isWholeNumber(value)) {
      return 'Geheel getal.';
    }

    if (['prijs', 'gewicht', 'lengte'].includes(field) && !isDecimal(value)) {
      return 'Geldig decimaal (bijv. 12.50).';
    }

    if (field === 'kleur' && !isHexColor(value)) {
      return 'Ongeldige kleur — gebruik een hexcode zoals #7A1F3D.';
    }
    
    return '';
  };

  const validateForm = (data: typeof formData) => {
    return Object.values(data).every(v => v.trim() !== '') &&
      isWholeNumber(data.aantal) &&
      isDecimal(data.prijs) &&
      isWholeNumber(data.aantalPerContainer) &&
      isDecimal(data.gewicht) &&
      isDecimal(data.lengte) &&
      isWholeNumber(data.fustcode);
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

  const renderDropdown = (label: string, field: keyof typeof formData) => (
    <div className="kavel-invul-row">
      <div className="kavel-invul-label">{label}</div>
      <select
        className={`kavel-invul-input ${errors[field] ? 'input-error' : ''}`}
        value={formData[field]}
        onChange={(e) => handleInputChange(field, e.target.value)}
      >
        <option value="">Selecteer locatie</option>
        {locations.map(loc => (
          <option key={loc.id} value={loc.id}>
            {loc.name}
          </option>
        ))}
      </select>
      {errors[field] && <div className="error-text">{errors[field]}</div>}
    </div>
  );

  return (
    <div className="kavel-invul-container">
      <div className="kavel-invul-header">Invullen</div>

      {renderInput('Naam', 'naam', 'Naam')}
      {renderInput('Prijs (€)', 'prijs', 'Bijv. 12.50')}
      {renderInput('Aantal containers', 'aantal', 'Bijv. 25')}
      {renderInput('Ql', 'ql', 'Kwaliteit van product')}
      {renderDropdown('Plaats van verkoop', 'plaats')}
      {renderInput('Stadium', 'stadium', 'Stadium')}
      {renderInput('Kleur', 'kleur', 'Kleur')}
      {renderInput('Fustcode', 'fustcode', 'Fustcode')}
      {renderInput('Producten per container', 'aantalPerContainer', 'Aantal producten per container')}
      {renderInput('Lengte Van Bloem', 'lengte', 'Bijv. 50, avg lengte per bloem in cm')}
      {renderInput('Gewicht Van Bloem', 'gewicht', 'Bijv. 25, avg gewicht per bloem in g')}
    </div>
  );
}

export default KavelInvulTabel;