import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingsPagina from './components/LandingsPagina';
import LoginPagina from './components/LoginPagina';
import Veiling from './pages/Veiling';
import LocatiePagina from './pages/Locatie.tsx';


function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingsPagina />} />
                <Route path="/login" element={<LoginPagina />} />
                <Route path="/veiling" element={<Veiling />} />
                <Route path="/locaties" element={<LocatiePagina />} />
            </Routes>
        </Router>
    );
}

export default App;