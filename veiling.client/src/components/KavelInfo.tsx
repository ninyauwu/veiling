import { useState, useEffect } from "react";
import KavelTabel from "./KavelTabel";
import "./KavelInfo.css";
import Spacer from "./Spacer";
import ImageSet from "./ImageSet";
import NavigationBar from "./NavigationBar";
import MetadataGrid from "./MetadataGrid";
import CompanyQuality from "./CompanyQuality";
import AuctionCountdown from "./AuctionCountdown";
import ApproveOrDeny from "./AproveOrDenyTextBox";

// Define the type for data coming from your API
type KavelInfoResponse = {
  kavel: {
    id: number;
    naam: string;
    beschrijving: string;
    stageOfMaturity: string;
    minimumPrijs: number;
    maximumPrijs: number;
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

interface KavelInfoProps {
  sortOnApproval?: boolean;
  onSelectKavel?: (kavel: number) => void;
}

function KavelInfo({ sortOnApproval, onSelectKavel }: KavelInfoProps) {
  const imagePaths = [
    "https://picsum.photos/400/400?random=1",
    "https://picsum.photos/400/400?random=2",
    "https://picsum.photos/400/400?random=3",
  ];

  const [kavels, setKavels] = useState<KavelInfoResponse[]>([]);
  const [selected, setSelected] = useState<number | null>(0);
  const [loading, setLoading] = useState(true);
  const [reload, setReload] = useState(0);

  useEffect(() => {
    async function fetchKavels() {
      try {
        setLoading(true); // start loading
        let res: Response;
        if (sortOnApproval) {
          res = await fetch("/api/KavelInfo/pending");
        } else {
          res = await fetch("/api/KavelInfo/0");
        }

        if (!res.ok) {
          // Handle non-2xx responses
          if (res.status === 404) {
            console.log("No kavels found");
            setKavels([]); // explicitly empty array
          } else {
            throw new Error(`HTTP error! status: ${res.status}`);
          }
        } else {
          const data: KavelInfoResponse[] = await res.json();
          setKavels(data);
          if (data.length > 0) setSelected(0);
        }
      } catch (err) {
        console.error("Failed to load kavels:", err);
        setKavels([]); // ensure kavels is empty on error
      } finally {
        setLoading(false);
      }
    }

    fetchKavels();
  }, [sortOnApproval, reload]);

    // Notify parent whenever selection changes
  useEffect(() => {
    if (onSelectKavel && kavels.length > 0 && selected !== null) {
      onSelectKavel(kavels[selected].kavel.id)
    }
  }, [selected, kavels, onSelectKavel]);

  if (kavels.length < 1) {
    return <div>Geen kavels gevonden</div>;
  }

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

  const bonusWidget = sortOnApproval ? (
    <ApproveOrDeny
      currentKavelId={kavel.id}
      onApprovalResponse={() => {
        console.log("Icky shticky");
        if (selected !== null) {
          if (selected == kavels.length) {
            setSelected(selected - 1);
          }
        }
        setReload(reload + 1);
      }}
    />
  ) : (
    <AuctionCountdown price={kavel.maximumPrijs} />
  );

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "row",
        gap: "60px",
      }}
    >
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
      {bonusWidget}
    </div>
  );
}

export const formatKavelData = (kavels: KavelInfoResponse[]) => {
  return kavels.map((kavel) => ({
    Naam: kavel?.kavel?.naam ?? "NA",
    "Max Prijs": `€${kavel?.kavel?.minimumPrijs?.toLocaleString() ?? "N/A"}`,
    Leverancier: kavel?.leverancier?.bedrijf?.bedrijfsnaam ?? "N/A",
    QI: kavel?.leverancier?.indexOfReliabilityOfInformation ?? "N/A",
    Kwaliteit: kavel?.kavel?.keurcode ?? "N/A",
  }));
};



export default KavelInfo;
