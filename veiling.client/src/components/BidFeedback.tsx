import { useEffect, useState } from "react";
import "./BidFeedback.css";

export type BidFeedbackStatus = "accepted" | "rejected" | "outbid";

interface BidFeedbackProps {
  status: BidFeedbackStatus | null;
  onFadeComplete?: () => void;
}

export default function BidFeedback({
  status,
  onFadeComplete,
}: BidFeedbackProps) {
  const [isVisible, setIsVisible] = useState(false);
  const [isFading, setIsFading] = useState(false);

  useEffect(() => {
    if (status) {
      setIsVisible(true);
      setIsFading(false);

      const fadeTimer = setTimeout(() => {
        setIsFading(true);
      }, 2000);

      const hideTimer = setTimeout(() => {
        setIsVisible(false);
        setIsFading(false);
        if (onFadeComplete) {
          onFadeComplete();
        }
      }, 2500);

      return () => {
        clearTimeout(fadeTimer);
        clearTimeout(hideTimer);
      };
    } else {
      setIsVisible(false);
      setIsFading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [status]);

  if (!isVisible || !status) return null;

  const getStatusConfig = () => {
    switch (status) {
      case "accepted":
        return {
          className: "bid-feedback--accepted",
          icon: (
            <svg viewBox="0 0 24 24" className="bid-feedback__icon">
              <path
                d="M20 6L9 17l-5-5"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          ),
        };
      case "rejected":
      case "outbid":
        return {
          className: "bid-feedback--outbid",
          icon: (
            <svg viewBox="0 0 24 24" className="bid-feedback__icon">
              <path
                d="M5 12h14M12 5l7 7-7 7"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
          ),
        };
    }
  };

  const config = getStatusConfig();

  return (
    <div
      className={`bid-feedback ${config.className} ${isFading ? "bid-feedback--fading" : ""}`}
      role="status"
      aria-live="polite"
    >
      <div className="bid-feedback__circle">{config.icon}</div>
    </div>
  );
}
