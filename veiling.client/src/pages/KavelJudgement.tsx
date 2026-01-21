import "../components/KavelInfo.css";
import KavelInfo from "../components/KavelInfo";

function KavelJudgement() {
  return (
    <div>
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
        <KavelInfo sortOnApproval={true} />
      </div>
    </div>
  );
}

export default KavelJudgement;
