import {useState, useRef, type FormEvent} from 'react';
import DefaultImage from "../assets/img_upload_bg.png";
import DefaultImageIcon from "../assets/img_upload.tsx";

interface ImageUploadProps {
    onImageUpload: (file: string | File | null) => void;
}

const ImageUpload = ({ onImageUpload }: ImageUploadProps) => {
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
                
                onImageUpload(uploadedFile);
                
            } catch(error) {
                console.error(error);
                
                setAvatarURL(DefaultImage);
                onImageUpload(DefaultImage);
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
                accept="image/*"
                hidden/>
            </form>
        </div>
    )
}

export default ImageUpload