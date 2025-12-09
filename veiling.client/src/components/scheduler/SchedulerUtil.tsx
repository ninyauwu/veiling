export const getCurrentMinuteTime = (): Date => {
  const now = new Date();
  now.setSeconds(0, 0);
  return now;
};

export const formatTime = (hour: number): string => {
  const h = Math.floor(hour);
  const m = Math.round((hour - h) * 60);
  return `${h.toString().padStart(2, "0")}:${m.toString().padStart(2, "0")}`;
};

export const parseTime = (timeString: string): number => {
  const [hours, minutes] = timeString.split(":").map(Number);
  return hours + minutes / 60;
};

export const getWeekDays = (weekOffset: number): Date[] => {
  const today = new Date();
  const weekStart = new Date(today);
  weekStart.setDate(today.getDate() - today.getDay() + weekOffset * 7);

  return Array.from({ length: 7 }, (_, i) => {
    const day = new Date(weekStart);
    day.setDate(weekStart.getDate() + i);
    return day;
  });
};

export const getNextValidStartTime = (date: Date, hour: number): number => {
  const cellDate = new Date(date);
  cellDate.setHours(hour, 0, 0, 0);

  const now = getCurrentMinuteTime();

  if (cellDate < now) {
    const currentHour = now.getHours() + now.getMinutes() / 60;
    return Math.ceil(currentHour * 4) / 4;
  }

  return hour;
};

export const isPastHourBlock = (date: Date, hour: number): boolean => {
  const cellDate = new Date(date);
  cellDate.setHours(hour + 1, 0, 0, 0);

  const now = getCurrentMinuteTime();

  return cellDate <= now;
};

export const getMinimumValidTime = (date: Date): number | null => {
  const dayStart = new Date(date);
  dayStart.setHours(0, 0, 0, 0);

  const now = getCurrentMinuteTime();

  if (dayStart.toDateString() !== now.toDateString()) {
    if (dayStart < now) {
      return null;
    }
    return 0;
  }

  const currentHour = now.getHours() + now.getMinutes() / 60;
  return currentHour;
};

export const snapToValidTime = (date: Date, hour: number): number => {
  const minimumTime = getMinimumValidTime(date);

  if (minimumTime === null) {
    return 0;
  }

  if (hour < minimumTime) {
    return Math.ceil(minimumTime * 4) / 4;
  }

  return hour;
};

export const isAppointmentInPast = (
  date: Date,
  startHour: number,
  durationHours: number,
): boolean => {
  const endDate = new Date(date);
  endDate.setHours(
    Math.floor(startHour + durationHours),
    ((startHour + durationHours) % 1) * 60,
    0,
    0,
  );

  const now = getCurrentMinuteTime();

  return endDate < now;
};

export const isSameDate = (date1: Date, date2: Date): boolean => {
  return (
    date1.getFullYear() === date2.getFullYear() &&
    date1.getMonth() === date2.getMonth() &&
    date1.getDate() === date2.getDate()
  );
};
