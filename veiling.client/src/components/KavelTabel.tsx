import React from "react";
import "./KavelTabel.css";

export interface KavelTabelProps {
  rows: any[];
  selectedRowIndex?: number | null;
  onRowSelect?: (row: any, index: number) => void;
  onSelectedRowChange?: (index: number | null) => void;
}

const KavelTabel: React.FC<KavelTabelProps> = ({
  rows,
  selectedRowIndex,
  onRowSelect,
  onSelectedRowChange,
}) => {
  const currentIndex = selectedRowIndex ?? null;

  const handleRowSelect = (i: number) => {
    if (i < 0 || i >= rows.length) return;
    if (currentIndex === i) return;

    onSelectedRowChange?.(i);
    onRowSelect?.(rows[i], i);
  };

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
            className={currentIndex === i ? "kavel-row-selected" : ""}
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
};

export default KavelTabel;
