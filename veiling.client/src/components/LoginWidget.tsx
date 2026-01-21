import React, { useState} from 'react';
import bloomifyLogo from '../assets/bloomify_naam_logo.png';
import emailIcon from '../assets/login/email.png';
import keyIcon from '../assets/login/key.png';
import { useNavigate } from "react-router-dom";
import './LoginWidget.css';

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
            const response = await fetch("/login?useCookies=false&useSessionCookies=false", {

            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                email,
                password
            })
        });

        if (!response.ok) {
            alert("Invalid email or password");
            return;
        }

        const data = await response.json();
        console.log("Login success:", data);

        // Store token
        localStorage.setItem("access_token", data.accessToken);

        const meResponse = await fetch("/me", {
        headers: {
            Authorization: `Bearer ${data.accessToken}`
        }
        });

        if (!meResponse.ok) {
        throw new Error("Failed to fetch user info");
        }

        const me = await meResponse.json();
        console.log("Current user:", me);

        // Optional: refresh token
        if (data.refreshToken) {
            localStorage.setItem("refresh_token", data.refreshToken);
        }

        if (me.roles.includes("Leverancier")) {
            window.location.href = "/verkoper-dashboard";
        } else {
            window.location.href = "/locaties";
        }



    } catch (error) {
        console.error("Login error:", error);
    }
};

    return (
        <div className="login-container">
            <div className="login-form-wrapper">
                <div className="login-form-content">
                    {/* Logo/Title Section */}
                    <div className="login-logo-section">
                        <img src={bloomifyLogo} alt="Bloomify" className="login-logo" />
                    </div>

                    {/* Email Input */}
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

                    {/* Password Input */}
                    <div className="login-input-wrapper">
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            placeholder="Password"
                            className="login-input"
                            required
                        />
                        <img src={keyIcon} alt="" className="login-input-icon" />
                    </div>

                    {/* Remember Me and Forgot Password */}
                    <div className="login-options">
                        <label className="login-remember-label">
                            <input
                                type="checkbox"
                                checked={rememberMe}
                                onChange={(e) => setRememberMe(e.target.checked)}
                                className="login-checkbox"
                            />
                            <span className="login-remember-text">Remember me</span>
                        </label>
                        <a href="#" className="login-forgot-link">
                            Forgot password?
                        </a>
                    </div>

                    {/* Login Button */}
                    <button
                        type="button"
                        onClick={handleSubmit}
                        className="login-button"
                    >
                        Login
                    </button>
                </div>
            </div>
        </div >
    );
}

export default Login;