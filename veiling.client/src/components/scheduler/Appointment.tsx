import { useEffect, useRef, useState } from "react";
import type { AppointmentProps } from "./AppointmentTypes";

export default function Appointment({
  appointment,
  onUpdate,
  onDoubleClick,
  hourHeight,
  columnWidth,
  gridLeft,
}: AppointmentProps) {
  const [isDragging, setIsDragging] = useState(false);
  const [isResizing, setIsResizing] = useState<"top" | "bottom" | null>(null);
  const dragStartRef = useRef({
    x: 0,
    y: 0,
    startHour: 0,
    duration: 0,
    dayIndex: 0,
  });

  const snapToQuarterHour = (hour: number): number => {
    return Math.round(hour * 4) / 4;
  };

  const snapDurationToQuarterHour = (
    baseDuration: number,
    delta: number,
  ): number => {
    //const newDuration = baseDuration + delta;
    const quarterHourDelta = Math.round(delta * 4) / 4;
    return Math.max(0.25, baseDuration + quarterHourDelta);
  };

  const snapStartHourToQuarterHour = (
    baseStartHour: number,
    delta: number,
  ): number => {
    const quarterHourDelta = Math.round(delta * 4) / 4;
    return Math.max(0, Math.min(23.75, baseStartHour + quarterHourDelta));
  };

  const getDayIndexFromX = (clientX: number): number => {
    if (columnWidth === 0 || gridLeft === 0) {
      return dragStartRef.current.dayIndex;
    }
    const relativeX = clientX - gridLeft;
    const dayIndex = Math.floor(relativeX / columnWidth);
    return Math.max(0, Math.min(6, dayIndex));
  };

  const handleMouseDown = (
    e: React.MouseEvent,
    type: "move" | "resize-top" | "resize-bottom",
  ) => {
    e.preventDefault();
    e.stopPropagation();

    if (type === "move") {
      setIsDragging(true);
      dragStartRef.current = {
        x: e.clientX,
        y: e.clientY,
        startHour: appointment.startHour,
        duration: appointment.durationHours,
        dayIndex: appointment.dayIndex,
      };
    } else if (type === "resize-top") {
      setIsResizing("top");
      dragStartRef.current = {
        x: e.clientX,
        y: e.clientY,
        startHour: appointment.startHour,
        duration: appointment.durationHours,
        dayIndex: appointment.dayIndex,
      };
    } else {
      setIsResizing("bottom");
      dragStartRef.current = {
        x: e.clientX,
        y: e.clientY,
        startHour: appointment.startHour,
        duration: appointment.durationHours,
        dayIndex: appointment.dayIndex,
      };
    }
  };

  useEffect(() => {
    const handleMouseMove = (e: MouseEvent) => {
      if (!isDragging && !isResizing) return;

      const deltaY = e.clientY - dragStartRef.current.y;
      const deltaHours = deltaY / hourHeight;

      if (isDragging) {
        // Handle day switching - only if grid metrics are available
        const newDayIndex = getDayIndexFromX(e.clientX);

        // Snap start hour to nearest 15-minute mark (absolute snapping for moving)
        const rawNewStartHour = dragStartRef.current.startHour + deltaHours;
        const newStartHour = Math.max(
          0,
          Math.min(23.75, snapToQuarterHour(rawNewStartHour)),
        );

        onUpdate(appointment.id, {
          startHour: newStartHour,
          dayIndex: newDayIndex,
        });
      } else if (isResizing === "top") {
        // Resize from top - snap with relative increments like duration
        const newStartHour = snapStartHourToQuarterHour(
          dragStartRef.current.startHour,
          deltaHours,
        );
        const deltaStart = newStartHour - dragStartRef.current.startHour;
        const newDuration = Math.max(
          0.25,
          dragStartRef.current.duration - deltaStart,
        );

        if (newStartHour + newDuration <= 24) {
          onUpdate(appointment.id, {
            startHour: newStartHour,
            durationHours: newDuration,
          });
        }
      } else if (isResizing === "bottom") {
        // Resize from bottom - snap duration based on base duration
        const newDuration = snapDurationToQuarterHour(
          dragStartRef.current.duration,
          deltaHours,
        );

        if (dragStartRef.current.startHour + newDuration <= 24) {
          onUpdate(appointment.id, { durationHours: newDuration });
        }
      }
    };

    const handleMouseUp = () => {
      setIsDragging(false);
      setIsResizing(null);
    };

    if (isDragging || isResizing) {
      document.addEventListener("mousemove", handleMouseMove);
      document.addEventListener("mouseup", handleMouseUp);
    }

    return () => {
      document.removeEventListener("mousemove", handleMouseMove);
      document.removeEventListener("mouseup", handleMouseUp);
    };
  }, [
    isDragging,
    isResizing,
    appointment.id,
    hourHeight,
    columnWidth,
    gridLeft,
    onUpdate,
  ]);

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
      onMouseDown={(e) => handleMouseDown(e, "move")}
      onDoubleClick={() => onDoubleClick(appointment)}
      draggable={false}
    >
      <div
        className="absolute top-0 left-0 right-0 h-2 cursor-ns-resize hover:bg-white hover:bg-opacity-20"
        onMouseDown={(e) => handleMouseDown(e, "resize-top")}
      />

      <div className="px-2 py-1 text-sm font-semibold pointer-events-none">
        {appointment.name || "Ongetiteld"}
      </div>

      <div
        className="absolute bottom-0 left-0 right-0 h-2 cursor-ns-resize hover:bg-white hover:bg-opacity-20"
        onMouseDown={(e) => handleMouseDown(e, "resize-bottom")}
      />
    </div>
  );
}
