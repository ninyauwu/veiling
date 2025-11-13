import "./SimpeleKnop.css";
import type { ReactNode } from "react";

export default function SimpeleKnop({
  children,
  label,
  onClick,
  disabled,
  type = "button",
  appearance = "secondary",
}: SimpeleKnopProps) {
  return (
    <button
      className={"simpele-knop " + appearance}
      onClick={onClick}
      disabled={disabled}
      type={type}
    >
      {label}
      {children}
    </button>
  );
}

export function SimpeleKnopPijl({
  children,
  direction,
  label,
  onClick,
  disabled = false,
  type = "button",
  appearance = "secondary",
}: SimpeleKnopPijlProps) {
  return (
    <button
      className={"simpele-knop " + appearance}
      onClick={onClick}
      disabled={disabled}
      type={type}
    >
      {direction == "left" ? getIcon("/icons/arrow_left.svg") : null}
      {label}
      {children}
      {direction == "right" ? getIcon("/icons/arrow_right.svg") : null}
    </button>
  );
}

function getIcon(maskUrl: string) {
  return (
    <div
      className="masked-color"
      style={{
        maskImage: `url(${maskUrl})`,
        WebkitMaskImage: `url(${maskUrl})`,
      }}
    />
  );
}

interface SimpeleKnopProps {
  children: ReactNode;
  label?: string;
  onClick?: () => void;
  disabled?: boolean;
  type?: "button" | "submit" | "reset";
  appearance?: "primary" | "secondary";
}

interface SimpeleKnopPijlProps extends SimpeleKnopProps {
  direction: "left" | "right";
}
