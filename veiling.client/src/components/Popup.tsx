import React, { useRef, useEffect } from "react";
import type { ReactNode } from "react";

interface PopupProps {
  children: ReactNode;
  allowManualClose?: boolean;
  onClose?: () => void;
}

export default function Popup({
  children,
  allowManualClose = true,
  onClose,
}: PopupProps) {
  const popupRef = useRef<HTMLDivElement>(null);
  const firstFocusableRef = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    const previouslyFocused = document.activeElement as HTMLElement;

    if (firstFocusableRef.current && allowManualClose) {
      firstFocusableRef.current.focus();
    } else if (popupRef.current) {
      popupRef.current.focus();
    }

    return () => {
      if (previouslyFocused) {
        previouslyFocused.focus();
      }
    };
  }, [allowManualClose]);

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === "Tab") {
      const focusableElements = popupRef.current?.querySelectorAll(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])',
      );

      if (!focusableElements || focusableElements.length === 0) {
        e.preventDefault();
        return;
      }

      const firstElement = focusableElements[0] as HTMLElement;
      const lastElement = focusableElements[
        focusableElements.length - 1
      ] as HTMLElement;

      if (e.shiftKey) {
        if (document.activeElement === firstElement) {
          e.preventDefault();
          lastElement.focus();
        }
      } else {
        if (document.activeElement === lastElement) {
          e.preventDefault();
          firstElement.focus();
        }
      }
    }
  };

  const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget && allowManualClose && onClose) {
      onClose();
    }
  };

  return (
    <div
      className="fixed inset-0 bg-black bg-opacity-50 flex items-start justify-center z-50 pt-30 pb-4"
      onClick={handleBackdropClick}
      role="dialog"
      aria-modal="true"
    >
      <div
        ref={popupRef}
        className="relative bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[calc(100vh-120px)] overflow-auto"
        onKeyDown={handleKeyDown}
        tabIndex={-1}
        style={{
          color: "#000000",
          backgroundColor: "#FFFFFF",
        }}
      >
        {allowManualClose && onClose && (
          <button
            ref={firstFocusableRef}
            onClick={onClose}
            className="absolute top-4 right-4 w-10 h-10 flex items-center justify-center rounded transition-colors hover:opacity-80 focus:outline-none focus:ring-2"
            style={{
              color: "#FFFFFF",
              backgroundColor: "#7A1F3D",
              fontSize: "28px",
              fontWeight: "bold",
              lineHeight: "1",
            }}
            aria-label="Close popup"
          >
            ×
          </button>
        )}
        <div className="p-6">{children}</div>
      </div>
    </div>
  );
}
