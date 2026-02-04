import { useState, useEffect, useRef } from "react";
import "./PriceBar.css";

export interface VeilingStartMessage {
  startingPrice: number;
  minimumPrice: number;
  durationMs: number;
  startTime: Date;
  kavelVeilingId: number;
}

interface PriceBarProps {
  progress: number;
  remaining?: number;
}

function PriceBar({ progress, remaining }: PriceBarProps) {
  const progressColor = progress < 0.5 ? "#FFFFFF" : "#000000";
  return (
    <div className="price-bar">
      <div
        className="price-bar-fill"
        style={{
          width: `${(1 - progress) * 100}%`,
        }}
      />
      <div className="price-bar-text" style={{ color: progressColor }}>
        {remaining ? remaining.toFixed(1) : "-"}
      </div>
    </div>
  );
}

interface PriceInterpolatorProps {
  startMessage: VeilingStartMessage | null;
  shouldInterrupt: boolean;
  onChange?: (newPrice: number) => void;
  serverReceivedTime?: Date | null;
}

export default function PriceInterpolator({
                                            startMessage,
                                            shouldInterrupt,
                                            onChange,
                                            serverReceivedTime,
                                          }: PriceInterpolatorProps) {
  const [, setCurrentValue] = useState<number>(0);
  const [progress, setProgress] = useState<number>(0);
  const [remaining, setRemaining] = useState<number | undefined>(0);
  const animationFrameRef = useRef<number | null>(null);
  const startTimeRef = useRef<number | null>(null);
  const previousMessageRef = useRef<VeilingStartMessage | null>(null);
  const timeOffsetRef = useRef<number>(0);
  const frozenProgressRef = useRef<number>(0);
  const frozenPriceRef = useRef<number | null>(null);

  useEffect(() => {
    if (shouldInterrupt) {
      return;
    }
    if (!startMessage) {
      if (animationFrameRef.current !== null) {
        cancelAnimationFrame(animationFrameRef.current);
        animationFrameRef.current = null;
      }
      setCurrentValue(0);
      setProgress(0);
      setRemaining(0);
      startTimeRef.current = null;
      previousMessageRef.current = null;
      timeOffsetRef.current = 0;
      frozenProgressRef.current = 0;
      return;
    }

    if (
      previousMessageRef.current &&
      previousMessageRef.current.kavelVeilingId === startMessage.kavelVeilingId &&
      frozenPriceRef.current === null  // <-- Alleen skippen als we NIET bevroren waren
    ) {
      return;
    }

    previousMessageRef.current = startMessage;
    let { startingPrice, minimumPrice, durationMs, startTime } = startMessage;

// Als we bevroren waren, herstart dan vanaf NU
    if (frozenPriceRef.current !== null) {
      startingPrice = frozenPriceRef.current;
      durationMs = durationMs * (1 - frozenProgressRef.current);

      // Nieuwe starttijd = NU + 100ms
      const now = new Date();
      startTime = new Date(now.getTime() + 100);
    }

    if (animationFrameRef.current !== null) {
      cancelAnimationFrame(animationFrameRef.current);
    }

    startTimeRef.current = null;

    const effectiveStartTime = startTime;

    const animate = (timestamp: number) => {
      // Reset frozen state HIER, niet eerder
      if (frozenProgressRef.current > 0) {
        frozenProgressRef.current = 0;
        frozenPriceRef.current = null;
      }

      const adjustedNow = Date.now() + timeOffsetRef.current;
      const startTimestamp = effectiveStartTime.getTime();

      if (adjustedNow < startTimestamp) {
        animationFrameRef.current = requestAnimationFrame(animate);
        return;
      }

      if (startTimeRef.current === null) {
        startTimeRef.current = timestamp;
      }

      const elapsed = adjustedNow - startTimestamp;
      const progressValue = Math.min(elapsed / durationMs, 1);
      const remainingValue = Math.max((durationMs - elapsed) / 1000, 0);
      const priceValue = Math.max(
        (1 - progressValue) * (startingPrice - minimumPrice) + minimumPrice,
        minimumPrice,
        0,
      );

      const interpolatedValue =
        startingPrice - (startingPrice - minimumPrice) * progressValue;

      setCurrentValue(interpolatedValue);
      setProgress(progressValue);
      setRemaining(remainingValue);

      if (onChange) {
        onChange(priceValue);
      }

      if (progressValue < 1) {
        animationFrameRef.current = requestAnimationFrame(animate);
      } else {
        animationFrameRef.current = null;
      }
    };

    animationFrameRef.current = requestAnimationFrame(animate);
  }, [startMessage, onChange, shouldInterrupt]);

  useEffect(() => {
    if (serverReceivedTime && startMessage) {
      const clientTime = Date.now();
      const serverTime = serverReceivedTime.getTime();
      const offset = serverTime - clientTime;
      timeOffsetRef.current = offset;
    }
  }, [serverReceivedTime, startMessage]);

  useEffect(() => {
    if (shouldInterrupt && animationFrameRef.current !== null && startMessage) {
      // Bevries de huidige progress en prijs
      frozenProgressRef.current = progress;
      frozenPriceRef.current =
        startMessage.startingPrice -
        (startMessage.startingPrice - startMessage.minimumPrice) * progress;

      // Stop de animatie
      cancelAnimationFrame(animationFrameRef.current);
      animationFrameRef.current = null;
    }
  }, [shouldInterrupt, progress, startMessage]);

  useEffect(() => {
    return () => {
      if (animationFrameRef.current !== null) {
        cancelAnimationFrame(animationFrameRef.current);
      }
    };
  }, []);

  return (
    <div className="price-interpolator-container">
      <PriceBar progress={progress} remaining={remaining} />
    </div>
  );
}