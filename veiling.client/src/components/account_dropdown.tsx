import { useEffect, useRef, useState } from "react";
import "./account_dropdown.css";

interface account_dropdownProps {
  className?: string;
  // Optioneel: positie-variant ("right" voor rechts uitlijnen)
  align?: "left" | "right";
  // Placeholder data kun je straks vervangen door echte user data
  lines?: [string, string, string, string];
  avatarUrl?: string;
}

export default function account_dropdown({
  className = "",
  align = "right",
  lines = [
    "Naam",
    "E-mail",
    "Bedrijf",
    "Functie",
  ],
  avatarUrl,
}: account_dropdownProps) {
  const [open, setOpen] = useState(false);
  const btnRef = useRef<HTMLButtonElement | null>(null);
  const menuRef = useRef<HTMLDivElement | null>(null);

  // Buiten-klik en Escape om te sluiten
  useEffect(() => {
    function onDocClick(e: MouseEvent) {
      if (!open) return;
      const target = e.target as Node;
      if (
        menuRef.current &&
        !menuRef.current.contains(target) &&
        btnRef.current &&
        !btnRef.current.contains(target)
      ) {
        setOpen(false);
      }
    }
    function onKey(e: KeyboardEvent) {
      if (e.key === "Escape") setOpen(false);
    }
    document.addEventListener("mousedown", onDocClick);
    document.addEventListener("keydown", onKey);
    return () => {
      document.removeEventListener("mousedown", onDocClick);
      document.removeEventListener("keydown", onKey);
    };
  }, [open]);

  // Focus het menu wanneer open
  useEffect(() => {
    if (open) {
      // focus het eerste focuseerbare element in het menu
      const first = menuRef.current?.querySelector<HTMLElement>("[tabindex]");
      first?.focus();
    }
  }, [open]);

  return (
    <div className={`pd-wrapper ${className}`}>
      <button
        ref={btnRef}
        className="pd-trigger"
        aria-haspopup="menu"
        aria-expanded={open}
        aria-controls="profile-dropdown-menu"
        onClick={() => setOpen((v) => !v)}
        title="Profiel"
      >
        {/* Icoon (svg) of avatar */}
        {avatarUrl ? (
          <img src={avatarUrl} alt="Profiel" className="pd-avatar" />
        ) : (
          <svg
            className="pd-icon"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            aria-hidden="true"
          >
            <path
              fill="currentColor"
              d="M12 12a5 5 0 1 0-5-5a5 5 0 0 0 5 5Zm0 2c-4.418 0-8 2.239-8 5v1h16v-1c0-2.761-3.582-5-8-5Z"
            />
          </svg>
        )}
      </button>

      {open && (
        <div
          ref={menuRef}
          id="profile-dropdown-menu"
          role="menu"
          aria-label="Profielmenu"
          className={`pd-menu pd-menu--${align}`}
        >
          <div className="pd-header" tabIndex={0}>
            <div className="pd-header__left">
              {avatarUrl ? (
                <img src={avatarUrl} alt="" className="pd-header__avatar" />
              ) : (
                <div className="pd-header__avatar pd-header__avatar--fallback" aria-hidden>
                  <svg
                    width="28"
                    height="28"
                    viewBox="0 0 24 24"
                    className="pd-header__avatarIcon"
                  >
                    <path
                      fill="currentColor"
                      d="M12 12a5 5 0 1 0-5-5a5 5 0 0 0 5 5Zm0 2c-4.418 0-8 2.239-8 5v1h16v-1c0-2.761-3.582-5-8-5Z"
                    />
                  </svg>
                </div>
              )}
            </div>
            <div className="pd-header__right">
              <p className="pd-line">{lines[0]}</p>
              <p className="pd-line">{lines[1]}</p>
              <p className="pd-line">{lines[2]}</p>
              <p className="pd-line">{lines[3]}</p>
            </div>
          </div>

        <div className="pd-actions">
           <button className="pd-action pd-action--danger" role="menuitem" tabIndex={0}>
            Uitloggen
            </button>
        </div>
        </div>
      )}
    </div>
  );
}