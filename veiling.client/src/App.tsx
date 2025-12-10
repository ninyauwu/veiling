import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LandingsPagina from "./pages/LandingsPagina";
import LoginPagina from "./pages/LoginPagina";
import Veiling from "./pages/Veiling";
import LocatiePagina from "./pages/Locatie.tsx";
import KavelJudgement from "./pages/KavelJudgement.tsx";
import VerkoperDashboard from "./pages/VerkoperDashboard.tsx";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LandingsPagina />} />
        <Route path="/login" element={<LoginPagina />} />
        <Route path="/veiling" element={<Veiling />} />
        <Route path="/locaties" element={<LocatiePagina />} />
        <Route path="/judgement" element={<KavelJudgement />} />
        <Route path="/verkoper-dashboard" element={<VerkoperDashboard />} />
      </Routes>
    </Router>
  );
}

export default App;

