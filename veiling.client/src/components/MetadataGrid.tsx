import React from "react";
import "./MetadataGrid.css";

interface MetadataItem {
  key: string;
  value?: React.ReactNode; // Make value optional
}

interface MetadataGridProps {
  items: MetadataItem[];
  className?: string;
  itemClassName?: string;
  labelClassName?: string;
  valueClassName?: string;
}

export default function MetadataGrid({
  items,
  className = "",
  itemClassName = "",
  labelClassName = "",
  valueClassName = "",
}: MetadataGridProps) {
  return (
    <div className={`metadata-grid ${className}`}>
      {items.map((item, index) => (
        <div key={index} className={`metadata-item ${itemClassName}`}>
          <span className={`metadata-label ${labelClassName}`}>
            {item.key}:
          </span>
          {item.value !== undefined && (
            <span className={`metadata-value ${valueClassName}`}>
              {item.value}
            </span>
          )}
        </div>
      ))}
    </div>
  );
}
