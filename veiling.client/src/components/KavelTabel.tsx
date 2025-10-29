import { useEffect, useState, useImperativeHandle, forwardRef } from "react";
import "./KavelTabel.css";

export interface KavelTabelProps {
  endpoint: string;
  onRowSelect?: (row: object) => void;
  selectedRowIndex?: number | null; // external control
  onSelectedRowChange?: (index: number | null) => void; // notify parent
}

// Expose internal methods to parent via ref
export interface KavelTabelRef {
  getRowCount: () => number;
}

const KavelTabel = forwardRef<KavelTabelRef, KavelTabelProps>(
  (
    {
      endpoint,
      onRowSelect,
      selectedRowIndex: controlledIndex,
      onSelectedRowChange,
    },
    ref,
  ) => {
    const [rows, setRows] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [internalIndex, setInternalIndex] = useState<number | null>(null);

    const selectedRowIndex =
      controlledIndex !== undefined ? controlledIndex : internalIndex;

    // Expose getters to parent
    useImperativeHandle(ref, () => ({
      getRowCount: () => rows.length,
    }));

    useEffect(() => {
      async function fetchData() {
        setLoading(true);
        setError(null);

        try {
          // Mocked test data
          const data = [
            {
              Naam: "Rozen",
              Prijs: "12c-20c",
              Aantal: 19,
              Leverancier: "Kazen NV",
              Kwaliteit: "B2",
              QI: "C",
            },
            {
              Naam: "Spaghetti",
              Prijs: "12c-20c",
              Aantal: 200,
              Leverancier: "RoyalTulpen BV",
              Kwaliteit: "A1",
              QI: "A",
            },
            {
              Naam: "Ravioli",
              Prijs: "12c-20c",
              Aantal: 43,
              Leverancier: "RoyalTulpen BV",
              Kwaliteit: "A1",
              QI: "A",
            },
          ];

          await new Promise((resolve) => setTimeout(resolve, 500));
          setRows(data);

          // Auto-select first row if none selected
          if (data.length > 0 && selectedRowIndex == null) {
            handleRowSelect(0, data);
          }
        } catch (err) {
          setError(err instanceof Error ? err.message : String(err));
        } finally {
          setLoading(false);
        }
      }

      fetchData();
    }, [endpoint]);

    const handleRowSelect = (i: number, dataOverride?: any[]) => {
      const data = dataOverride ?? rows;

      if (i < 0 || i >= data.length) return; // Prevent overflow
      if (selectedRowIndex === i) return;

      if (controlledIndex === undefined) {
        setInternalIndex(i);
      }

      onSelectedRowChange?.(i);
      onRowSelect?.(data[i]);
    };

    if (loading) return <p className="text-gray-500">Loading...</p>;
    if (error) return <p className="text-red-600">Error: {error}</p>;
    if (rows.length === 0) return <p>No data found.</p>;

    const columns = Object.keys(rows[0]);

    return (
      <table className="kavel-tabel">
        <thead>
          <tr>
            {columns.map((col) => (
              <th key={col}>{col}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {rows.map((row, i) => (
            <tr
              key={i}
              onClick={() => handleRowSelect(i)}
              className={selectedRowIndex === i ? "kavel-row-selected" : ""}
            >
              {columns.map((col) => (
                <td key={col} className="border border-gray-300 p-2">
                  {String(row[col])}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    );
  },
);

export default KavelTabel;
