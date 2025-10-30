import "./Spacer.css";

export default function Spacer({ color = "#d9d9d9", height = 6 }: SpacerProps) {
  return (
    <div className="spacer">
      <div
        style={{
          backgroundColor: color,
          height: height,
        }}
      />
    </div>
  );
}

interface SpacerProps {
  color?: string;
  height?: number;
}
