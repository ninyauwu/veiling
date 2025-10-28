import React from "react";

interface AspectRatioImageGridProps {
  images: string[];
  className?: string;
  gap?: number;
  objectFit?: "cover" | "contain" | "fill" | "none" | "scale-down";
}

const AspectRatioImageGrid: React.FC<AspectRatioImageGridProps> = ({
  images,
  className = "",
  gap = 8,
  objectFit = "cover",
}) => {
  if (!images || images.length === 0) {
    return null;
  }

  return (
    <div
      className={`aspect-ratio-image-grid ${className}`}
      style={{
        display: "grid",
        gridTemplateColumns: `repeat(${images.length}, 1fr)`,
        gap: `${gap}px`,
        width: "100%",
        aspectRatio: `${images.length} / 1`,
      }}
    >
      {images.map((imagePath, index) => (
        <div
          key={index}
          className="image-container"
          style={{
            position: "relative",
            width: "100%",
            height: "100%",
          }}
        >
          <img
            src={imagePath}
            alt={`Image ${index + 1}`}
            style={{
              width: "100%",
              height: "100%",
              objectFit: objectFit,
              borderRadius: "4px",
            }}
            onError={(e) => {
              // Fallback for broken images
              const target = e.target as HTMLImageElement;
              target.src = `https://via.placeholder.com/150/cccccc/969696?text=Image+${index + 1}`;
            }}
          />
        </div>
      ))}
    </div>
  );
};

export default AspectRatioImageGrid;
