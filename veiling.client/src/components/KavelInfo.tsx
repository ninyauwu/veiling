import { useState, useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";
import KavelTabel from "./KavelTabel";
import "./KavelInfo.css";
import Spacer from "./Spacer";
import ImageSet from "./ImageSet";
import NavigationBar from "./NavigationBar";
import MetadataGrid from "./MetadataGrid";
import CompanyQuality from "./CompanyQuality";
import { authFetch } from "../utils/AuthFetch";
import AuctionCountdown from "./AuctionCountdown";
import ApproveOrDeny from "./AproveOrDenyTextBox";
import type { VeilingStartMessage } from "./PriceBar";
import { authCheckFetch } from "../utils/AuthCheckFetch";

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
    foto: string;
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
  onSelectKavel?: (kavel: number) => void;
}

interface MeResponse {
  email: string;
  isEmailConfirmed: boolean;
  roles: string[];
  id: string;
}

function KavelInfo({
                     locatieId = 1,
                     sortOnApproval = false,
                     onSelectKavel,
                   }: KavelInfoProps) {
  const [kavels, setKavels] = useState<KavelInfoResponse[]>([]);
  const [selected, setSelected] = useState<number | null>(0);
  const [loading, setLoading] = useState(true);
  const [reload, setReload] = useState(0);
  const [startMessage, setStartMessage] = useState<VeilingStartMessage | null>(
    null,
  );
  const [auctionKey, setAuctionKey] = useState(0);
  const [user, setUser] = useState<MeResponse>();

  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const currentKavelIdRef = useRef<number | null>(null);
  const removalTimeoutRef = useRef<number | null>(null);

  useEffect(() => {
    async function fetchKavels() {
      try {
        setLoading(true);
        let res: Response;
        if (sortOnApproval) {
          res = await authFetch("/api/KavelInfo/pending");
        } else {
          res = await authFetch(`/api/KavelInfo/${locatieId}`);
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
          const checkAuth = async () => {
            const me = await authCheckFetch("/me");
            if (me == undefined) {
              throw new Error(`Kon gebruiker niet ophalen.`);
            }
            setUser(me);
            setLoading(false);
            console.log(user?.email);
          };

          checkAuth();
        }
      } catch (err) {
        console.error("Kon kavels niet laden:", err);
        setKavels([]);
      } finally {
        setLoading(false);
      }
    }

    fetchKavels();
  }, [sortOnApproval, reload, locatieId]);

  useEffect(() => {
    if (onSelectKavel && kavels.length > 0 && selected !== null) {
      const newKavelId = kavels[selected].kavel.id;
      onSelectKavel(newKavelId);

      if (currentKavelIdRef.current !== newKavelId) {
        setStartMessage(null);
        currentKavelIdRef.current = newKavelId;
      }
    }
    setAuctionKey((prev) => prev + 1);
  }, [selected, kavels, onSelectKavel]);

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
        kavelVeilingId: number,
      ) => {
        setStartMessage({
          startingPrice,
          minimumPrice,
          durationMs,
          startTime: new Date(startTime),
          kavelVeilingId,
        });

        console.log(
          "SignalR message received for " + new Date(startTime).toString(),
        );
      },
    );

    // Listen for ContainersPurchased - updated to only 2 parameters
    // Only remove the kavel when hoeveelheidOver is 0 (no containers left).
    connection.on(
      "ContainersPurchased",
      (kavelId: number, hoeveelheidOver: number) => {
        console.log(
          `ContainersPurchased: kavelId=${kavelId}, over=${hoeveelheidOver}`,
        );

        if (hoeveelheidOver !== 0) return;

        if (removalTimeoutRef.current !== null) {
          clearTimeout(removalTimeoutRef.current);
        }

        removalTimeoutRef.current = window.setTimeout(() => {
          setKavels((currentKavels) => {
            const kavelIndex = currentKavels.findIndex(
              (k) => k.kavel.id === kavelId,
            );

            if (kavelIndex === -1) return currentKavels;

            const newKavels = currentKavels.filter(
              (k) => k.kavel.id !== kavelId,
            );

            if (selected === kavelIndex) {
              if (newKavels.length === 0) {
                setSelected(null);
              } else if (kavelIndex >= newKavels.length) {
                setSelected(newKavels.length - 1);
              } else {
                setSelected(kavelIndex);
              }
            } else if (selected !== null && selected > kavelIndex) {
              setSelected(selected - 1);
            }

            return newKavels;
          });
        }, 2000);
      },
    );

    connection.start().catch((err) => console.error("SignalR error:", err));

    return () => {
      if (removalTimeoutRef.current !== null) {
        clearTimeout(removalTimeoutRef.current);
      }
      connection.stop();
    };
  }, []);

  if (loading) return <div>Loading...</div>;
  if (kavels.length < 1) return <div>Geen kavels gevonden</div>;

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
  const imagePaths = [kavel.foto];

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
      key={auctionKey}
      price={kavel.maximumPrijs}
      startMessage={startMessage}
      connection={connectionRef.current}
      kavelId={kavel.id}
      gebruikerId={user?.id ?? ""}
      onPriceReachedZero={() => {
        // Remove kavel when price reaches 0, similar to when containers run out
        const kavelIdToRemove = kavel.id;

        setTimeout(() => {
          setKavels((currentKavels) => {
            const kavelIndex = currentKavels.findIndex(
              (k) => k.kavel.id === kavelIdToRemove,
            );

            if (kavelIndex === -1) return currentKavels;

            const newKavels = currentKavels.filter(
              (k) => k.kavel.id !== kavelIdToRemove,
            );

            if (selected === kavelIndex) {
              if (newKavels.length === 0) {
                setSelected(null);
              } else if (kavelIndex >= newKavels.length) {
                setSelected(newKavels.length - 1);
              } else {
                setSelected(kavelIndex);
              }
            } else if (selected !== null && selected > kavelIndex) {
              setSelected(selected - 1);
            }

            return newKavels;
          });
        }, 2000);
      }}
    />
  );

  return (
    <div
      className="kavel-info-fixed-layout"
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
                      backgroundColor: kavel.kavelkleur,
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

        <div className="kavel-image-container-fixed">
          <ImageSet images={imagePaths} />
        </div>
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
    "Max Prijs": `€${kavel?.kavel?.maximumPrijs?.toLocaleString() ?? "N/A"}`,
    Leverancier: kavel?.leverancier?.bedrijf?.bedrijfsnaam ?? "N/A",
    QI: kavel?.leverancier?.indexOfReliabilityOfInformation ?? "N/A",
    Kwaliteit: kavel?.kavel?.keurcode ?? "N/A",
  }));
};

export default KavelInfo;