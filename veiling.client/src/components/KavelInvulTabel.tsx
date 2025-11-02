import { useState } from 'react';
import './KavelInvulTabel.css';

function KavelInvulTabel() {
    const [formData, setFormData] = useState({
        naam: '',
        prijs: '',
        minimum: '',
        aantal: '',
        ql: '',
        qualiteit: '',
        plaats: '',
        plaatsVerkoop: '',
        stadium: '',
        stadiumValue: '',
        lengte: '',
        lengteGewicht: '',
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
        <div className="form-table-container">
            <div className="form-table-header">Invullen</div>

            <div className="form-table-row">
                <div className="form-table-label">Naam</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Naam"
                    value={formData.naam}
                    onChange={(e) => handleChange('naam', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Prijs</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Minimum"
                    value={formData.prijs}
                    onChange={(e) => handleChange('prijs', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Aantal</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Aantal"
                    value={formData.aantal}
                    onChange={(e) => handleChange('aantal', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Ql</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Qualiteit van product"
                    value={formData.ql}
                    onChange={(e) => handleChange('ql', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Plaats van verkoop</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Plaats van verkoop"
                    value={formData.plaats}
                    onChange={(e) => handleChange('plaats', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Stadium</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Stadium"
                    value={formData.stadium}
                    onChange={(e) => handleChange('stadium', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Lengte x Gewicht</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Lengte x Gewicht"
                    value={formData.lengte}
                    onChange={(e) => handleChange('lengte', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Kleur</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Kleur"
                    value={formData.kleur}
                    onChange={(e) => handleChange('kleur', e.target.value)}
                />
            </div>

            <div className="form-table-row">
                <div className="form-table-label">Fustcode</div>
                <input
                    type="text"
                    className="form-table-input"
                    placeholder="Fustcode"
                    value={formData.fustcode}
                    onChange={(e) => handleChange('fustcode', e.target.value)}
                />
            </div>
        </div>
    );
}

export default KavelInvulTabel;