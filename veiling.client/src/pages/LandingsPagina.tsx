import Header from "../components/HeaderLoggedout";

function LandingsPagina() {
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
              className="px-8 py-4 rounded-lg font-semibold text-lg transition"
              style={{
                backgroundColor: "white",
                color: "#7A1F3D",
                border: "2px solid white",
              }}
              onMouseEnter={(e) =>
                (e.currentTarget.style.backgroundColor = "#dddddd")
              }
              onMouseLeave={(e) =>
                (e.currentTarget.style.backgroundColor = "white")
              }
              onMouseDown={(e) => {
                // tijdelijk stijl van de verkoper knop
                e.currentTarget.style.backgroundColor = "rgba(0,0,0,0.18)";
                e.currentTarget.style.color = "white";
                e.currentTarget.style.backdropFilter = "blur(10px)";
                e.currentTarget.style.border = "2px solid white";
              }}
              onMouseUp={(e) => {
                // terug naar originele stijl
                e.currentTarget.style.backgroundColor = "white";
                e.currentTarget.style.color = "#7A1F3D";
                e.currentTarget.style.backdropFilter = "none";
                e.currentTarget.style.border = "2px solid white";
              }}
            >
              Ik ben een koper
            </button>

            <button
              className="text-white px-8 py-4 rounded-lg font-semibold text-lg transition border-2 border-white"
              style={{
                backgroundColor: "transparent",
                backdropFilter: "blur(10px)",
              }}
              onMouseEnter={(e) =>
                (e.currentTarget.style.backgroundColor = "rgba(0,0,0,0.18)")
              }
              onMouseLeave={(e) =>
                (e.currentTarget.style.backgroundColor = "transparent")
              }
              onMouseDown={(e) => {
                // tijdelijk stijl van de koper knop
                e.currentTarget.style.backgroundColor = "white";
                e.currentTarget.style.color = "#7A1F3D";
                e.currentTarget.style.backdropFilter = "none";
                e.currentTarget.style.border = "2px solid white";
              }}
              onMouseUp={(e) => {
                // terug naar originele stijl
                e.currentTarget.style.backgroundColor = "transparent";
                e.currentTarget.style.color = "white";
                e.currentTarget.style.backdropFilter = "blur(10px)";
                e.currentTarget.style.border = "2px solid white";
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

