import React, { useState } from 'react';
import bloomifyLogo from '../assets/bloomify_naam_logo.png';
import emailIcon from '../assets/login/email.png';
import keyIcon from '../assets/login/key.png';
import { useNavigate } from 'react-router-dom';
import './LoginWidget.css'; // Gewoon de originele CSS

function Registratie() {
  const [email, setEmail] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (password !== confirmPassword) {
      setError('Wachtwoorden komen niet overeen');
      return;
    }

    if (password.length < 6) {
      setError('Wachtwoord moet minimaal 6 tekens lang zijn');
      return;
    }

    try {
      const response = await fetch('/api/gebruikers/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email,
          firstName,
          lastName,
          phoneNumber,
          password
        }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Registratie mislukt');
      }

      alert('Registratie succesvol! Je kunt nu inloggen.');
      navigate('/login');
    } catch (err) {
      console.error('Registratie fout:', err);
      setError(err instanceof Error ? err.message : 'Er is een fout opgetreden');
    }
  };

  return (
    <div className="login-container">
      <div className="login-form-wrapper" style={{ padding: '2rem 2rem' }}>
        <div className="login-form-content" style={{ gap: '1.5rem' }}>
          <div className="login-logo-section" style={{ marginBottom: '0.1rem' }}> {/* Was 1.5rem */}
            <img src={bloomifyLogo} alt="Bloomify" className="login-logo" />
          </div>

          <h2 className="text-2xl font-bold" style={{ color: '#7A1F3D', textAlign: 'center', marginTop: 0, marginBottom: '1rem' }}>
            Registreren
          </h2>

          {error && (
            <div style={{
              padding: '0.75rem',
              backgroundColor: '#fee',
              color: '#c33',
              borderRadius: '4px',
              marginBottom: '0.5rem'
            }}>
              {error}
            </div>
          )}

          <div className="login-input-wrapper">
            <input
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              placeholder="Voornaam"
              className="login-input"
              required
            />
          </div>

          <div className="login-input-wrapper">
            <input
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              placeholder="Achternaam"
              className="login-input"
              required
            />
          </div>

          <div className="login-input-wrapper">
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Email"
              className="login-input"
              required
            />
            <img src={emailIcon} alt="" className="login-input-icon" />
          </div>

          <div className="login-input-wrapper">
            <input
              type="tel"
              value={phoneNumber}
              onChange={(e) => setPhoneNumber(e.target.value)}
              placeholder="Telefoonnummer"
              className="login-input"
              required
            />
          </div>

          <div className="login-input-wrapper">
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Wachtwoord"
              className="login-input"
              required
            />
            <img src={keyIcon} alt="" className="login-input-icon" />
          </div>

          <div className="login-input-wrapper">
            <input
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="Bevestig wachtwoord"
              className="login-input"
              required
            />
            <img src={keyIcon} alt="" className="login-input-icon" />
          </div>

          <button
            type="button"
            onClick={handleSubmit}
            className="login-button"
          >
            Registreren
          </button>

          <div style={{ textAlign: 'center', marginTop: '1rem' }}>
            <span style={{ color: 'black' }}>Heb je al een account? </span>
            <a
              href="/login"
              style={{
                color: '#7A1F3D',
                textDecoration: 'underline',
                fontWeight: 600
              }}
            >
              Log in
            </a>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Registratie;