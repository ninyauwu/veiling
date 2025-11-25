import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingsPagina from './pages/LandingsPagina';
import LoginPagina from './pages/LoginPagina';
import Veiling from './pages/Veiling';
import VerkoperDashboard from './pages/VerkoperDashboard';
import LocatiePagina from './pages/Locatie.tsx';


function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingsPagina />} />
                <Route path="/login" element={<LoginPagina />} />
                <Route path="/veiling" element={<Veiling />} />
                <Route path="/verkoper-dashboard" element={<VerkoperDashboard />} />
                <Route path="/locaties" element={<LocatiePagina />} />
            </Routes>
        </Router>
    );
}

export default App;