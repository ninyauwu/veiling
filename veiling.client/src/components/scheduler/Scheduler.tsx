import { useEffect, useRef, useState } from "react";
import Popup from "../Popup";
import Appointment from "./Appointment";
import type { AppointmentData, AppointmentFormData } from "./AppointmentTypes";

const Scheduler: React.FC = () => {
  const [appointments, setAppointments] = useState<AppointmentData[]>([]);
  const [editingAppointment, setEditingAppointment] =
    useState<AppointmentData | null>(null);
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const [formData, setFormData] = useState<AppointmentFormData>({
    startTime: "",
    endTime: "",
    name: "",
  });

  const gridRef = useRef<HTMLDivElement>(null);
  const [gridMetrics, setGridMetrics] = useState({ left: 0, columnWidth: 0 });

  const hourHeight = 60;
  const hours = Array.from({ length: 24 }, (_, i) => i);

  // Get current week
  const today = new Date();
  const weekStart = new Date(today);
  weekStart.setDate(today.getDate() - today.getDay());

  const weekDays = Array.from({ length: 7 }, (_, i) => {
    const day = new Date(weekStart);
    day.setDate(weekStart.getDate() + i);
    return day;
  });

  useEffect(() => {
    const updateGridMetrics = () => {
      if (gridRef.current) {
        const firstColumn = gridRef.current.children[1] as HTMLElement;
        if (firstColumn) {
          const rect = firstColumn.getBoundingClientRect();
          const columnWidth = rect.width;
          const left = rect.left;
          setGridMetrics({ left, columnWidth });
        }
      }
    };

    updateGridMetrics();
    window.addEventListener("resize", updateGridMetrics);

    // Use a timeout to ensure the layout is complete
    const timeout = setTimeout(updateGridMetrics, 100);

    return () => {
      window.removeEventListener("resize", updateGridMetrics);
      clearTimeout(timeout);
    };
  }, []);

  const formatTime = (hour: number): string => {
    const h = Math.floor(hour);
    const m = Math.round((hour - h) * 60);
    return `${h.toString().padStart(2, "0")}:${m.toString().padStart(2, "0")}`;
  };

  const parseTime = (timeString: string): number => {
    const [hours, minutes] = timeString.split(":").map(Number);
    return hours + minutes / 60;
  };

  const handleCellClick = (dayIndex: number, hour: number) => {
    const newAppointment: AppointmentData = {
      id: `apt-${Date.now()}`,
      dayIndex,
      startHour: hour,
      durationHours: 1,
      name: "",
    };

    setEditingAppointment(newAppointment);
    setFormData({
      startTime: formatTime(hour),
      endTime: formatTime(hour + 1),
      name: "",
    });
    setIsPopupOpen(true);
  };

  const handleAppointmentUpdate = (
    id: string,
    updates: Partial<AppointmentData>,
  ) => {
    setAppointments((prev) =>
      prev.map((apt) => (apt.id === id ? { ...apt, ...updates } : apt)),
    );
  };

  const handleAppointmentDoubleClick = (appointment: AppointmentData) => {
    setEditingAppointment(appointment);
    setFormData({
      startTime: formatTime(appointment.startHour),
      endTime: formatTime(appointment.startHour + appointment.durationHours),
      name: appointment.name,
    });
    setIsPopupOpen(true);
  };

  const handleFormSubmit = () => {
    if (!editingAppointment) return;

    if (!formData.startTime || !formData.endTime) {
      alert("Please fill in start and end times");
      return;
    }

    const startHour = parseTime(formData.startTime);
    const endHour = parseTime(formData.endTime);
    const durationHours = endHour - startHour;

    if (durationHours <= 0) {
      alert("End time must be after start time");
      return;
    }

    const updatedAppointment: AppointmentData = {
      ...editingAppointment,
      startHour,
      durationHours,
      name: formData.name,
    };

    setAppointments((prev) => {
      const existing = prev.find((apt) => apt.id === updatedAppointment.id);
      if (existing) {
        return prev.map((apt) =>
          apt.id === updatedAppointment.id ? updatedAppointment : apt,
        );
      }
      return [...prev, updatedAppointment];
    });

    setIsPopupOpen(false);
    setEditingAppointment(null);
  };

  return (
    <div className="min-h-screen p-4" style={{ backgroundColor: "#D9D9D9" }}>
      <div className="max-w-7xl mx-auto bg-white rounded-lg shadow-lg overflow-hidden">
        <div
          className="grid grid-cols-8 border-b-2"
          style={{ borderColor: "#7A1F3D" }}
        >
          <div className="p-4" style={{ backgroundColor: "#FFFFFF" }} />
          {weekDays.map((day, i) => {
            const isToday = day.toDateString() === today.toDateString();
            return (
              <div
                key={i}
                className="p-4 text-center font-semibold"
                style={{
                  backgroundColor: isToday ? "#7A1F3D" : "#FFFFFF",
                  color: isToday ? "#FFFFFF" : "#000000",
                }}
              >
                <div className="text-sm">
                  {day.toLocaleDateString("en-US", { weekday: "short" })}
                </div>
                <div
                  className="text-2xl"
                  style={{ color: isToday ? "#FFFFFF" : "#7A1F3D" }}
                >
                  {day.getDate()}
                </div>
              </div>
            );
          })}
        </div>

        <div className="grid grid-cols-8" ref={gridRef}>
          <div style={{ backgroundColor: "#FFFFFF" }}>
            {hours.map((hour) => (
              <div
                key={hour}
                className="border-t text-right pr-2 text-sm"
                style={{
                  height: `${hourHeight}px`,
                  color: "#000000",
                  borderColor: "#D9D9D9",
                }}
              >
                {hour.toString().padStart(2, "0")}:00
              </div>
            ))}
          </div>
          {weekDays.map((_, dayIndex) => (
            <div
              key={dayIndex}
              className="relative border-l"
              style={{ borderColor: "#D9D9D9" }}
            >
              {hours.map((hour) => (
                <div
                  key={hour}
                  className="border-t cursor-pointer hover:bg-opacity-50"
                  style={{
                    height: `${hourHeight}px`,
                    borderColor: "#D9D9D9",
                    backgroundColor: "#FFFFFF",
                  }}
                  onClick={() => handleCellClick(dayIndex, hour)}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.backgroundColor = "#D9D9D9";
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.backgroundColor = "#FFFFFF";
                  }}
                />
              ))}
              {appointments
                .filter((apt) => apt.dayIndex === dayIndex)
                .map((apt) => (
                  <Appointment
                    key={apt.id}
                    appointment={apt}
                    onUpdate={handleAppointmentUpdate}
                    onDoubleClick={handleAppointmentDoubleClick}
                    hourHeight={hourHeight}
                    columnWidth={gridMetrics.columnWidth}
                    gridLeft={gridMetrics.left}
                  />
                ))}
            </div>
          ))}
        </div>
      </div>
      {isPopupOpen && (
        <Popup onClose={() => setIsPopupOpen(false)}>
          <h2 className="text-2xl font-bold mb-4" style={{ color: "#7A1F3D" }}>
            {editingAppointment &&
            appointments.find((a) => a.id === editingAppointment.id)
              ? "Bewerk Veiling"
              : "Nieuwe Veiling"}
          </h2>
          <div>
            <div className="mb-4">
              <label
                className="block text-sm font-semibold mb-2"
                style={{ color: "#000000" }}
              >
                Appointment Name{" "}
                <span className="text-sm font-normal">(optional)</span>
              </label>
              <input
                type="text"
                value={formData.name}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, name: e.target.value }))
                }
                className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2"
                style={{
                  borderColor: "#D9D9D9",
                  color: "#000000",
                }}
                placeholder="Vul veilingnaam in"
              />
            </div>

            <div className="mb-4">
              <label
                className="block text-sm font-semibold mb-2"
                style={{ color: "#000000" }}
              >
                Start Time
              </label>
              <input
                type="time"
                value={formData.startTime}
                onChange={(e) =>
                  setFormData((prev) => ({
                    ...prev,
                    startTime: e.target.value,
                  }))
                }
                className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2"
                style={{
                  borderColor: "#D9D9D9",
                  color: "#000000",
                }}
              />
            </div>

            <div className="mb-6">
              <label
                className="block text-sm font-semibold mb-2"
                style={{ color: "#000000" }}
              >
                End Time
              </label>
              <input
                type="time"
                value={formData.endTime}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, endTime: e.target.value }))
                }
                className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2"
                style={{
                  borderColor: "#D9D9D9",
                  color: "#000000",
                }}
              />
            </div>

            <button
              onClick={handleFormSubmit}
              className="w-full py-3 rounded font-semibold transition-opacity hover:opacity-90"
              style={{
                backgroundColor: "#7A1F3D",
                color: "#FFFFFF",
              }}
            >
              Save Appointment
            </button>
          </div>
        </Popup>
      )}
    </div>
  );
};

export default Scheduler;
