  import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
  import LandingsPagina from "./pages/LandingsPagina";
  import LoginPagina from "./pages/LoginPagina";
  import Veiling from "./pages/Veiling";
  import LocatiePagina from "./pages/Locatie.tsx";
  import KavelJudgement from "./pages/KavelJudgement.tsx";
  import RegistratiePagina from "./pages/RegistratiePagina";
  import KavelInvoer from "./pages/KavelInvoer.tsx";
  import VerkoperDashboard from "./pages/VerkoperDashboard.tsx";
  import React, { useEffect, useState } from "react";
  import Header from "./components/Header";
  import { type Me } from "./types";
  import { authFetch } from "./utils/AuthFetch"; // jouw fetch met token

  function App() {
    const [me, setMe] = useState<Me | null>(null);

    useEffect(() => {
      const fetchMe = async () => {
        try {
          const response = await authFetch("/me"); // call naar backend
          if (!response.ok) {
            setMe(null);
            return;
          }
          const data: Me = await response.json();
          setMe(data);
        } catch (error) {
          console.error("Fout bij ophalen gebruiker:", error);
          setMe(null);
        }
      };

      fetchMe();
    }, []);
    return (
      <Router>
        <Header me={me} />
        <Routes>
          <Route path="/" element={<LandingsPagina />} /> 
          <Route path="/invoer" element={<KavelInvoer />} />
          <Route path="/login" element={<LoginPagina />} />
          <Route path="/veiling" element={<Veiling />} />
          <Route path="/locaties" element={<LocatiePagina />} />
          <Route path="/judgement" element={<KavelJudgement />} />
          <Route path="/registreer" element={<RegistratiePagina />} />
          <Route path="/verkoper-dashboard" element={<VerkoperDashboard />} />
        </Routes>
      </Router>
    );
  }

  export default App;

