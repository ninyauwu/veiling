import { useState, useEffect} from "react";
import KavelTabel from "./KavelTabel";
import "./KavelInfo.css";
import Spacer from "./Spacer";
import ImageSet from "./ImageSet";
import NavigationBar from "./NavigationBar";
import MetadataGrid from "./MetadataGrid";
import CompanyQuality from "./CompanyQuality";

// Define the type for data coming from your API
type KavelInfoResponse = {
  kavel: {
    id: number;
    naam: string;
    beschrijving: string;
    stageOfMaturity: string;
    minimumPrijs: number;
    kavelkleur: string;
    keurcode: string;
    fustcode: number;
    approval: boolean | null | undefined;
  };
  leverancier: {
    indexOfReliabilityOfInformation: string;
    bedrijf: {
      bedrijfsnaam: string;
    };
  };
};

function KavelInfo({ sortOnApproval = false, onKavelFetched }: KavelInfoProps) {
  const imagePaths = [
    "https://picsum.photos/400/400?random=1",
    "https://picsum.photos/400/400?random=2",
    "https://picsum.photos/400/400?random=3",
  ];

  const [kavels, setKavels] = useState<KavelInfoResponse[]>([]);
  const [selected, setSelected] = useState<number | null>(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchKavels() {
      try {
        if (sortOnApproval) {
          const res = await fetch("/api/kavels?lookForPending=true")
          const data: KavelInfoResponse[] = await res.json();
          console.log('API Response:', data);
          setKavels(data);
          setLoading(false);
          if (data.length > 0) setSelected(0);
          if (onKavelFetched) {
            onKavelFetched(data[0].kavel.id);
          }
        } else {
          const res = await fetch("/api/KavelInfo/0");
          const data: KavelInfoResponse[] = await res.json();
          setKavels(data);
          setLoading(false);
          if (data.length > 0) setSelected(0);
        }
      } catch (err) {
        console.error("Failed to load kavels:", err);
        setLoading(false);
      }
    } 
    fetchKavels();
  }, []);

  useEffect(() => {
  if (onKavelFetched && selected !== null && kavels.length > 0) {
    onKavelFetched(kavels[selected].kavel.id);
  }
}, [selected, kavels, onKavelFetched]); // Runs whenever selected changes

  const handleNext = () => {
    if (selected === null) return;
    setSelected((prev) =>
      prev! + 1 >= kavels.length ? kavels.length - 1 : prev! + 1,
    );
  };

  const handlePrevious = () => {
    if (selected === null) return;
    setSelected((prev) => (prev! - 1 < 0 ? 0 : prev! - 1));
  };

  if (loading) return <div>Loading...</div>;
  if (kavels.length === 0) return <div>No kavels found.</div>;

  const currentKavel = selected !== null ? kavels[selected] : kavels[0];
  const { kavel, leverancier } = currentKavel;

  const tableRows = formatKavelData(kavels);

  return (
    <div className="flex-column">
      <h1 className="hidden">Veilingpagina</h1>
      <h2 className="hidden">Kaveltabel</h2>

      <KavelTabel
        rows={tableRows}
        selectedRowIndex={selected}
        onSelectedRowChange={setSelected}
        onRowSelect={(row, index) => {
          setSelected(index);
          console.log(row);
        }}
      />

      <Spacer color="#00000000" />
      <NavigationBar
        onPrevious={handlePrevious}
        onNext={handleNext}
        getSelectedItemString={() => {
          if (selected === null) return "Geen naam";
          return tableRows[selected].Naam;
        }}
      />
      <Spacer />

      <span>
        <div className="flex-row-justify">
          <h2>{kavel.naam}</h2>
          <CompanyQuality
            naam={leverancier.bedrijf.bedrijfsnaam}
            qi={leverancier.indexOfReliabilityOfInformation}
            kwaliteit={kavel.keurcode}
          />
        </div>

        <p>{kavel.beschrijving}</p>
      </span>

      <ImageSet images={imagePaths} />
      <MetadataGrid
        items={[
          { key: "Stadium", value: kavel.stageOfMaturity },
          { key: "Fustcode", value: kavel.fustcode },
          {
            key: "Kleur",
            value: (
              <div
                style={{
                  backgroundColor: "#" + kavel.kavelkleur,
                  width: "24px",
                  height: "24px",
                }}
              />
            ),
          },
        ]}
      />

      <Spacer />
      <NavigationBar
        onPrevious={handlePrevious}
        onNext={handleNext}
        getSelectedItemString={() => {
          if (selected === null) return "Geen naam";
          return tableRows[selected].Naam;
        }}
      />
    </div>
  );
}

// In a separate utils file
export const formatKavelData = (kavels: KavelInfoResponse[]) => {
  return kavels.map((kavel) => ({
    Naam: kavel?.kavel?.naam ?? "NA",
    "Max Prijs": `€${kavel?.kavel?.minimumPrijs?.toLocaleString() ?? "N/A"}`,
    Leverancier: kavel?.leverancier?.bedrijf?.bedrijfsnaam ?? "N/A",
    QI: kavel?.leverancier?.indexOfReliabilityOfInformation ?? "N/A",
    Kwaliteit: kavel?.kavel?.keurcode ?? "N/A",
  }));
};

interface KavelInfoProps {
  sortOnApproval?: boolean;
  onKavelFetched?: (kavelId: number) => void;
}

export default KavelInfo;
