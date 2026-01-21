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
import Layout from "./layout/Layout.tsx";
import VeilingMeesterKeuzePagina from "./pages/VeilingMeesterKeuzePagina";


function App() {
  return (
    <Router>
      <Routes>
        <Route element={<Layout />}>
        <Route path="/" element={<LandingsPagina />} /> 
        <Route path="/invoer" element={<KavelInvoer />} />
        <Route path="/login" element={<LoginPagina />} />
        <Route path="/veiling/:locatieId" element={<Veiling />} />
        <Route path="/locaties" element={<LocatiePagina />} />
        <Route path="/judgement" element={<KavelJudgement />} />
        <Route path="/registreer" element={<RegistratiePagina />} />
        <Route path="/verkoper-dashboard" element={<VerkoperDashboard />} />
        <Route path="/scheduler" element={<Scheduler />} />
        </Route>
        <Route path="/veilingmeester" element={<VeilingMeesterKeuzePagina />} />
      </Routes>
    </Router>
  );
}

export default App;
