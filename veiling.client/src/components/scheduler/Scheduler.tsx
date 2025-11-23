import { useEffect, useRef, useState } from "react";
import Popup from "../Popup";
import Appointment from "./Appointment";
import type { AppointmentData, AppointmentFormData, Kavel } from "./AppointmentTypes";

const Scheduler: React.FC = () => {
  const [appointments, setAppointments] = useState<AppointmentData[]>([]);
  const [kavels, setKavels] = useState<Kavel[]>([]);
  const [editingAppointment, setEditingAppointment] =
    useState<AppointmentData | null>(null);
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const [formData, setFormData] = useState<AppointmentFormData>({
    startTime: "",
    endTime: "",
    name: "",
    kavelIds: [],
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
    const fetchKavels = async () => {
      try {
        const response = await fetch("/api/kavels");
        if (!response.ok) throw new Error("Failed to fetch kavels");
        const data = await response.json();
        setKavels(data);
      } catch (error) {
        console.error("Error fetching kavels:", error);
      }
    };

    fetchKavels();
  }, []);

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

  const getKavelNames = (kavelIds: number[]): string => {
    if (kavelIds.length === 0) return "";
    if (kavelIds.length === 1) {
      const kavel = kavels.find(k => k.id === kavelIds[0]);
      return kavel ? kavel.naam : "";
    }
    return `${kavelIds.length} kavels`;
  };

  const handleCellClick = (dayIndex: number, hour: number) => {
    const newAppointment: AppointmentData = {
      id: `apt-${Date.now()}`,
      dayIndex,
      startHour: hour,
      durationHours: 1,
      name: "",
      kavelIds: [],
    };

    setEditingAppointment(newAppointment);
    setFormData({
      startTime: formatTime(hour),
      endTime: formatTime(hour + 1),
      name: "",
      kavelIds: [],
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
      kavelIds: appointment.kavelIds,
    });
    setIsPopupOpen(true);
  };

  const handleFormSubmit = async () => {
    if (!editingAppointment) return;

    if (!formData.startTime || !formData.endTime) {
      alert("Vul start- en eindtijd in");
      return;
    }

    if (formData.kavelIds.length === 0) {
      alert("Selecteer minimaal 1 kavel");
      return;
    }

    const startHour = parseTime(formData.startTime);
    const endHour = parseTime(formData.endTime);
    const durationHours = endHour - startHour;

    if (durationHours <= 0) {
      alert("Eindtijd moet na starttijd zijn");
      return;
    }

    const startDate = new Date(weekDays[editingAppointment.dayIndex]);
    startDate.setHours(Math.floor(startHour), (startHour % 1) * 60, 0, 0);

    const endDate = new Date(startDate);
    endDate.setHours(Math.floor(endHour), (endHour % 1) * 60, 0, 0);

    const payload = {
      naam: formData.name || getKavelNames(formData.kavelIds) || "Ongetiteld",
      startTijd: startDate.toISOString(),
      endTijd: endDate.toISOString(),
      kavelIds: formData.kavelIds
    };

    try {
      const response = await fetch('/api/veilingen', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Veiling aanmaken mislukt: ${errorText}`);
      }

      const updatedAppointment: AppointmentData = {
        ...editingAppointment,
        startHour,
        durationHours,
        name: formData.name || getKavelNames(formData.kavelIds),
        kavelIds: formData.kavelIds,
      };

      setAppointments((prev) => {
        const existing = prev.find((apt) => apt.id === updatedAppointment.id);
        if (existing) {
          return prev.map((apt) =>
            apt.id === updatedAppointment.id ? updatedAppointment : apt
          );
        }
        return [...prev, updatedAppointment];
      });

      setIsPopupOpen(false);
      setEditingAppointment(null);
    } catch (error) {
      console.error('Error creating veiling:', error);
      alert('Fout bij opslaan veiling. Controleer of de server draait.');
    }
  };

  return (
    <div className="min-h-screen p-4" style={{ backgroundColor: "#D9D9D9" }}>
      <div className="max-w-7xl mx-auto">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-4 mb-4">
          <div className="lg:col-span-1 bg-white rounded-lg shadow-lg p-4 max-h-[600px] overflow-y-auto">
            <h2 className="text-xl font-bold mb-4" style={{ color: "#7A1F3D" }}>
              Beschikbare Kavels
            </h2>
            {kavels.length === 0 ? (
              <p className="text-gray-500">Geen kavels gevonden</p>
            ) : (
              <div className="space-y-3">
                {kavels.map((kavel) => (
                  <div
                    key={kavel.id}
                    className="p-3 border rounded hover:bg-gray-50 transition-colors"
                    style={{ borderColor: "#D9D9D9" }}
                  >
                    <div className="flex justify-between items-start mb-2">
                      <h3 className="font-semibold" style={{ color: "#000000" }}>
                        {kavel.naam}
                      </h3>
                      <div
                        className="w-6 h-6 rounded border border-gray-300"
                        style={{ backgroundColor: `#${kavel.kavelkleur}` }}
                      />
                    </div>
                    <p className="text-sm text-gray-600 mb-2 line-clamp-2">
                      {kavel.beschrijving}
                    </p>
                    <div className="text-xs space-y-1">
                      <div className="flex justify-between">
                        <span className="text-gray-500">Prijs:</span>
                        <span className="font-medium text-gray-900">
                          €{kavel.minimumPrijs.toFixed(2)} - €{kavel.maximumPrijs.toFixed(2)}
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-500">Containers:</span>
                        <span className="font-medium text-gray-900">{kavel.hoeveelheidContainers}</span>
                      </div>
                      {kavel.leverancier?.bedrijf?.bedrijfsnaam && (
                        <div className="flex justify-between">
                          <span className="text-gray-500">Leverancier:</span>
                          <span className="font-medium text-gray-900 text-xs">
                            {kavel.leverancier.bedrijf.bedrijfsnaam}
                          </span>
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>

          <div className="lg:col-span-3 bg-white rounded-lg shadow-lg overflow-hidden">
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
                Selecteer Kavels ({formData.kavelIds.length} geselecteerd)
              </label>
              <div
                className="border rounded p-3 max-h-48 overflow-y-auto space-y-2"
                style={{ borderColor: "#D9D9D9" }}
              >
                {kavels.map((kavel) => (
                  <label
                    key={kavel.id}
                    className="flex items-center gap-2 cursor-pointer hover:bg-gray-50 p-2 rounded"
                  >
                    <input
                      type="checkbox"
                      checked={formData.kavelIds.includes(kavel.id)}
                      onChange={(e) => {
                        setFormData((prev) => ({
                          ...prev,
                          kavelIds: e.target.checked
                            ? [...prev.kavelIds, kavel.id]
                            : prev.kavelIds.filter(id => id !== kavel.id)
                        }));
                      }}
                      className="w-4 h-4"
                      style={{ accentColor: "#7A1F3D" }}
                    />
                    <span className="text-sm flex-1">
                      {kavel.naam} - €{kavel.minimumPrijs.toFixed(2)}
                    </span>
                    <div
                      className="w-4 h-4 rounded border border-gray-300"
                      style={{ backgroundColor: `#${kavel.kavelkleur}` }}
                    />
                  </label>
                ))}
              </div>
            </div>

            <div className="mb-4">
              <label
                className="block text-sm font-semibold mb-2"
                style={{ color: "#000000" }}
              >
                Veilingnaam{" "}
                <span className="text-sm font-normal">(optioneel)</span>
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
              Veiling Opslaan
            </button>
          </div>
        </Popup>
      )}
    </div>
  );
};

export default Scheduler;