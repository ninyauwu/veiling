import { useNavigate } from "react-router-dom";
import Header from "../components/HeaderLoggedout";

function LandingsPagina() {
  const navigate = useNavigate();

  const checkLoginAndNavigate = async (targetIfLoggedIn: string) => {
    console.log("checkLoginAndNavigate called with:", targetIfLoggedIn);
    const token = localStorage.getItem("access_token");
    console.log("Token found:", token ? "yes" : "no");

    if (!token) {
      console.log("No token, navigating to /login");
      navigate("/login");
      return;
    }

    try {
      console.log("Calling /me endpoint");
      const meResponse = await fetch("/me", {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });

      console.log("Response status:", meResponse.status);

      if (!meResponse.ok) {
        console.log("Response not OK, clearing tokens");
        localStorage.removeItem("access_token");
        localStorage.removeItem("refresh_token");
        navigate("/login");
        return;
      }

      console.log("Login valid, navigating to:", targetIfLoggedIn);
      navigate(targetIfLoggedIn);
    } catch (error) {
      console.error("Error checking login status:", error);
      navigate("/login");
    }
  };

  const handleKoperClick = () => {
    console.log("Koper button clicked");
    checkLoginAndNavigate("/locaties");
  };

  const handleVerkoperClick = () => {
    console.log("Verkoper button clicked");
    checkLoginAndNavigate("/verkoper-dashboard");
  };

  return (
    <div className="bg-white w-full overflow-x-hidden">
      <Header />

      <div
        className="w-screen bg-cover bg-center bg-no-repeat flex items-center justify-center"
        style={{
          backgroundImage: "url(/background.jpg)",
          minHeight: "100vh",
          backgroundSize: "cover",
          backgroundPosition: "center",
          position: "relative",
        }}
      >
        <div
          style={{
            position: "absolute",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: "rgba(0, 0, 0, 0.3)",
            zIndex: 1,
          }}
        />

        <div
          style={{
            position: "relative",
            zIndex: 10,
            textAlign: "center",
            padding: "0 1rem",
          }}
        >
          <h1
            className="text-5xl md:text-7xl font-bold text-white mb-12 max-w-5xl leading-tight mx-auto"
            style={{ textShadow: "2px 2px 8px rgba(0,0,0,0.5)" }}
          >
            Het veilingplatform voor de moderne sierteelthandelaar
          </h1>

          <div className="flex gap-6 justify-center">
            <button
              onClick={handleKoperClick}
              className="px-8 py-4 rounded-lg font-semibold text-lg transition hover:bg-gray-100 active:bg-gray-200"
              style={{
                backgroundColor: "white",
                color: "#7A1F3D",
                border: "2px solid white",
              }}
            >
              Ik ben een koper
            </button>

            <button
              onClick={handleVerkoperClick}
              className="text-white px-8 py-4 rounded-lg font-semibold text-lg transition border-2 border-white hover:bg-white hover:bg-opacity-20 active:bg-white active:text-primary"
              style={{
                backgroundColor: "transparent",
                backdropFilter: "blur(10px)",
              }}
            >
              Ik ben een verkoper
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default LandingsPagina;