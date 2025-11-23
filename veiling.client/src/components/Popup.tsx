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
      className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
      onClick={handleBackdropClick}
      role="dialog"
      aria-modal="true"
    >
      <div
        ref={popupRef}
        className="relative bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-auto"
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
            className="absolute top-4 right-4 w-8 h-8 flex items-center justify-center rounded hover:bg-opacity-10 transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2"
            style={{
              color: "#7A1F3D",
              backgroundColor: "#D9D9D9",
            }}
            aria-label="Close popup"
          >
            <svg width="16" height="16" viewBox="0 0 16 16" fill="none">
              <path
                d="M12 4L4 12M4 4L12 12"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
              />
            </svg>
          </button>
        )}
        <div className="p-6">{children}</div>
      </div>
    </div>
  );
}
