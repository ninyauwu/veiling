import "../components/KavelInfo.css";
import KavelInfo from "../components/KavelInfo";
import HeaderLoggedout from "../components/HeaderLoggedout";
import { useParams } from "react-router-dom";

function Veiling() {
  const { locatieId } = useParams<{ locatieId?: string }>();

  if (!locatieId) {
    return <div>Locatie niet gevonden</div>;
  }

  const locatie = parseInt(locatieId, 10);

  return (
    <div>
      <HeaderLoggedout />
      <div
        style={{
          height: "96px",
        }}
      />
      <KavelInfo locatieId={locatie} />
    </div>
  );
}

export default Veiling;
