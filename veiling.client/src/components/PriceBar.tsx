import { useState, useEffect, useRef } from "react";
import "./PriceBar.css";

export interface VeilingStartMessage {
  startingPrice: number;
  minimumPrice: number;
  durationMs: number;
  startTime: Date;
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
}

export default function PriceInterpolator({
  startMessage,
  shouldInterrupt,
  onChange,
}: PriceInterpolatorProps) {
  const [currentValue, setCurrentValue] = useState<number>(0);
  const [progress, setProgress] = useState<number>(0);
  const [remaining, setRemaining] = useState<number | undefined>(0);

  const animationFrameRef = useRef<number | null>(null);
  const startTimeRef = useRef<number | null>(null);
  const previousMessageRef = useRef<VeilingStartMessage | null>(null);

  useEffect(() => {
    if (!startMessage || startMessage === previousMessageRef.current) {
      return;
    }

    previousMessageRef.current = startMessage;
    const { startingPrice, minimumPrice, durationMs, startTime } = startMessage;

    if (animationFrameRef.current !== null) {
      cancelAnimationFrame(animationFrameRef.current);
    }

    startTimeRef.current = null;

    const animate = (timestamp: number) => {
      const now = Date.now();
      const startTimestamp = startTime.getTime();

      if (now < startTimestamp) {
        animationFrameRef.current = requestAnimationFrame(animate);
        return;
      }

      if (startTimeRef.current === null) {
        startTimeRef.current = timestamp;
      }

      const elapsed = timestamp - startTimeRef.current;
      const progressValue = Math.min(elapsed / durationMs, 1);
      const remainingValue = Math.max((durationMs - elapsed) / 1000, 0);
      const priceValue = Math.max(
        (1 - progressValue) * (startMessage.startingPrice - minimumPrice) +
          minimumPrice,
        minimumPrice,
        0,
      );

      const interpolatedValue =
        startingPrice - (startingPrice - minimumPrice) * progressValue;
      setCurrentValue(currentValue);
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
  }, [startMessage]);

  useEffect(() => {
    if (shouldInterrupt && animationFrameRef.current !== null) {
      cancelAnimationFrame(animationFrameRef.current);
      animationFrameRef.current = null;
    }
  }, [shouldInterrupt]);

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
