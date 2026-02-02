import { useState } from "react";
import SimpeleKnop from "./SimpeleKnop";
import "./ApproveOrDenyTextBox.css";
import { authFetch } from "../utils/AuthFetch";

interface ApproveOrDenyProps {
  currentKavelId: number;
  onApprovalResponse?: (kavelId: number) => void;
}

export default function ApproveOrDeny({
  currentKavelId,
  onApprovalResponse,
}: ApproveOrDenyProps) {
  const [reasoning, setReasoning] = useState("");
  const [maximumPrijs, setMaximumPrijs] = useState<number | "">("");

  const isMaxPriceValid = typeof maximumPrijs === "number" && maximumPrijs > 0;

  const handleApprove = () => {
    console.log("Approved with reasoning:", reasoning);
    onSubmitApproval(true, reasoning, Number(maximumPrijs));
  };

  const onSubmitApproval = async (approval: boolean, reasoning: string, maximumPrijs: number | "") => {
    if (currentKavelId === null) {
      console.error("No kavel selected");
      return;
    }

    try {
      const response = await authFetch(`/api/kavels/${currentKavelId}/approve`, {
        method: "PATCH",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          approval,
          reasoning,
          maximumPrijs,
        }),
      });

      if (response.ok) {
        const data = await response.json();
        console.log("Success:", data);
        if (onApprovalResponse) {
          onApprovalResponse(currentKavelId);
        }
      } else {
        console.error("Failed to update approval");
      }
    } catch (error) {
      console.error("Error: ", error);
    }
  };

  const handleDeny = () => {
    console.log("Denied with reasoning:", reasoning);
    onSubmitApproval(false, reasoning, Number(maximumPrijs));
  };

  return (
    <div className="min-h-screen bg-white-100 p-8">
      <div className="max-w-2xl mx-auto bg-white rounded-sm shadow-lg overflow-hidden">
        <div className="bg-gray-200 px-6 py-4">
          <h2>Approve or Deny</h2>
        </div>

        <div className="p-6">
          <label
            htmlFor="reasoning"
            className="block text-xl font-medium text-gray-400 mb-4"
          >
            Reasoning
          </label>
          <textarea
            id="reasoning"
            value={reasoning}
            onChange={(e) => setReasoning(e.target.value)}
            className="w-full h-64 p-4 border border-gray-300 rounded-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none text-gray-700"
            placeholder="Enter your reasoning here..."
          />

          <label
            htmlFor="maximumPrijs"
            className="block text-xl font-medium text-gray-400 mb-4"
          >
            Maximum Prijs (€)
          </label>
          <input
            type="number"
            id="maximumPrijs"
            value={maximumPrijs}
            onChange={(e) => {
              const value = e.target.value;
              setMaximumPrijs(value === "" ? "" : Number(value));
            }}
            step="0.01"
            min="0"
            inputMode="decimal"
            className="w-full h-14 p-4 border border-gray-300 rounded-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent text-gray-700"
            placeholder="0.00"
          />

          <div className="flex gap-4 mt-6">
            <SimpeleKnop
              onClick={handleApprove}
              appearance="primary"
              disabled={!isMaxPriceValid}
            >
              Approve
            </SimpeleKnop>
            <SimpeleKnop onClick={handleDeny} appearance="secondary">
              Deny
            </SimpeleKnop>
          </div>
        </div>
      </div>
    </div>
  );
}
