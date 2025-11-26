import type { AppointmentProps } from "./AppointmentTypes";

export default function Appointment({
  appointment,
  onMouseDown,
  onDoubleClick,
  hourHeight,
  isDragging,
  isResizing,
}: AppointmentProps) {
  const top = appointment.startHour * hourHeight;
  const height = appointment.durationHours * hourHeight;

  return (
    <div
      className="absolute left-1 right-1 rounded shadow cursor-move overflow-hidden select-none"
      style={{
        top: `${top}px`,
        height: `${height}px`,
        backgroundColor: "#7A1F3D",
        color: "#FFFFFF",
        zIndex: isDragging || isResizing ? 10 : 1,
      }}
      onMouseDown={(e) => onMouseDown(e, "move")}
      onDoubleClick={() => onDoubleClick(appointment)}
      draggable={false}
    >
      <div
        className="absolute top-0 left-0 right-0 h-2 cursor-ns-resize hover:bg-white hover:bg-opacity-20"
        onMouseDown={(e) => onMouseDown(e, "resize-top")}
      />

      <div className="px-2 py-1 text-sm font-semibold pointer-events-none">
        {appointment.name || "Ongetiteld"}
      </div>

      <div
        className="absolute bottom-0 left-0 right-0 h-2 cursor-ns-resize hover:bg-white hover:bg-opacity-20"
        onMouseDown={(e) => onMouseDown(e, "resize-bottom")}
      />
    </div>
  );
}
