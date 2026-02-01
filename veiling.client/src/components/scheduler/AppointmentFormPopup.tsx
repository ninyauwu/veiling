import Popup from "../Popup";
import type {
  AppointmentData,
  AppointmentFormData,
  Kavel,
  Locatie,
} from "./AppointmentTypes";
import KavelOrderingList from "./KavelOrderingList";

// New component for the appointment form popup
interface AppointmentFormPopupProps {
  isOpen: boolean;
  onClose: () => void;
  editingAppointment: AppointmentData | null;
  appointments: AppointmentData[];
  kavels: Kavel[];
  locaties: Locatie[];
  formData: AppointmentFormData;
  onFormDataChange: (data: AppointmentFormData) => void;
  onSubmit: () => void;
}

export default function AppointmentFormPopup({
  isOpen,
  onClose,
  editingAppointment,
  appointments,
  kavels,
  locaties,
  formData,
  onFormDataChange,
  onSubmit,
}: AppointmentFormPopupProps) {
  const handleKavelToggle = (kavelId: number, checked: boolean) => {
    const newKavelIds = checked
      ? [...formData.kavelIds, kavelId]
      : formData.kavelIds.filter((id) => id !== kavelId);

    onFormDataChange({
      ...formData,
      kavelIds: newKavelIds,
    });
  };

  const handleReorder = (newKavelIds: number[]) => {
    onFormDataChange({
      ...formData,
      kavelIds: newKavelIds,
    });
  };

  const filteredKavels = formData.locationId
  ? kavels.filter(
      (kavel) => kavel.locatieId === formData.locationId
    )
  : [];


  if (!isOpen) return null;

  return (
    <Popup onClose={onClose}>
      <h2 className="text-2xl font-bold mb-4" style={{ color: "#7A1F3D" }}>
        {editingAppointment &&
        appointments.find((a) => a.id === editingAppointment.id)
          ? "Bewerk Veiling"
          : "Nieuwe Veiling"}
      </h2>

      <div className="mb-4">
        <label
          className="block text-sm font-semibold mb-2"
          style={{ color: "#000000" }}
          htmlFor="location-select"
        >
          Selecteer Locatie Van Veiling
        </label>

        <select
          id="location-select"
          value={formData.locationId ?? ""}
          onChange={(e) =>
            onFormDataChange({
              ...formData,
              locationId: Number(e.target.value),
              kavelIds: [],
            })
          }
          className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2"
          style={{
            borderColor: "#D9D9D9",
            color: "#000000",
          }}
        >
          <option value="">
            -- Kies een locatie --
          </option>

          {locaties.map((locatie) => (
            <option key={locatie.id} value={locatie.id}>
              {locatie.naam}
            </option>
          ))}
        </select>
      </div>


      {formData.locationId ? (
        <KavelOrderingList
          kavels={filteredKavels}
          selectedKavelIds={formData.kavelIds}
          onKavelToggle={handleKavelToggle}
          onReorder={handleReorder}
        />
      ) : (
        <p className="text-gray-400 italic">
          Selecteer eerst een locatie om kavels te zien
        </p>
      )}

      <div className="mb-4">
        <label
          className="block text-sm font-semibold mb-2"
          style={{ color: "#000000" }}
          htmlFor="veiling-name"
        >
          Veilingnaam <span className="text-sm font-normal">(optioneel)</span>
        </label>
        <input
          id="veiling-name"
          type="text"
          value={formData.name}
          onChange={(e) =>
            onFormDataChange({ ...formData, name: e.target.value })
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
          htmlFor="start-time"
        >
          Starttijd
        </label>
        <input
          id="start-time"
          type="time"
          value={formData.startTime}
          onChange={(e) =>
            onFormDataChange({
              ...formData,
              startTime: e.target.value,
            })
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
          htmlFor="end-time"
        >
          Eindtijd
        </label>
        <input
          id="end-time"
          type="time"
          value={formData.endTime}
          onChange={(e) =>
            onFormDataChange({ ...formData, endTime: e.target.value })
          }
          className="w-full px-3 py-2 border rounded focus:outline-none focus:ring-2"
          style={{
            borderColor: "#D9D9D9",
            color: "#000000",
          }}
        />
      </div>

      <button
        onClick={onSubmit}
        className="w-full py-3 rounded font-semibold transition-opacity hover:opacity-90 focus:outline-none focus:ring-2"
        style={{
          backgroundColor: "#7A1F3D",
          color: "#FFFFFF",
        }}
      >
        Veiling Opslaan
      </button>
    </Popup>
  );
}
