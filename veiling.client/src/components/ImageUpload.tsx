import {useState, useRef, type FormEvent} from 'react';
import DefaultImage from "../assets/img_upload_bg.png";
import DefaultImageIcon from "../assets/img_upload.tsx";

interface UploadResponse {
    originalname: string;
    filename: string;
    location: string;
}

const ImageUpload = () => {
    const [avatarURL, setAvatarURL] = useState<string>(DefaultImage);
    const [showIcon, setShowIcon] = useState<boolean>(true);

    const fileUploadRef = useRef<HTMLInputElement>(null); 

    const handleImageUpload = (event: FormEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fileUploadRef.current?.click();
    }

    const uploadImageDisplay = async () => {
        try {
            if (!fileUploadRef.current?.files) return;

            const uploadedFile = fileUploadRef.current.files[0];

            const  cashedURL = URL.createObjectURL(uploadedFile);
            setAvatarURL(cashedURL);
            setShowIcon(false);

            const formData = new FormData();
            formData.append("file", uploadedFile);

            const response = await fetch("https://api.escuelajs.co/api/v1/files/upload", {
                method: "post",
                body: formData
            })

            if(!response.ok) {
                throw new Error(`Upload failed: ${response.status}`);
            }

            const data = await response.json() as UploadResponse;

            if(data.location) {
                console.log("Upload succesful: ", data.location);
            }
        } catch(error) {
            console.error(error);
            setAvatarURL(DefaultImage);
        }
    }

    return (
        <div className="relative w-full h-full">
            <img 
            src={avatarURL}
            alt="UploadImage"
            className="w-full h-full object-cover" />
            <form id="form" encType="multipart/form-data">
            {showIcon && (
                <button
                    type="submit"
                    onClick={handleImageUpload}
                    className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
                    <DefaultImageIcon className="w-16 h-16 text-gray-600 hover:text-gray-800" />
                </button>
            )}
                <input 
                type="file"
                id="file" 
                ref={fileUploadRef}
                onChange={uploadImageDisplay}
                hidden/>
            </form>
        </div>
    )
}

export default ImageUpload