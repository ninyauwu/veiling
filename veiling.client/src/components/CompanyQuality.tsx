interface CompanyQualityProps {
  naam: string;
  qi: string;
  kwaliteit: string;
}

export default function CompanyQuality({
  naam,
  qi,
  kwaliteit,
}: CompanyQualityProps) {
  return (
    <div className="company-info">
      <div className="company-name">{naam}</div>
      <div>QI</div>
      <div className="company-rating">{qi}</div>
      <div>Kw.</div>
      <div className="company-rating">
        <span
          style={{
            paddingRight: "12px",
          }}
        >
          {kwaliteit}
        </span>
        {Array.from({ length: getStarCount(kwaliteit) }, () => (
          <img
            src="/icons/quality_star.svg"
            alt="Ster"
            className="quality-star"
          />
        ))}
      </div>
    </div>
  );
}

function getStarCount(kwaliteit: string) {
  switch (kwaliteit) {
    case "A1":
      return 3;
    case "A2":
      return 2;
    case "B1":
      return 1;
    default:
      return 0;
  }
}
