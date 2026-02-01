import { useEffect, useRef, useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";
import Appointment from "./Appointment";
import type {
  AppointmentData,
  AppointmentFormData,
  DragState,
  Kavel,
  Locatie,
} from "./AppointmentTypes";
import AppointmentFormPopup from "./AppointmentFormPopup";
import {
  formatTime,
  parseTime,
  getCurrentMinuteTime,
  getWeekDays,
  getNextValidStartTime,
  isPastHourBlock,
  snapToValidTime,
  isAppointmentInPast,
  isSameDate,
} from "./SchedulerUtil";
import { authFetch } from "../../utils/AuthFetch";

export default function Scheduler() {
  const [appointments, setAppointments] = useState<AppointmentData[]>([]);
  const [kavels, setKavels] = useState<Kavel[]>([]);
  const [editingAppointment, setEditingAppointment] =
    useState<AppointmentData | null>(null);
  const [isPopupOpen, setIsPopupOpen] = useState(false);
  const [locations, setLocations] = useState<Locatie[]>([]);
  const [formData, setFormData] = useState<AppointmentFormData>({
    startTime: "",
    endTime: "",
    name: "",
    kavelIds: [],
    locationId: null,
  });
  const [dragState, setDragState] = useState<DragState | null>(null);
  const [weekOffset, setWeekOffset] = useState(0);

  const gridRef = useRef<HTMLDivElement>(null);
  const [gridMetrics, setGridMetrics] = useState({ left: 0, columnWidth: 0 });

  const hourHeight = 60;
  const hours = Array.from({ length: 24 }, (_, i) => i);

  const today = new Date();
  const weekDays = getWeekDays(weekOffset);

  useEffect(() => {
    const fetchKavels = async () => {
      try {
        const response = await authFetch("/api/kavels/approved");
        if (!response.ok) throw new Error("Failed to fetch kavels");
        const data = await response.json();
        setKavels(data);
      } catch (error) {
        console.error("Error fetching kavels:", error);
        setKavels([
          {
            id: 1,
            naam: "Kavel A",
            beschrijving: "Test kavel",
            locatieId: 1,
            minimumPrijs: 100,
            maximumPrijs: 200,
            hoeveelheidContainers: 5,
            kavelkleur: "FF5733",
          },
        ]);
      }
    };

    fetchKavels();
  }, []);

  useEffect(() => {
  const fetchLocations = async () => {
    try {
      const response = await authFetch("/api/locaties");
      if (!response.ok) throw new Error("Failed to fetch locaties");

      const data = await response.json();
      setLocations(data);
    } catch (error) {
      console.error("Error fetching locaties:", error);
    }
  };

  fetchLocations();
}, []);


  useEffect(() => {
    const fetchVeilingen = async () => {
      try {
        const response = await authFetch("/api/veilingen");
        if (!response.ok) throw new Error("Failed to fetch veilingen");
        const data = await response.json();

        const fetchedAppointments: AppointmentData[] = data.map((veiling: any) => {
          // UTC tijden
          const startDateString = veiling.startTijd.endsWith('Z') ? veiling.startTijd : veiling.startTijd + 'Z';
          const endDateString = veiling.endTijd.endsWith('Z') ? veiling.endTijd : veiling.endTijd + 'Z';

          const startDate = new Date(startDateString);
          const endDate = new Date(endDateString);

          const startHour = startDate.getHours() + startDate.getMinutes() / 60;
          const endHour = endDate.getHours() + endDate.getMinutes() / 60;
          const durationHours = endHour - startHour;

          return {
            id: `veiling-${veiling.id}`,
            date: startDate,
            startHour: startHour,
            durationHours: durationHours,
            name: veiling.naam,
            kavelIds: veiling.kavels?.map((k: any) => k.id) || [],
          };
        });

        setAppointments(fetchedAppointments);
      } catch (error) {
        console.error("Error fetching veilingen:", error);
      }
    };

    fetchVeilingen();
  }, []);

  // refresh veilingen wanneer de week verandert
  useEffect(() => {
    const fetchVeilingenForWeek = async () => {
      try {
        const response = await authFetch("/api/veilingen");
        if (!response.ok) throw new Error("Failed to fetch veilingen");
        const data = await response.json();

        const fetchedAppointments: AppointmentData[] = data.map((veiling: any) => {
          // UTC tijden
          const startDateString = veiling.startTijd.endsWith('Z') ? veiling.startTijd : veiling.startTijd + 'Z';
          const endDateString = veiling.endTijd.endsWith('Z') ? veiling.endTijd : veiling.endTijd + 'Z';

          const startDate = new Date(startDateString);
          const endDate = new Date(endDateString);

          const startHour = startDate.getHours() + startDate.getMinutes() / 60;
          const endHour = endDate.getHours() + endDate.getMinutes() / 60;
          const durationHours = endHour - startHour;

          return {
            id: `veiling-${veiling.id}`,
            date: startDate,
            startHour: startHour,
            durationHours: durationHours,
            name: veiling.naam,
            kavelIds: veiling.kavels?.map((k: any) => k.id) || [],
          };
        });

        setAppointments(fetchedAppointments);
      } catch (error) {
        console.error("Error fetching veilingen:", error);
      }
    };

    fetchVeilingenForWeek();
  }, [weekOffset]);

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

  useEffect(() => {
    if (!dragState) return;

    const snapToQuarterHour = (hour: number): number => {
      return Math.round(hour * 4) / 4;
    };

    const getDayIndexFromX = (clientX: number): number => {
      if (gridMetrics.columnWidth === 0 || gridMetrics.left === 0) {
        return dragState.originalDayIndex;
      }
      const relativeX = clientX - gridMetrics.left;
      const dayIndex = Math.floor(relativeX / gridMetrics.columnWidth);
      return Math.max(0, Math.min(6, dayIndex));
    };

    const handleMouseMove = (e: MouseEvent) => {
      const deltaY = e.clientY - dragState.startY;
      const deltaHours = deltaY / hourHeight;

      if (dragState.type === "move") {
        const newDayIndex = getDayIndexFromX(e.clientX);
        const rawNewStartHour = dragState.originalStartHour + deltaHours;
        const snappedHour = Math.max(
          0,
          Math.min(23.75, snapToQuarterHour(rawNewStartHour)),
        );
        const newStartHour = snapToValidTime(
          weekDays[newDayIndex],
          snappedHour,
        );

        setAppointments((prev) =>
          prev.map((apt) =>
            apt.id === dragState.appointmentId
              ? { ...apt, startHour: newStartHour, date: weekDays[newDayIndex] }
              : apt,
          ),
        );
      } else if (dragState.type === "resize-top") {
        const quarterHourDelta = Math.round(deltaHours * 4) / 4;
        const rawNewStartHour = dragState.originalStartHour + quarterHourDelta;
        const clampedHour = Math.max(0, Math.min(23.75, rawNewStartHour));
        const originalDate = appointments.find(
          (apt) => apt.id === dragState.appointmentId,
        )?.date;

        if (!originalDate) return;

        const newStartHour = snapToValidTime(originalDate, clampedHour);
        const deltaStart = newStartHour - dragState.originalStartHour;
        const newDuration = Math.max(
          0.25,
          dragState.originalDuration - deltaStart,
        );

        if (newStartHour + newDuration <= 24) {
          setAppointments((prev) =>
            prev.map((apt) =>
              apt.id === dragState.appointmentId
                ? {
                    ...apt,
                    startHour: newStartHour,
                    durationHours: newDuration,
                  }
                : apt,
            ),
          );
        }
      } else if (dragState.type === "resize-bottom") {
        const quarterHourDelta = Math.round(deltaHours * 4) / 4;
        const newDuration = Math.max(
          0.25,
          dragState.originalDuration + quarterHourDelta,
        );

        if (dragState.originalStartHour + newDuration <= 24) {
          setAppointments((prev) =>
            prev.map((apt) =>
              apt.id === dragState.appointmentId
                ? { ...apt, durationHours: newDuration }
                : apt,
            ),
          );
        }
      }
    };

    const handleMouseUp = () => {
      // als appointment verplaatst, update database
      if (dragState) {
        const updatedAppointment = appointments.find(
          (apt) => apt.id === dragState.appointmentId
        );
        if (updatedAppointment) {
          updateVeilingInDatabase(updatedAppointment);
        }
      }

      setDragState(null);
    };

    document.addEventListener("mousemove", handleMouseMove);
    document.addEventListener("mouseup", handleMouseUp);

    return () => {
      document.removeEventListener("mousemove", handleMouseMove);
      document.removeEventListener("mouseup", handleMouseUp);
    };
  }, [dragState, hourHeight, gridMetrics, weekDays, appointments]);

  const getKavelNames = (kavelIds: number[]): string => {
    if (kavelIds.length === 0) return "";
    if (kavelIds.length === 1) {
      const kavel = kavels.find((k) => k.id === kavelIds[0]);
      return kavel ? kavel.naam : "";
    }
    return `${kavelIds.length} kavels`;
  };

  const handleCellClick = (dayIndex: number, hour: number) => {
    const cellDate = weekDays[dayIndex];

    if (isPastHourBlock(cellDate, hour)) {
      return;
    }

    const validStartHour = getNextValidStartTime(cellDate, hour);

    const newAppointment: AppointmentData = {
      id: `apt-${Date.now()}`,
      date: cellDate,
      startHour: validStartHour,
      durationHours: 1,
      name: "",
      kavelIds: [],
      locationId: 0,
    };

    setEditingAppointment(newAppointment);
    setFormData({
      startTime: formatTime(validStartHour),
      endTime: formatTime(validStartHour + 1),
      name: "",
      kavelIds: [],
      locationId: 0,
    });
    setIsPopupOpen(true);
  };

  const handleAppointmentMouseDown = (
    e: React.MouseEvent,
    appointment: AppointmentData,
    type: "move" | "resize-top" | "resize-bottom",
  ) => {
    e.preventDefault();
    e.stopPropagation();

    const dayIndex = weekDays.findIndex((day) =>
      isSameDate(day, appointment.date),
    );

    setDragState({
      appointmentId: appointment.id,
      type,
      startX: e.clientX,
      startY: e.clientY,
      originalStartHour: appointment.startHour,
      originalDuration: appointment.durationHours,
      originalDayIndex: dayIndex,
    });
  };

  const handleAppointmentDoubleClick = (appointment: AppointmentData) => {
    setEditingAppointment(appointment);
    setFormData({
      startTime: formatTime(appointment.startHour),
      endTime: formatTime(appointment.startHour + appointment.durationHours),
      name: appointment.name,
      kavelIds: appointment.kavelIds,
      locationId: appointment.locationId || null,
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

    const startDate = new Date(editingAppointment.date);
    startDate.setHours(Math.floor(startHour), (startHour % 1) * 60, 0, 0);

    const endDate = new Date(startDate);
    endDate.setHours(Math.floor(endHour), (endHour % 1) * 60, 0, 0);

    const now = getCurrentMinuteTime();

    if (startDate < now) {
      alert("Kan geen veiling in het verleden aanmaken");
      return;
    }

    const payload = {
      Naam: formData.name || getKavelNames(formData.kavelIds) || "Ongetiteld",
      StartTijd: startDate.toISOString(),
      EndTijd: endDate.toISOString(),
      KavelIds: formData.kavelIds,
    };

    try {
      const response = await authFetch("/api/veilingen", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
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
            apt.id === updatedAppointment.id ? updatedAppointment : apt,
          );
        }
        return [...prev, updatedAppointment];
      });

      setIsPopupOpen(false);
      setEditingAppointment(null);
    } catch (error) {
      console.error("Error creating veiling:", error);
      alert("Fout bij opslaan veiling. Controleer of de server draait.");
    }
  };

  const updateVeilingInDatabase = async (appointment: AppointmentData) => {
    if (!appointment.id.startsWith("veiling-")) {
      return;
    }

    const veilingId = parseInt(appointment.id.split("-")[1]);

    const startDate = new Date(appointment.date);
    startDate.setHours(
      Math.floor(appointment.startHour),
      (appointment.startHour % 1) * 60,
      0,
      0
    );

    const endDate = new Date(startDate);
    endDate.setHours(
      Math.floor(appointment.startHour + appointment.durationHours),
      ((appointment.startHour + appointment.durationHours) % 1) * 60,
      0,
      0
    );

    const payload = {
      Id: veilingId,
      Naam: appointment.name,
      StartTijd: startDate.toISOString(),
      EndTijd: endDate.toISOString(),
      Klokduur: 1.0,
      GeldPerTickCode: 0.01,
      VeilingmeesterId: null,
      LocatieId: null,
    };

    try {
      const response = await authFetch(`/api/veilingen/${veilingId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Veiling updaten mislukt: ${errorText}`);
      }

      console.log("Veiling succesvol geüpdatet in database");
    } catch (error) {
      console.error("Error updating veiling:", error);
      alert("Fout bij updaten veiling in database");
    }
  };

  const handlePreviousWeek = () => {
    setWeekOffset((prev) => prev - 1);
  };

  const handleNextWeek = () => {
    setWeekOffset((prev) => prev + 1);
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
              <div
                className="space-y-3"
                role="list"
                aria-label="Beschikbare kavels"
              >
                {kavels.map((kavel) => (
                  <div
                    key={kavel.id}
                    className="p-3 border rounded hover:bg-gray-50 transition-colors"
                    style={{ borderColor: "#D9D9D9" }}
                    role="listitem"
                  >
                    <div className="flex justify-between items-start mb-2">
                      <h3
                        className="font-semibold"
                        style={{ color: "#000000" }}
                      >
                        {kavel.naam}
                      </h3>
                      <div
                        className="w-6 h-6 rounded border border-gray-300"
                        style={{ backgroundColor: `#${kavel.kavelkleur}` }}
                        aria-hidden="true"
                      />
                    </div>
                    <p className="text-sm text-gray-600 mb-2 line-clamp-2">
                      {kavel.beschrijving}
                    </p>
                    <div className="text-xs space-y-1">
                      <div className="flex justify-between">
                        <span className="text-gray-500">Prijs:</span>
                        <span className="font-medium text-gray-900">
                          €{kavel.minimumPrijs.toFixed(2)} - €
                          {kavel.maximumPrijs.toFixed(2)}
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-500">Containers:</span>
                        <span className="font-medium text-gray-900">
                          {kavel.hoeveelheidContainers}
                        </span>
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
              role="row"
            >
              <div
                className="p-4 flex items-center justify-center"
                style={{ backgroundColor: "#FFFFFF" }}
              >
                <div className="flex gap-2">
                  <button
                    onClick={handlePreviousWeek}
                    className="p-2 rounded hover:bg-gray-100 transition-colors"
                    aria-label="Vorige week"
                    style={{ color: "#7A1F3D" }}
                  >
                    <ChevronLeft size={24} />
                  </button>
                  <button
                    onClick={handleNextWeek}
                    className="p-2 rounded hover:bg-gray-100 transition-colors"
                    aria-label="Volgende week"
                    style={{ color: "#7A1F3D" }}
                  >
                    <ChevronRight size={24} />
                  </button>
                </div>
              </div>
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
                    role="columnheader"
                    aria-label={`${day.toLocaleDateString("nl-NL", { weekday: "long" })} ${day.getDate()} ${day.toLocaleDateString("nl-NL", { month: "long" })}`}
                  >
                    <div className="text-sm">
                      {day.toLocaleDateString("nl-NL", { weekday: "short" })}
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

            <div className="grid grid-cols-8" ref={gridRef} role="grid">
              <div style={{ backgroundColor: "#FFFFFF" }} role="rowheader">
                {hours.map((hour) => (
                  <div
                    key={hour}
                    className="border-t text-right pr-2 text-sm"
                    style={{
                      height: `${hourHeight}px`,
                      color: "#000000",
                      borderColor: "#D9D9D9",
                    }}
                    role="columnheader"
                    aria-label={`${hour.toString().padStart(2, "0")}:00`}
                  >
                    {hour.toString().padStart(2, "0")}:00
                  </div>
                ))}
              </div>
              {weekDays.map((day, dayIndex) => (
                <div
                  key={dayIndex}
                  className="relative border-l"
                  style={{ borderColor: "#D9D9D9" }}
                  role="gridcell"
                >
                  {hours.map((hour) => {
                    const isPast = isPastHourBlock(day, hour);
                    return (
                      <div
                        key={hour}
                        className={`border-t ${isPast ? "cursor-not-allowed" : "cursor-pointer hover:bg-opacity-50"}`}
                        style={{
                          height: `${hourHeight}px`,
                          borderColor: "#D9D9D9",
                          backgroundColor: isPast ? "#F5F5F5" : "#FFFFFF",
                        }}
                        onClick={() => handleCellClick(dayIndex, hour)}
                        onMouseEnter={(e) => {
                          if (!isPast) {
                            e.currentTarget.style.backgroundColor = "#D9D9D9";
                          }
                        }}
                        onMouseLeave={(e) => {
                          if (!isPast) {
                            e.currentTarget.style.backgroundColor = "#FFFFFF";
                          }
                        }}
                        role="button"
                        tabIndex={isPast ? -1 : 0}
                        aria-label={`Maak afspraak op ${day.toLocaleDateString("nl-NL")} om ${hour.toString().padStart(2, "0")}:00`}
                        aria-disabled={isPast}
                        onKeyDown={(e) => {
                          if (!isPast && (e.key === "Enter" || e.key === " ")) {
                            e.preventDefault();
                            handleCellClick(dayIndex, hour);
                          }
                        }}
                      />
                    );
                  })}
                  {appointments
                    .filter((apt) => isSameDate(apt.date, day))
                    .map((apt) => {
                      const isPast = isAppointmentInPast(
                        apt.date,
                        apt.startHour,
                        apt.durationHours,
                      );
                      return (
                        <Appointment
                          key={apt.id}
                          appointment={apt}
                          onMouseDown={(e, type) =>
                            handleAppointmentMouseDown(e, apt, type)
                          }
                          onDoubleClick={handleAppointmentDoubleClick}
                          hourHeight={hourHeight}
                          isDragging={
                            dragState?.appointmentId === apt.id &&
                            dragState.type === "move"
                          }
                          isResizing={
                            dragState?.appointmentId === apt.id &&
                            dragState.type !== "move"
                          }
                          isDisabled={isPast}
                        />
                      );
                    })}
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      <AppointmentFormPopup
        isOpen={isPopupOpen}
        onClose={() => setIsPopupOpen(false)}
        editingAppointment={editingAppointment}
        appointments={appointments}
        kavels={kavels}
        locaties={locations}
        formData={formData}
        onFormDataChange={setFormData}
        onSubmit={handleFormSubmit}
      />
    </div>
  );
}
