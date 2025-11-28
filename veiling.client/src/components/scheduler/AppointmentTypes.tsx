export interface AppointmentData {
  id: string;
  startHour: number;
  date: Date;
  durationHours: number;
  name: string;
  kavelIds: number[];
}

export interface AppointmentFormData {
  startTime: string;
  endTime: string;
  name: string;
  kavelIds: number[];
}

export interface Kavel {
  id: number;
  naam: string;
  beschrijving: string;
  minimumPrijs: number;
  maximumPrijs: number;
  hoeveelheidContainers: number;
  kavelkleur: string;
  leverancier?: {
    bedrijf?: {
      bedrijfsnaam: string;
    };
  };
}

export interface AppointmentProps {
  appointment: AppointmentData;
  onMouseDown: (
    e: React.MouseEvent,
    type: "move" | "resize-top" | "resize-bottom",
  ) => void;
  onDoubleClick: (appointment: AppointmentData) => void;
  hourHeight: number;
  isDragging: boolean;
  isResizing: boolean;
  isDisabled: boolean;
}

export interface DragState {
  appointmentId: string;
  type: "move" | "resize-top" | "resize-bottom";
  startX: number;
  startY: number;
  originalStartHour: number;
  originalDuration: number;
  originalDayIndex: number;
}
