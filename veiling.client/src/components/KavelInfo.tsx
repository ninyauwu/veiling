import { useRef, useState } from "react";
import KavelTabel from "./KavelTabel";
import type { KavelTabelRef } from "./KavelTabel";
import "../App.css";
import Spacer from "./Spacer";
import ImageSet from "./ImageSet";
import NavigationBar from "./NavigationBar";
import MetadataGrid from "./MetadataGrid";

function KavelInfo() {
  const imagePaths = [
    "https://picsum.photos/400/400?random=1",
    "https://picsum.photos/400/400?random=2",
    "https://picsum.photos/400/400?random=3",
  ];

  const tableRef = useRef<KavelTabelRef>(null);
  const [selected, setSelected] = useState<number | null>(null);

  const handleNext = () => {
    const total = tableRef.current?.getRowCount() ?? 0;
    setSelected((prev) => {
      const next = (prev ?? 0) + 1;
      return next >= total ? total - 1 : next;
    });
  };

  const handlePrevious = () => {
    setSelected((prev) => {
      const next = (prev ?? 0) - 1;
      return next < 0 ? 0 : next;
    });
  };

  return (
    <div className="flex-column">
      <h1 className="hidden">Veilingpagina</h1>
      <h2 className="hidden">Kaveltabel</h2>
      <KavelTabel
        ref={tableRef}
        endpoint="/api/kavels"
        selectedRowIndex={selected}
        onSelectedRowChange={setSelected}
        onRowSelect={(row) => console.log("Selected:", row)}
      />
      <Spacer color="#00000000" />
      <NavigationBar onPrevious={handlePrevious} onNext={handleNext} />
      <Spacer />
      <div
        style={{
          inlineSize: "100%",
          maxWidth: "100%",
          boxSizing: "border-box",
          overflowWrap: "break-word",
          wordWrap: "break-word",
          minWidth: 0,
          height: "100%",
          display: "inline-block",
          flexGrow: 0,
        }}
      />
      <span>
        <div className="flex-row-justify">
          <h2>Alexei</h2>
          <div className="company-info">
            <div className="company-name">Supah</div>
            <div>QI</div>
            <div className="company-rating">A</div>
            <div>Kw.</div>
            <div className="company-rating">
              <span
                style={{
                  paddingRight: "12px",
                }}
              >
                A1
              </span>
              <img
                src="/icons/quality_star.svg"
                alt="Ster"
                className="quality-star"
              />
              <img
                src="/icons/quality_star.svg"
                alt="Ster"
                className="quality-star"
              />
            </div>
          </div>
        </div>
        <p>
          In the Siberian gulag I met a man mamed Alexei who spoke of things I
          could neither understand nor ignore. He spoke of aristocrats without
          faces who wore clothes of human skin. He spoke of the old Tsar rising
          on a moonless night to take back his skeletal empire. He spoke of
          children melting into the snow and their parents chasing their echoing
          voices upon the falling flakes in the dark. I did not sleep well on
          nights I spoke with Alexei. He had a disposition that would make even
          the late Rasputin nervous.
        </p>
        <p>
          I remember most that dreadful October morning when the guards found
          him dead in his cell. The pieces they carried out didn't make sense.
          Some I recognized, some I didn't. There were too many, to be sure. I
          think they pulled out three arms somehow, even though he was the only
          one in that cell the previous night. They tried to clean the blood off
          the walls but the water kept freezing, so they left it for the spring.
        </p>
        <p>
          They didn't stick with it. It made them too nervous, whatever was left
          on those concrete walls. I never saw what inside that cell, but the
          way they glanced at it was clear. For men of ice and steel such as
          them to be afraid... It took only one week for them to build a brick
          wall in front of it. That was the last of Alexei.
        </p>
      </span>
      <ImageSet images={imagePaths} />
      <MetadataGrid items={GetExampleMetadata()} />
      <Spacer />
      <NavigationBar onPrevious={handlePrevious} onNext={handleNext} />
    </div>
  );
}

function GetExampleMetadata() {
  return [
    { key: "Stadium", value: "23" },
    { key: "Fustcode", value: "13714" },
    {
      key: "Kleur",
      value: (
        <div
          style={{
            backgroundColor: "#360134",
            width: "24px",
            height: "24px",
          }}
        />
      ),
    },
  ];
}

export default KavelInfo;
