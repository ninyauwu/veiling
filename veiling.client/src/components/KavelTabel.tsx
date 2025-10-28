import { useEffect, useState } from "react";
import "./KavelTabel.css";

export default function KavelTabel({ endpoint, onRowSelect }: KavelTabelProps) {
  const [rows, setRows] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedRowIndex, setSelectedRowIndex] = useState<number | null>(0);

  useEffect(() => {
    async function fetchData() {
      setLoading(true);
      setError(null);

      try {
        // 🔹 Mock test data
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
      } catch (err) {
        if (err instanceof Error) setError(err.message);
        else setError(String(err));
      } finally {
        setLoading(false);
      }
    }

    fetchData();
  }, [endpoint]);

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
            onClick={() => {
              if (selectedRowIndex == i) return;
              setSelectedRowIndex(i);
              if (onRowSelect) onRowSelect(rows[i]);
            }}
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
}

interface KavelTabelProps {
  endpoint: string;
  onRowSelect?: (row: object) => void;
}
