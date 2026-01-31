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
import ProtectedRoute from "./utils/ProtectedRoute.tsx";


function App() {
  return (
    <Router>
  <Routes>
    <Route path="/" element={<Layout />}>
      
      <Route index element={<LandingsPagina />} />
      <Route path="login" element={<LoginPagina />} />
      <Route path="registreer" element={<RegistratiePagina />} />

      <Route
        path="invoer"
        element={
          <ProtectedRoute>
            <KavelInvoer />
          </ProtectedRoute>
        }
      />

      <Route
        path="veiling/:locatieId"
        element={
          <ProtectedRoute>
            <Veiling />
          </ProtectedRoute>
        }
      />

      <Route
        path="locaties"
        element={
          <ProtectedRoute>
            <LocatiePagina />
          </ProtectedRoute>
        }
      />

      <Route
        path="judgement"
        element={
          <ProtectedRoute>
            <KavelJudgement />
          </ProtectedRoute>
        }
      />

      <Route
        path="verkoper-dashboard"
        element={
          <ProtectedRoute>
            <VerkoperDashboard />
          </ProtectedRoute>
        }
      />

      <Route
        path="scheduler"
        element={
          <ProtectedRoute>
            <Scheduler />
          </ProtectedRoute>
        }
      />

      <Route
        path="veilingmeester"
        element={
          <ProtectedRoute>
            <VeilingMeesterKeuzePagina />
          </ProtectedRoute>
        }
      />

    </Route>
  </Routes>
</Router>
  );
}
export default App;
