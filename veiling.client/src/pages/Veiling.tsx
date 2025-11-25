import "../components/KavelInfo.css";
import KavelInfo from "../components/KavelInfo";
import HeaderLoggedout from "../components/HeaderLoggedout";
import AuctionCountdown from "../components/AuctionCountdown";

function Veiling() {
  return (
    <div>
      <HeaderLoggedout />
      <div
        style={{
          height: "96px",
        }}
      />
      <div
        style={{
          display: "flex",
          flexDirection: "row",
          gap: "60px",
        }}
      >
        <KavelInfo />
        <AuctionCountdown />
      </div>
    </div>
  );
}

export default Veiling;
