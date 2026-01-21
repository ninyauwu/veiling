import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LandingsPagina from "./pages/LandingsPagina";
import LoginPagina from "./pages/LoginPagina";
import Veiling from "./pages/Veiling";
import LocatiePagina from "./pages/Locatie.tsx";
import KavelJudgement from "./pages/KavelJudgement.tsx";
import RegistratiePagina from "./pages/RegistratiePagina";
import KavelInvoer from "./pages/KavelInvoer.tsx";
import VerkoperDashboard from "./pages/VerkoperDashboard.tsx";
import Scheduler from "./pages/SchedulerPagina.tsx";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LandingsPagina />} /> 
        <Route path="/invoer" element={<KavelInvoer />} />
        <Route path="/login" element={<LoginPagina />} />
        <Route path="/veiling/:locatieId" element={<Veiling />} />
        <Route path="/locaties" element={<LocatiePagina />} />
        <Route path="/judgement" element={<KavelJudgement />} />
        <Route path="/registreer" element={<RegistratiePagina />} />
        <Route path="/verkoper-dashboard" element={<VerkoperDashboard />} />
        <Route path="/scheduler" element={<Scheduler />} />
      </Routes>
    </Router>
  );
}

export default App;
