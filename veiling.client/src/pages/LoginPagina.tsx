import Header from "../components/HeaderLoggedout";
import Login from "../components/LoginWidget";

function LoginPagina() {
    return (
        <div className="bg-white w-full overflow-x-hidden">
            <Header />

            <div
                className="w-screen bg-cover bg-center bg-no-repeat flex flex-col items-center justify-center text-center relative"
                style={{
                    backgroundImage: "url(/background.jpg)",
                    minHeight: "100vh",
                    backgroundSize: "cover",
                    backgroundPosition: "center",
                    position: "relative",
                }}
            >
                {}
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

                {}
                <div
                    style={{
                        position: "relative",
                        zIndex: 10,
                        textAlign: "center",
                        padding: "0 1rem",
                        color: "white",
                    }}
                >

                    {/* Login widget */}
                    <Login />
                </div>
            </div>
        </div>
    );
}

export default LoginPagina;
