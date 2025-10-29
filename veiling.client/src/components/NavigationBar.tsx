import { SimpeleKnopPijl } from "./SimpeleKnop";

interface NavigationBarProps {
  onPrevious?: () => void;
  onNext?: () => void;
  getSelectedItemString?: () => string;
  className?: string;
  previousDisabled?: boolean;
  nextDisabled?: boolean;
}

export default function NavigationBar({
  onPrevious,
  onNext,
  getSelectedItemString,
  className = "",
  previousDisabled = false,
  nextDisabled = false,
}: NavigationBarProps) {
  // Default handlers that do nothing if callbacks aren't provided
  const handlePrevious = onPrevious || (() => {});
  const handleNext = onNext || (() => {});

  // Default selected item text if no function is provided
  const selectedText = getSelectedItemString
    ? getSelectedItemString()
    : "Unknown";

  return (
    <div className={`flex-row-justify ${className}`}>
      <SimpeleKnopPijl
        direction="left"
        onClick={handlePrevious}
        disabled={previousDisabled}
      >
        Vorige
      </SimpeleKnopPijl>

      <div className="selection-box">Geselecteerd: {selectedText}</div>

      <SimpeleKnopPijl
        direction="right"
        onClick={handleNext}
        disabled={nextDisabled}
      >
        Volgende
      </SimpeleKnopPijl>
    </div>
  );
}
