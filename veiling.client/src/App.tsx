import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LandingsPagina from './components/LandingsPagina';
import LoginPagina from './components/LoginPagina';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LandingsPagina />} />
                <Route path="/login" element={<LoginPagina />} />
            </Routes>
        </Router>
    );
}

export default App;