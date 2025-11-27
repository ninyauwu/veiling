import "../components/KavelInfo.css";
import KavelInfo from "../components/KavelInfo";
import HeaderLoggedout from "../components/HeaderLoggedout";

function Veiling() {
  return (
    <div>
      <HeaderLoggedout />
      <div
        style={{
          height: "96px",
        }}
      />
      <KavelInfo />
    </div>
  );
}

export default Veiling;
