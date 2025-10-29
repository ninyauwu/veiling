import React, { useState} from 'react';
import bloomifyLogo from '../assets/bloomify_naam_logo.png';
import emailIcon from '../assets/login/email.png';
import keyIcon from '../assets/login/key.png';

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        console.log('Login submitted:', { email, password, rememberMe });
    };

    return (
        <div className="min-h-screen bg-gray-300 flex items-center justify-center p-4">
            <div className="w-full max-w-md">
                <div className="space-y-8">
                    {/* Logo/Title Section */}
                        <div className="text-center mb-12">
                            <img src={bloomifyLogo} alt="Bloomify" className="mx-auto" />
                        </div>

                        {/* Email Input */}
                        <div className="relative">
                            <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="Email"
                            className="w-full bg-transparent border-b-2 border-black pb-2 text-lg placeholder-black focus:outline-none focus:border-purple-900"
                            required
                            />
                            <img src={emailIcon} alt="" className="absolute right-0 top-0 w-6 h-6" />
                        </div>

                        {/* Password Input */}
                        <div className="relative">
                            <input
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                placeholder="Password"
                                className="w-full bg-transparent border-b-2 border-black pb-2 text-lg placeholder-black focus:outline-none focus:border-purple-900"
                                required
                            />
                            <img src={keyIcon} alt="" className="absolute right-0 top-0 w-6 h-6" />
                        </div>

                        {/* Remember Me and Forgot Password */}
                        <div className="flex items-center justify-between text-sm">
                            <label className="flex items-center cursor-pointer">
                                <input
                                    type="checkbox"
                                    checked={rememberMe}
                                    onChange={(e) => setRememberMe(e.target.checked)}
                                    className="w-5 h-5 mr-2 cursor-pointer accent-black"
                                />
                                <span className="font-semibold">Remember me</span>
                            </label>
                            <a href="#" className="italic underline font-medium">
                                Forgot password?
                            </a>
                        </div>

            {/* Login Button */}
                    <button
                        type="button"
                        onClick={handleSubmit}
                        className="w-full bg-purple-900 text-white py-4 text-2xl font-semibold hover:bg-purple-800 transition-colors"
                        >
                        Login
                    </button>
                </div>
            </div>
        </div>
    );
}

export default Login;