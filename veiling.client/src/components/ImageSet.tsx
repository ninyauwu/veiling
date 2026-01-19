import React from "react";
import type { FC, CSSProperties } from "react";

interface AspectRatioImageGridProps {
  images: string[];
  className?: string;
  gap?: number;
  objectFit?: "cover" | "contain" | "fill" | "none" | "scale-down";
}

const AspectRatioImageGrid: FC<AspectRatioImageGridProps> = ({
  images,
  className = "",
  gap = 8,
  objectFit = "cover",
}) => {
  if (!images || images.length === 0) {
    return null;
  }

  const imageCount = images.length;
  const maxWidthPercent = imageCount < 3 ? 33.333 : 100;

  const containerStyle: CSSProperties = {
    display: "flex",
    justifyContent: "center",
    width: "100%",
  };

  const gridStyle: CSSProperties = {
    display: "grid",
    gridTemplateColumns: `repeat(${imageCount}, 1fr)`,
    gap: `${gap}px`,
    width: "100%",
    maxWidth: `${maxWidthPercent}%`,
  };

  const imageContainerStyle: CSSProperties = {
    position: "relative",
    width: "100%",
    aspectRatio: "1 / 1",
  };

  const imageStyle: CSSProperties = {
    width: "100%",
    height: "100%",
    objectFit: objectFit,
    borderRadius: "4px",
  };

  const handleImageError =
    (index: number) => (e: React.SyntheticEvent<HTMLImageElement>) => {
      const target = e.currentTarget;
      target.src = `https://via.placeholder.com/150/cccccc/969696?text=Image+${index + 1}`;
    };

  return (
    <div style={containerStyle}>
      <div className={`aspect-ratio-image-grid ${className}`} style={gridStyle}>
        {images.map((imagePath, index) => (
          <div
            key={index}
            className="image-container"
            style={imageContainerStyle}
          >
            <img
              src={imagePath}
              alt={`Kavel afbeelding ${index + 1}`}
              style={imageStyle}
              onError={handleImageError(index)}
            />
          </div>
        ))}
      </div>
    </div>
  );
};

export default AspectRatioImageGrid;
