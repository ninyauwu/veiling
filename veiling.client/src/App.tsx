import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingsPagina from './components/LandingsPagina';
import LoginPagina from './components/LoginPagina';
import Veiling from './pages/Veiling';
import VerkoperDashboard from './pages/VerkoperDashboard';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingsPagina />} />
                <Route path="/login" element={<LoginPagina />} />
                <Route path="/veiling" element={<Veiling />} />
                <Route path="/verkoper-dashboard" element={<VerkoperDashboard />} />
            </Routes>
        </Router>
    );
}

export default App;