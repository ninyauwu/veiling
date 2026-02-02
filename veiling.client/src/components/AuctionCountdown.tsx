import "./AuctionCountdown.css";
import { useEffect, useState, useRef } from "react";
import type * as signalR from "@microsoft/signalr";
import PriceInterpolator from "./PriceBar";
import type { VeilingStartMessage } from "./PriceBar";
import SimpeleKnop from "./SimpeleKnop";
import Spacer from "./Spacer";
import BidFeedback, { type BidFeedbackStatus } from "./BidFeedback";
import { authFetch } from "../utils/AuthFetch";
import Popup from "./Popup";

function getNextNov15(): Date {
  const now = new Date();
  const year =
    now.getMonth() > 10 || (now.getMonth() === 10 && now.getDate() > 15)
      ? now.getFullYear() + 1
      : now.getFullYear();
  return new Date(year, 10, 15, 0, 0, 0, 0);
}

function getTimePartsUntil(target: Date) {
  let ms = target.getTime() - Date.now();
  if (ms < 0) ms = 0;
  const totalSec = Math.floor(ms / 1000);
  const days = Math.floor(totalSec / 86400);
  const hours = Math.floor((totalSec % 86400) / 3600);
  const minutes = Math.floor((totalSec % 3600) / 60);
  const seconds = totalSec % 60;
  return { days, hours, minutes, seconds };
}

interface AuctionCountdownProps {
  price?: number;
  quantity?: number;
  containers?: number;
  targetDate?: Date | null;
  startMessage: VeilingStartMessage | null;
  connection: signalR.HubConnection | null;
  kavelId: number;
  gebruikerId: string;
  onPriceReachedZero?: () => void;
}

interface GeplaatstBod {
  HoeveelheidContainers: number | null;
  GebruikerId: string;
  KavelId: number;
  KavelVeilingId: number;
}

interface BodResponse {
  accepted: boolean;
  acceptedPrice: number;
  receivedAt: string;
  remainingContainers: number;
}

interface AankoopContainers {
  GebruikerId: string;
  KavelId: number;
  Hoeveelheid: number;
}

export default function AuctionCountdown({
  price,
  quantity,
  containers,
  targetDate,
  startMessage,
  connection,
  kavelId,
  gebruikerId,
  onPriceReachedZero,
}: AuctionCountdownProps) {
  const [shouldInterrupt, setShouldInterrupt] = useState(false);
  const [isCountdown, setIsCountdown] = useState(false);
  const [currentPrice, setCurrentPrice] = useState(price ?? 0);
  const [isSubmittingBid, setIsSubmittingBid] = useState(false);
  const [serverReceivedTime, setServerReceivedTime] = useState<Date | null>(
    null,
    );
  const [feedbackStatus, setFeedbackStatus] =
    useState<BidFeedbackStatus | null>(null);
  const [awaitingBidResponse, setAwaitingBidResponse] = useState(false);
  const [userRoles, setUserRoles] = useState<string[]>([]);
  const [biddingRemainingContainers, setBiddingRemainingContainers] = useState<
    number | null
  >(null);

  // Track if we've already triggered removal for this auction
    const hasTriggeredRemovalRef = useRef(false);
    const lastKnownPriceRef = useRef<number | null>(null);
    const lastRemainingMsRef = useRef<number | null>(null);


  // Purchase popup state
  const [showPurchasePopup, setShowPurchasePopup] = useState(false);
  const [remainingContainers, setRemainingContainers] = useState(0);
  const [containerCountStr, setContainerCountStr] = useState("1");
  const [isSubmittingPurchase, setIsSubmittingPurchase] = useState(false);
  const [purchaseError, setPurchaseError] = useState<string | null>(null);

  const containerCount =
    containerCountStr === "" ? 0 : parseInt(containerCountStr, 10) || 0;

  const resolvedTarget = (() => {
    if (targetDate === null) return getNextNov15();
    if (targetDate instanceof Date && !isNaN(targetDate.getTime())) {
      return targetDate;
    }
    const parsed = new Date(targetDate as unknown as string);
    if (!isNaN(parsed.getTime())) return parsed;
    return getNextNov15();
  })();

  const [time, setTime] = useState(() => getTimePartsUntil(resolvedTarget));
  const formattedPrice = "€" + currentPrice.toFixed(2).toString();

  // Fetch initial remaining containers when component mounts or kavelId changes
  useEffect(() => {
    async function fetchRemainingContainers() {
      try {
        console.log(`Fetching remaining containers for kavelId: ${kavelId}`);
        const response = await authFetch(
          `/api/KavelInfo/remainingcontainers/${kavelId}`,
        );
        console.log(`Response status: ${response.status}`);

        if (response.ok) {
          const text = await response.text();
          console.log(`Response text: "${text}"`);
          const remaining = parseInt(text, 10);
          console.log(
            `Parsed remaining: ${remaining}, isNaN: ${isNaN(remaining)}`,
          );

          if (!isNaN(remaining)) {
            console.log(`Setting biddingRemainingContainers to: ${remaining}`);
            setBiddingRemainingContainers(remaining);
          } else {
            console.error(`Failed to parse remaining containers: "${text}"`);
          }
        } else {
          console.error(
            `Failed to fetch: ${response.status} ${response.statusText}`,
          );
        }
      } catch (error) {
        console.error("Error fetching remaining containers:", error);
      }
    }

    fetchRemainingContainers();
  }, [kavelId]);

  useEffect(() => {
    async function fetchUserRoles() {
      try {
        const response = await authFetch("/me");
        if (response.ok) {
          const data = await response.json();
          setUserRoles(data.roles || []);
        }
      } catch (error) {
        console.error("Error fetching user roles:", error);
      }
    }

    fetchUserRoles();
  }, []);

  useEffect(() => {
    const id = setInterval(() => {
      setTime(getTimePartsUntil(resolvedTarget));
    }, 1000);

    return () => clearInterval(id);
  }, [resolvedTarget]);

  useEffect(() => {
    const checkCountdownStatus = () => {
      if (!startMessage) {
        setIsCountdown(false);
        return;
      }

      const now = new Date();
      const veilingStart = new Date(startMessage.startTime);

      if (now < veilingStart) {
        setIsCountdown(false);
      } else {
        setIsCountdown(false);
      }
    };

    checkCountdownStatus();

    const interval = setInterval(checkCountdownStatus, 1000);

    return () => clearInterval(interval);
  }, [startMessage]);

    useEffect(() => {
        console.log("startMessage changed:", startMessage);
        if (startMessage) {
            setCurrentPrice(startMessage.startingPrice ?? 0);
            setShouldInterrupt(false);
            setServerReceivedTime(null);
            setFeedbackStatus(null);
            setAwaitingBidResponse(false);
            setIsSubmittingBid(false);
            lastKnownPriceRef.current = null;
            lastRemainingMsRef.current = null;

            hasTriggeredRemovalRef.current = false;
        }
    }, [startMessage]);

    useEffect(() => {
        if (!startMessage) return;
        setShouldInterrupt(false);
        setServerReceivedTime(new Date());
    }, [startMessage?.startTime]);



  useEffect(() => {
    if (!connection) return;

    const handleBidPlaced = (bidKavelId: number) => {
      if (bidKavelId === kavelId && !awaitingBidResponse && !feedbackStatus) {
        setFeedbackStatus("outbid");
        setShouldInterrupt(true); // Freeze the price bar for other users
      }
    };

    connection.on("BidPlaced", handleBidPlaced);

    return () => {
      connection.off("BidPlaced", handleBidPlaced);
    };
  }, [connection, kavelId, awaitingBidResponse, feedbackStatus]);

  // Listen for ContainersPurchased SignalR event to update remaining containers
  useEffect(() => {
    if (!connection) return;

    const handleContainersPurchased = (
      eventKavelId: number,
      hoeveelheidOver: number,
    ) => {
      if (eventKavelId === kavelId) {
        setBiddingRemainingContainers(hoeveelheidOver);
      }
    };

    connection.on("ContainersPurchased", handleContainersPurchased);

    return () => {
      connection.off("ContainersPurchased", handleContainersPurchased);
    };
  }, [connection, kavelId]);

  // Call callback when price reaches minimum price (auction ends)
  useEffect(() => {
    if (!startMessage || !onPriceReachedZero || hasTriggeredRemovalRef.current)
      return;

    const minPrice = startMessage.minimumPrice ?? 0;
    console.log(`Current price: ${currentPrice}, Minimum price: ${minPrice}`);

    if (currentPrice <= minPrice) {
      console.log(
        `Price reached minimum for kavelId: ${kavelId}, triggering removal`,
      );
      hasTriggeredRemovalRef.current = true;
      onPriceReachedZero();
    }
  }, [currentPrice, startMessage, onPriceReachedZero, kavelId]);

    const simulateSignalRMessage = () => {
        if (!connection) return;

        setShouldInterrupt(false);
        setFeedbackStatus(null);
        setServerReceivedTime(null);

        const startTime = new Date();
        startTime.setSeconds(startTime.getSeconds() + 1);

        const effectiveStartingPrice =
            lastKnownPriceRef.current ?? price ?? 0.8;

        const effectiveDurationMs =
            lastRemainingMsRef.current ?? 30000;

        connection
            .invoke(
                "SendVeilingStart",
                kavelId,
                effectiveStartingPrice,
                0.3,
                effectiveDurationMs,
                startTime,
            )
            .catch((err) => console.error("Failed to send message:", err));
    };


  const placeBid = async () => {
    setIsSubmittingBid(true);
    setAwaitingBidResponse(true);
    setShouldInterrupt(true);

    try {
      const bod: GeplaatstBod = {
        HoeveelheidContainers: containers ?? null,
        GebruikerId: gebruikerId,
        KavelId: kavelId,
        KavelVeilingId: startMessage?.kavelVeilingId ?? 0,
      };

      const response = await authFetch("/api/Bod", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(bod),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data: BodResponse = await response.json();
      console.log("Bid response data:", data);
      setServerReceivedTime(new Date(data.receivedAt));

      if (data.accepted) {
        setFeedbackStatus("accepted");
        setRemainingContainers(data.remainingContainers);
        setContainerCountStr("1");
        setPurchaseError(null);
        setShowPurchasePopup(true);
      } else {
        setFeedbackStatus("rejected");
      }
    } catch (err) {
      console.error("Failed to place bid:", err);
      setFeedbackStatus("rejected");
    } finally {
      setIsSubmittingBid(false);
      setAwaitingBidResponse(false);
    }
  };

  const submitPurchase = async () => {
    setIsSubmittingPurchase(true);
    setPurchaseError(null);

    try {
      const aankoop: AankoopContainers = {
        GebruikerId: gebruikerId,
        KavelId: kavelId,
        Hoeveelheid: containerCount,
      };

      const response = await authFetch("/api/Aankoop", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(aankoop),
      });

      if (!response.ok) {
        const errorText = await response.text();
        setPurchaseError(errorText || "Aankoop mislukt.");
        return;
      }

      // Purchase succeeded — close popup.
      // Removal is now handled by KavelInfo listening for ContainersPurchased.
      setShowPurchasePopup(false);
    } catch (err) {
      console.error("Failed to purchase containers:", err);
      setPurchaseError("Er is een fout opgetreden bij de aankoop.");
    } finally {
      setIsSubmittingPurchase(false);
    }
  };

  const handleFeedbackComplete = () => {
    setFeedbackStatus(null);
  };

  const isAdministrator = userRoles.includes("Administrator");
  const isVeilingmeester = userRoles.includes("Veilingmeester");
  const isBedrijf = userRoles.includes("Bedrijfsvertegenwoordiger");
  const isManager = userRoles.includes("BedrijfManager");

  const canBid = isBedrijf || isManager || isAdministrator;
  const canStartVeiling = isVeilingmeester || isAdministrator;

  const purchasePopup = showPurchasePopup ? (
    <Popup allowManualClose={false} onClose={() => setShowPurchasePopup(false)}>
      <h3 style={{ marginBottom: "12px" }}>Containers kopen</h3>
      <p style={{ marginBottom: "16px" }}>
        Hoeveel containers wilt u kopen? (max {remainingContainers})
      </p>
      <div style={{ display: "flex", flexDirection: "column", gap: "12px" }}>
        <input
          type="number"
          min={0}
          max={remainingContainers}
          value={containerCountStr}
          onChange={(e) => {
            const raw = e.target.value;
            if (raw === "") {
              setContainerCountStr("");
              return;
            }
            const val = parseInt(raw, 10);
            if (!isNaN(val)) {
              setContainerCountStr(
                String(Math.max(0, Math.min(val, remainingContainers))),
              );
            }
          }}
          style={{
            padding: "8px 12px",
            borderRadius: "6px",
            border: "1px solid #ccc",
            fontSize: "16px",
            width: "120px",
          }}
        />
        {purchaseError && (
          <p style={{ color: "#c0392b", margin: 0 }}>{purchaseError}</p>
        )}
        <div style={{ display: "flex", gap: "8px" }}>
          <SimpeleKnop
            onClick={submitPurchase}
            appearance="primary"
            disabled={isSubmittingPurchase || containerCount < 1}
          >
            {isSubmittingPurchase ? "Bezig..." : "Kopen"}
          </SimpeleKnop>
          <SimpeleKnop
            onClick={() => setShowPurchasePopup(false)}
            appearance="secondary"
            disabled={isSubmittingPurchase}
          >
            Annuleren
          </SimpeleKnop>
        </div>
      </div>
    </Popup>
  ) : null;

  const countdown = (
    <section
      className="auc-card"
      aria-label="Veiling info"
      style={{ position: "relative" }}
    >
      {purchasePopup}
      <BidFeedback
        status={feedbackStatus}
        onFadeComplete={handleFeedbackComplete}
      />
      <header className="auc-card__head">
        <h3 className="auc-card__title">Veiling start in</h3>
      </header>

      <div className="auc-timer">
        <div className="auc-timer__cell">
          <span className="auc-timer__value">{time.days}</span>
          <span className="auc-timer__unit">d</span>
        </div>
        <div className="auc-timer__cell">
          <span className="auc-timer__value">{time.hours}</span>
          <span className="auc-timer__unit">h</span>
        </div>
        <div className="auc-timer__cell">
          <span className="auc-timer__value">{time.minutes}</span>
          <span className="auc-timer__unit">m</span>
        </div>
        <div className="auc-timer__cell">
          <span className="auc-timer__value">{time.seconds}</span>
          <span className="auc-timer__unit">s</span>
        </div>
      </div>

      <hr className="auc-divider" />
      <div className="auc-field">
        <div className="auc-label">Startprijs</div>
        <div className="auc-price">
          <span className="auc-price__main">{formattedPrice}</span>
        </div>
      </div>

      <div className="auc-field">
        <div className="auc-label">Aantal Eenheden</div>
        <div className="auc-qty">
          <span className="auc-qty__main">{quantity}</span>
          <span className="auc-qty__sub">{containers}</span>
        </div>
      </div>
      <PriceInterpolator
        startMessage={startMessage}
        shouldInterrupt={shouldInterrupt}
        serverReceivedTime={serverReceivedTime}
      />
      <div className="button-container">
        {canBid && (
          <SimpeleKnop
            onClick={placeBid}
            appearance="primary"
            disabled={isSubmittingBid}
          >
            {isSubmittingBid ? "Bezig..." : "Bieden"}
          </SimpeleKnop>
        )}
        {canStartVeiling && (
          <SimpeleKnop onClick={simulateSignalRMessage} appearance="secondary">
            Start veiling
          </SimpeleKnop>
        )}
      </div>
    </section>
  );

  const bidding = (
    <section
      className="auc-card"
      aria-label="Veiling info"
      style={{ position: "relative" }}
    >
      {purchasePopup}
      <BidFeedback
        status={feedbackStatus}
        onFadeComplete={handleFeedbackComplete}
      />
      <header className="auc-card__head">
        <h3 className="auc-card__title">Bieden</h3>
      </header>
      <PriceInterpolator
        startMessage={startMessage}
        shouldInterrupt={shouldInterrupt}
              onChange={(pr) => {
                  setCurrentPrice(pr);
                  lastKnownPriceRef.current = pr;
              }}
        serverReceivedTime={serverReceivedTime}
      />
      <Spacer />
      <div className="auc-field">
        <div className="auc-label">Prijs per eenheid</div>
        <div className="auc-price">
          <span className="auc-price__main">{formattedPrice}</span>
        </div>
      </div>
      <Spacer />
      <div className="auc-field">
        <div className="auc-label">Beschikbare containers</div>
        <div className="auc-qty">
          <span className="auc-qty__main">
            {biddingRemainingContainers !== null
              ? biddingRemainingContainers
              : "..."}
          </span>
        </div>
      </div>
      <Spacer />
      <div className="button-container">
        {canBid && (
          <SimpeleKnop
            onClick={placeBid}
            appearance="primary"
            disabled={isSubmittingBid}
          >
            {isSubmittingBid ? "Bezig..." : "Bied"}
          </SimpeleKnop>
        )}
        {canStartVeiling && (
          <SimpeleKnop onClick={simulateSignalRMessage} appearance="secondary">
            Start veiling
          </SimpeleKnop>
        )}
      </div>
    </section>
  );

  return isCountdown ? countdown : bidding;
}
