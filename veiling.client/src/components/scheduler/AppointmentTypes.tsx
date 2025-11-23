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

export interface AppointmentData {
  id: string;
  dayIndex: number;
  startHour: number;
  durationHours: number;
  name: string;
  kavelIds: number[];
}

export interface AppointmentProps {
  appointment: AppointmentData;
  onUpdate: (id: string, updates: Partial<AppointmentData>) => void;
  onDoubleClick: (appointment: AppointmentData) => void;
  hourHeight: number;
  columnWidth: number;
  gridLeft: number;
}

export interface AppointmentFormData {
  startTime: string;
  endTime: string;
  name: string;
  kavelIds: number[];
}