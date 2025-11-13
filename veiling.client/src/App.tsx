import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingsPagina from './components/LandingsPagina';
import LoginPagina from './components/LoginPagina';
import Veiling from './pages/Veiling';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingsPagina />} />
                <Route path="/login" element={<LoginPagina />} />
                <Route path="/veiling" element={<Veiling />} />
            </Routes>
        </Router>
    );
}

export default App;