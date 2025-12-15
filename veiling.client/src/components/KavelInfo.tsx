import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import KavelTabel from "./KavelTabel";
import "./KavelInfo.css";
import Spacer from "./Spacer";
import ImageSet from "./ImageSet";
import NavigationBar from "./NavigationBar";
import MetadataGrid from "./MetadataGrid";
import CompanyQuality from "./CompanyQuality";
import AuctionCountdown from "./AuctionCountdown";
import ApproveOrDeny from "./AproveOrDenyTextBox";
import type { VeilingStartMessage } from "./PriceBar";

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
  locatieId?: number;
  sortOnApproval?: boolean;
}

function KavelInfo({ locatieId = 1, sortOnApproval = false }: KavelInfoProps) {
  const imagePaths = [
    "https://picsum.photos/400/400?random=1",
    "https://picsum.photos/400/400?random=2",
    "https://picsum.photos/400/400?random=3",
  ];

  const [kavels, setKavels] = useState<KavelInfoResponse[]>([]);
  const [selected, setSelected] = useState<number | null>(0);
  const [loading, setLoading] = useState(true);
  const [reload, setReload] = useState(0);
  const [startMessage, setStartMessage] = useState<VeilingStartMessage | null>(
    null,
  );
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    async function fetchKavels() {
      try {
        setLoading(true);
        let res: Response;
        if (sortOnApproval) {
          res = await fetch("/api/KavelInfo/pending");
        } else {
          res = await fetch(`/api/KavelInfo/${locatieId}`);
        }

        if (!res.ok) {
          if (res.status === 404) {
            console.log("No kavels found");
            setKavels([]);
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
        setKavels([]);
      } finally {
        setLoading(false);
      }
    }

    fetchKavels();
  }, [sortOnApproval, reload, locatieId]);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:32274/hubs/veiling")
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    connection.on(
      "VeilingStart",
      (
        startingPrice: number,
        minimumPrice: number,
        durationMs: number,
        startTime: string,
      ) => {
        setStartMessage({
          startingPrice,
          minimumPrice,
          durationMs,
          startTime: new Date(startTime),
        });

        console.log(
          "SignalR message received for " + new Date(startTime).toString(),
        );
      },
    );

    connection.start().catch((err) => console.error("SignalR error:", err));

    return () => {
      connection.stop();
    };
  }, []);

  if (loading) return <div>Loading...</div>;
  if (kavels.length === 0) return <div>Geen kavels gevonden</div>;

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

  const currentKavel = selected !== null ? kavels[selected] : kavels[0];
  const { kavel, leverancier } = currentKavel;

  const tableRows = formatKavelData(kavels);

  const bonusWidget = sortOnApproval ? (
    <ApproveOrDeny
      currentKavelId={kavel.id}
      onApprovalResponse={() => {
        console.log("Icky shticky");
        if (selected) {
          if (selected === kavels.length) {
            setSelected(selected - 1);
          }
        }
        setReload(reload + 1);
      }}
    />
  ) : (
    <AuctionCountdown
      price={kavel.maximumPrijs}
      startMessage={startMessage}
      connection={connectionRef.current}
    />
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

        <span>
          <div className="flex-row-justify">
            <h2>{kavel.naam}</h2>
            <CompanyQuality
              naam={leverancier.bedrijf.bedrijfsnaam}
              qi={leverancier.indexOfReliabilityOfInformation}
              kwaliteit={kavel.keurcode}
            />
          </div>
          <Spacer />
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
          <p>{kavel.beschrijving}</p>
        </span>

        <ImageSet images={imagePaths} />
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

const formatKavelData = (kavels: KavelInfoResponse[]) => {
  return kavels.map((kavel) => ({
    Naam: kavel?.kavel?.naam ?? "NA",
    "Max Prijs": `€${kavel?.kavel?.minimumPrijs?.toLocaleString() ?? "N/A"}`,
    Leverancier: kavel?.leverancier?.bedrijf?.bedrijfsnaam ?? "N/A",
    QI: kavel?.leverancier?.indexOfReliabilityOfInformation ?? "N/A",
    Kwaliteit: kavel?.kavel?.keurcode ?? "N/A",
  }));
};

export default KavelInfo;
