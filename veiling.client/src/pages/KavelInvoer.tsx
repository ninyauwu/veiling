import ImageUpload from "../components/ImageUpload";
import KavelDescription from "../components/KavelDescription";
import KavelInvulTabel from "../components/KavelInvulTabel";
import SimpeleKnop from "../components/SimpeleKnop";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { authFetch } from "../utils/AuthFetch";

function KavelInvoer() {
    const navigate = useNavigate();
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

            const response = await authFetch('/api/kavels/upload-image', {
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
            Description: description,
            ImageUrl: uploadedImageUrl, 
            MinimumPrijs: formData.prijs,
            Aantal: formData.aantal, 
            Ql: formData.ql, 
            Plaats: formData.plaats, 
            Stadium: formData.stadium, 
            Lengte: formData.lengte, 
            Kleur: formData.kleur,
            Fustcode: formData.fustcode,
            AantalProductenPerContainer: formData.aantalPerContainer,
            GewichtVanBloemen: formData.gewicht 
          };

            console.log('Payload being sent:', payload);

            const response = await authFetch('/api/kavels', {
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
            <div style={{ height: "96px" }} />
            
            <div style={{
                display: "flex",
                flexDirection: "row",
                gap: "60px",
                flexWrap: "wrap",
                width: "100%",
            }}>
                <div style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '8px',
                    flex: '1 1 400px',
                    maxWidth: '600px',
                }}>
                    <div style={{
                        height: '35vh',
                        minHeight: '300px'
                    }}>
                        <ImageUpload onImageUpload={handleImageUpload} />
                    </div>
                    <KavelDescription onDescriptionChange={handleDescriptionChange}/>
                </div>
                
                <div style={{
                    flex: '2 1 500px',
                    minWidth: '400px',
                }}>
                    <KavelInvulTabel onDataChange={handleTableDataChange} />
                </div>
                
                <div style={{
                    display: 'flex',
                    position: 'fixed',
                    gap: '10px',
                    right: '2vw',
                    bottom: '2vh'
                }}>
                    <SimpeleKnop
                        label="Terug naar dashboard"
                        appearance="primary"
                        onClick={() => navigate("/verkoper-dashboard")}
                    />
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

export default KavelInvoer;