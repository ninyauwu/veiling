import "./AuctionCountdown.css";
import { useEffect, useState } from "react";
import type * as signalR from "@microsoft/signalr";
import PriceInterpolator from "./PriceBar";
import type { VeilingStartMessage } from "./PriceBar";
import SimpeleKnop from "./SimpeleKnop";
import Spacer from "./Spacer";
import BidFeedback, { type BidFeedbackStatus } from "./BidFeedback";
import { authFetch } from "../utils/AuthFetch";

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
}

interface GeplaatstBod {
  HoeveelheidContainers: number | null;
  GebruikerId: string;
  KavelId: number;
}

interface BodResponse {
  accepted: boolean;
  acceptedPrice: number;
  receivedAt: string;
}

export default function AuctionCountdown({
                                           price,
                                           quantity,
                                           containers,
                                           targetDate,
                                           startMessage,
                                           connection,
                                           kavelId,
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
    console.log("startMessage changed:", startMessage);
    if (startMessage) {
      console.log("startingPrice:", startMessage.startingPrice);
      setIsCountdown(false);
      setCurrentPrice(startMessage.startingPrice ?? 0);
      setShouldInterrupt(false);
      setServerReceivedTime(null);
    }
  }, [startMessage]);

  useEffect(() => {
    if (!connection) return;

    const handleBidPlaced = (bidKavelId: number) => {
      if (bidKavelId === kavelId && !awaitingBidResponse && !feedbackStatus) {
        setFeedbackStatus("outbid");
      }
    };

    connection.on("BidPlaced", handleBidPlaced);

    return () => {
      connection.off("BidPlaced", handleBidPlaced);
    };
  }, [connection, kavelId, awaitingBidResponse, feedbackStatus]);

  const simulateSignalRMessage = () => {
    if (!connection) return;

    // Reset state before starting new auction
    setShouldInterrupt(false);
    setFeedbackStatus(null);
    setServerReceivedTime(null);

    const startTime = new Date();
    startTime.setSeconds(startTime.getSeconds() + 1);

    connection
      .invoke("SendVeilingStart", kavelId, 0.8, 0.3, 5000, startTime)
      .catch((err) => console.error("Failed to send message:", err));
  };

  const placeBid = async () => {
    setIsSubmittingBid(true);
    setAwaitingBidResponse(true);
    setShouldInterrupt(true);

    try {
      const bod: GeplaatstBod = {
        HoeveelheidContainers: containers ?? null,
        GebruikerId: "",
        KavelId: kavelId,
      };

      const response = await fetch("/api/Bod", {
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
      console.log("data.accepted:", data.accepted);
      setServerReceivedTime(new Date(data.receivedAt));

      if (data.accepted) {
        console.log("Setting feedback to accepted");
        setFeedbackStatus("accepted");
      } else {
        console.log("Setting feedback to rejected");
        setFeedbackStatus("rejected");
      }

      console.log("Bid response:", data);
    } catch (err) {
      console.error("Failed to place bid:", err);
      setFeedbackStatus("rejected");
    } finally {
      setIsSubmittingBid(false);
      setAwaitingBidResponse(false);
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

  const countdown = (
    <section
      className="auc-card"
      aria-label="Veiling info"
      style={{ position: "relative" }}
    >
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
        onChange={(pr) => setCurrentPrice(pr)}
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