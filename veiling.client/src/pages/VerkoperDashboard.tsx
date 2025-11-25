import ImageUpload from "../components/ImageUpload";
import KavelDescription from "../components/KavelDescription";
import HeaderLoggedout from "../components/HeaderLoggedout";
import KavelInvulTabel from "../components/KavelInvulTabel";
import SimpeleKnop from "../components/SimpeleKnop";
import { useState } from "react";

function VerkoperDashboard() {
    const [description, setDescription] = useState<string>('');
    const [imageFile, setImageFile] = useState<string | File | null>();
    const [formData, setFormData] = useState({
    naam: '',
    prijs: '',
    aantal: '',
    ql: '',
    plaats: '',
    stadium: '',
    lengte: '',
    kleur: '',
    fustcode: '',
    aantalPerContainer: '',
    gewicht: ''
    });

    const [isFormValid, setIsFormValid] = useState(false);

    const handleImageUpload = (file: string | File | null, ) => {
        setImageFile(file); 
    };

    const handleTableDataChange = (data: any, isValid: boolean) => {
        setFormData(data);
        setIsFormValid(isValid);
    };

    const handleDescriptionChange = (description: string) => {
        setDescription(description);
    }

    const uploadImageToServer = async (): Promise<string | null | string> => {
        if (!imageFile) return null;

        try {
            const formData = new FormData();
            formData.append('image', imageFile);

            const response = await fetch('/api/kavels/upload-image', {
                method: 'POST',
                body: formData
            });

            if (!response.ok) {
                throw new Error('Image upload failed');
            }

            const result = await response.json();
            return result.imageUrl;
        } catch (error) {
            console.error('Error uploading image:', error);
            throw error;
        }
    };

    const handleSubmit = async () => {
        try {
            let uploadedImageUrl = null;
            if (imageFile) {
                uploadedImageUrl = await uploadImageToServer();
            }

            const payload = {
                Naam: formData.naam,
                Beschrijving: description,
                Foto: uploadedImageUrl,
                
                MinimumPrijs: formData.prijs,
                aantal: formData.aantal,
                ql: formData.ql,
                VeilingId: formData.plaats,
                StageOfMaturity: formData.stadium,
                LengteVanBloem: formData.lengte,
                KavelKleur: formData.kleur,
                Fustcode: formData.fustcode,
                AantalProductenPerContainer: formData.aantalPerContainer,
                GewichtVanBloem: formData.gewicht,
            };

            console.log('Payload being sent:', payload);

            const response = await fetch('/api/kavels', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
            }

            const result = await response.json();
            console.log('Success:', result);
            alert('Kavel successfully created!');
            
        } catch (error) {
            console.error('Error submitting data:', error);
            alert('Failed to submit data. Please try again.');
        }
    };

    return (
        <div style={{
            maxWidth: '1400px',  
            margin: '0 auto',     
            padding: '0 40px',    
            width: '100%'
        }}>
            <HeaderLoggedout />
            <div style={{ height: "96px" }} />
            
            <div style={{
                display: "flex",
                flexDirection: "row",
                gap: "60px",
                flexWrap: "wrap",        // <—
                width: "100%",
            }}>
                <div style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '8px',
                    flex: '1 1 400px',     // <—
                    maxWidth: '600px',
                }}>
                    <div style={{
                        height: '35vh',      // <—
                        minHeight: '300px'   // <—
                    }}>
                        <ImageUpload onImageUpload={handleImageUpload} />
                    </div>
                    <KavelDescription onDescriptionChange={handleDescriptionChange}/>
                </div>
                
                <div style={{
                    flex: '2 1 500px',     // <—
                    minWidth: '400px',      // <—
                }}>
                    <KavelInvulTabel onDataChange={handleTableDataChange} />
                </div>
                
                <div style={{
                    display: 'flex',
                    position: 'fixed',
                    gap: '10px',
                    right: '2vw',          // <—
                    bottom: '2vh'          // <—
                }}>
                    {/* UPDATED: Disable button when form is invalid */}
                    <SimpeleKnop 
                        label="Submit"
                        appearance="primary"
                        onClick={handleSubmit}
                        disabled={!isFormValid}
                    />
                </div>
            </div>
        </div>
    );
}

export default VerkoperDashboard;