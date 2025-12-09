import { useState } from "react";
import Popup from "../Popup";
import type { Kavel } from "./AppointmentTypes";

// New component for the draggable kavel list with ordering
interface KavelOrderingListProps {
  kavels: Kavel[];
  selectedKavelIds: number[];
  onKavelToggle: (kavelId: number, checked: boolean) => void;
  onReorder: (newOrder: number[]) => void;
}

export default function KavelOrderingList({
  kavels,
  selectedKavelIds,
  onKavelToggle,
  onReorder,
}: KavelOrderingListProps) {
  const [draggedIndex, setDraggedIndex] = useState<number | null>(null);
  const [dragOverIndex, setDragOverIndex] = useState<number | null>(null);
  const [isKavelSelectPopupOpen, setIsKavelSelectPopupOpen] = useState(false);

  const handleDragStart = (index: number) => {
    setDraggedIndex(index);
  };

  const handleDragOver = (e: React.DragEvent, index: number) => {
    e.preventDefault();
    setDragOverIndex(index);
  };

  const handleDragEnd = () => {
    if (
      draggedIndex !== null &&
      dragOverIndex !== null &&
      draggedIndex !== dragOverIndex
    ) {
      const newKavelIds = [...selectedKavelIds];
      const draggedItem = newKavelIds[draggedIndex];
      newKavelIds.splice(draggedIndex, 1);
      newKavelIds.splice(dragOverIndex, 0, draggedItem);
      onReorder(newKavelIds);
    }
    setDraggedIndex(null);
    setDragOverIndex(null);
  };

  const moveKavelUp = (index: number) => {
    if (index === 0) return;
    const newKavelIds = [...selectedKavelIds];
    const temp = newKavelIds[index];
    newKavelIds[index] = newKavelIds[index - 1];
    newKavelIds[index - 1] = temp;
    onReorder(newKavelIds);
  };

  const moveKavelDown = (index: number) => {
    if (index === selectedKavelIds.length - 1) return;
    const newKavelIds = [...selectedKavelIds];
    const temp = newKavelIds[index];
    newKavelIds[index] = newKavelIds[index + 1];
    newKavelIds[index + 1] = temp;
    onReorder(newKavelIds);
  };

  //const selectedKavels = selectedKavelIds
  //  .map(id => kavels.find(k => k.id === id))
  //  .filter(Boolean) as Kavel[];

  return (
    <>
      <div className="mb-4">
        <label
          className="block text-sm font-semibold mb-2"
          style={{ color: "#000000" }}
        >
          Selecteer Kavels ({selectedKavelIds.length} geselecteerd)
        </label>

        <button
          type="button"
          onClick={() => setIsKavelSelectPopupOpen(true)}
          className="w-full py-2 px-3 border border-dashed rounded mb-3 transition-colors hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-opacity-50"
          style={{
            borderColor: "#7A1F3D",
            color: "#7A1F3D",
          }}
        >
          + Voeg kavel toe
        </button>

        {selectedKavelIds.length > 0 && (
          <div>
            <label
              className="block text-sm font-semibold mb-2"
              style={{ color: "#000000" }}
            >
              Volgorde van Kavels
            </label>
            <div
              className="border rounded p-3 space-y-2"
              style={{ borderColor: "#D9D9D9", backgroundColor: "#f9f9f9" }}
              role="list"
              aria-label="Geselecteerde kavels in volgorde"
            >
              {selectedKavelIds.map((kavelId, index) => {
                const kavel = kavels.find((k) => k.id === kavelId);
                if (!kavel) return null;

                const isDragging = draggedIndex === index;
                const isDragOver = dragOverIndex === index;

                return (
                  <div
                    key={`selected-${kavelId}-${index}`}
                    draggable
                    onDragStart={() => handleDragStart(index)}
                    onDragOver={(e) => handleDragOver(e, index)}
                    onDragEnd={handleDragEnd}
                    className="flex items-center gap-3 p-3 bg-white border rounded transition-all group"
                    style={{
                      borderColor: isDragOver ? "#7A1F3D" : "#D9D9D9",
                      opacity: isDragging ? 0.5 : 1,
                      cursor: "grab",
                      borderWidth: isDragOver ? "2px" : "1px",
                    }}
                    role="listitem"
                    aria-label={`${kavel.naam}, positie ${index + 1} van ${selectedKavelIds.length}`}
                  >
                    <svg
                      className="w-5 h-5 text-gray-400 flex-shrink-0"
                      fill="currentColor"
                      viewBox="0 0 20 20"
                      aria-hidden="true"
                    >
                      <path d="M7 2a2 2 0 1 0 .001 4.001A2 2 0 0 0 7 2zm0 6a2 2 0 1 0 .001 4.001A2 2 0 0 0 7 8zm0 6a2 2 0 1 0 .001 4.001A2 2 0 0 0 7 14zm6-8a2 2 0 1 0-.001-4.001A2 2 0 0 0 13 6zm0 2a2 2 0 1 0 .001 4.001A2 2 0 0 0 13 8zm0 6a2 2 0 1 0 .001 4.001A2 2 0 0 0 13 14z" />
                    </svg>

                    <div
                      className="w-8 h-8 flex items-center justify-center rounded font-bold text-white flex-shrink-0"
                      style={{ backgroundColor: "#7A1F3D" }}
                      aria-hidden="true"
                    >
                      {index + 1}
                    </div>

                    <div className="flex-1 min-w-0">
                      <div className="font-semibold text-sm truncate">
                        {kavel.naam}
                      </div>
                      <div className="text-xs text-gray-500">
                        €{kavel.minimumPrijs.toFixed(2)} • Duur: 60 sec
                      </div>
                    </div>

                    <div
                      className="w-6 h-6 rounded border border-gray-300 flex-shrink-0"
                      style={{ backgroundColor: `#${kavel.kavelkleur}` }}
                      aria-hidden="true"
                    />

                    <div className="flex flex-col gap-1 flex-shrink-0">
                      <button
                        onClick={() => moveKavelUp(index)}
                        disabled={index === 0}
                        className="w-6 h-6 flex items-center justify-center rounded transition-colors focus:outline-none focus:ring-2 disabled:opacity-30 disabled:cursor-not-allowed"
                        style={{
                          backgroundColor: index === 0 ? "#D9D9D9" : "#7A1F3D",
                          color: "#FFFFFF",
                        }}
                        aria-label={`Verplaats ${kavel.naam} omhoog`}
                        title="Verplaats omhoog"
                      >
                        <svg
                          width="12"
                          height="12"
                          viewBox="0 0 24 24"
                          fill="currentColor"
                        >
                          <path d="M7 14l5-5 5 5z" />
                        </svg>
                      </button>
                      <button
                        onClick={() => moveKavelDown(index)}
                        disabled={index === selectedKavelIds.length - 1}
                        className="w-6 h-6 flex items-center justify-center rounded transition-colors focus:outline-none focus:ring-2 disabled:opacity-30 disabled:cursor-not-allowed"
                        style={{
                          backgroundColor:
                            index === selectedKavelIds.length - 1
                              ? "#D9D9D9"
                              : "#7A1F3D",
                          color: "#FFFFFF",
                        }}
                        aria-label={`Verplaats ${kavel.naam} omlaag`}
                        title="Verplaats omlaag"
                      >
                        <svg
                          width="12"
                          height="12"
                          viewBox="0 0 24 24"
                          fill="currentColor"
                        >
                          <path d="M7 10l5 5 5-5z" />
                        </svg>
                      </button>
                    </div>

                    <button
                      onClick={() => onKavelToggle(kavelId, false)}
                      className="w-8 h-8 flex items-center justify-center rounded transition-colors hover:opacity-80 focus:outline-none focus:ring-2 flex-shrink-0"
                      style={{
                        color: "#FFFFFF",
                        backgroundColor: "#7A1F3D",
                        fontSize: "24px",
                        fontWeight: "bold",
                        lineHeight: "1",
                      }}
                      aria-label={`Verwijder ${kavel.naam} uit selectie`}
                      title="Verwijder uit selectie"
                    >
                      ×
                    </button>
                  </div>
                );
              })}
            </div>
          </div>
        )}
      </div>

      {isKavelSelectPopupOpen && (
        <Popup onClose={() => setIsKavelSelectPopupOpen(false)}>
          <h3 className="text-lg font-bold mb-4" style={{ color: "#7A1F3D" }}>
            Selecteer Kavels
          </h3>
          <div className="max-h-96 overflow-y-auto space-y-2">
            {kavels.map((kavel) => {
              const isSelected = selectedKavelIds.includes(kavel.id);
              return (
                <label
                  key={kavel.id}
                  className="flex items-center gap-2 cursor-pointer hover:bg-gray-50 p-2 rounded"
                >
                  <input
                    type="checkbox"
                    checked={isSelected}
                    onChange={(e) => onKavelToggle(kavel.id, e.target.checked)}
                    className="w-4 h-4"
                    style={{ accentColor: "#7A1F3D" }}
                    aria-label={`Selecteer ${kavel.naam}`}
                  />
                  <span className="text-sm flex-1">
                    {kavel.naam} - €{kavel.minimumPrijs.toFixed(2)}
                  </span>
                  <div
                    className="w-4 h-4 rounded border border-gray-300"
                    style={{ backgroundColor: `#${kavel.kavelkleur}` }}
                    aria-hidden="true"
                  />
                </label>
              );
            })}
          </div>
          <button
            onClick={() => setIsKavelSelectPopupOpen(false)}
            className="w-full mt-4 py-2 rounded font-semibold transition-opacity hover:opacity-90"
            style={{
              backgroundColor: "#7A1F3D",
              color: "#FFFFFF",
            }}
          >
            Bevestigen
          </button>
        </Popup>
      )}
    </>
  );
}
