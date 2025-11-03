import { useState } from 'react';
import './KavelInvulTabel.css';

function KavelInvulTabel() {
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

    const handleChange = (field: string, value: string) => {
        setFormData(prev => ({
            ...prev,
            [field]: value
        }));
    };

    return (
        <div className="kavel-invul-container">
            <div className="kavel-invul-header">Invullen</div>
            
            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Naam</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Naam"
                    value={formData.naam}
                    onChange={(e) => handleChange('naam', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Prijs</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Minimum"
                    value={formData.prijs}
                    onChange={(e) => handleChange('prijs', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Aantal</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Aantal"
                    value={formData.aantal}
                    onChange={(e) => handleChange('aantal', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Ql</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Qualiteit van product"
                    value={formData.ql}
                    onChange={(e) => handleChange('ql', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Plaats van verkoop</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Plaats van verkoop"
                    value={formData.plaats}
                    onChange={(e) => handleChange('plaats', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Stadium</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Stadium"
                    value={formData.stadium}
                    onChange={(e) => handleChange('stadium', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Lengte x Gewicht</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Lengte x Gewicht"
                    value={formData.lengte}
                    onChange={(e) => handleChange('lengte', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Kleur</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Kleur"
                    value={formData.kleur}
                    onChange={(e) => handleChange('kleur', e.target.value)}
                />
            </div>

            <div className="kavel-invul-row">
                <div className="kavel-invul-label">Fustcode</div>
                <input
                    type="text"
                    className="kavel-invul-input"
                    placeholder="Fustcode"
                    value={formData.fustcode}
                    onChange={(e) => handleChange('fustcode', e.target.value)}
                />
            </div>
        </div>
    );
}

export default KavelInvulTabel;