import React, {useState, useRef, type FormEvent, type ChangeEvent} from 'react';
import DefaultImage from "../assets/Upload_Img.png";
import DefaultImageIcon from "../assets/Upload_icon.svg";

const ImageUpload = () => {
    const [avatarURL, setAvatarURL] = useState<string>(DefaultImage);

    const fileUploadRef = useRef<HTMLInputElement>(null); 

    const handleImageUpload = (event: FormEvent<HTMLButtonElement>) => {
        event.preventDefault();
        fileUploadRef.current?.click();
    }

    const uploadImageDisplay = async (event: ChangeEvent<HTMLInputElement>) => {
        try {
        if (!fileUploadRef.current?.files) return;

        const uploadedFile = fileUploadRef.current.files[0];
        const formData = new FormData();

        formData.append("File", uploadedFile);

        const response = await fetch("https://api.escuelajs.co/api/v1/files/upload", {
            method: "post",
            body: formData
        })

        if (response.status === 201) {
            const data = await response.json();
            setAvatarURL(data?.location);
        }

    } catch(error) {
        console.error(error);
        setAvatarURL(DefaultImage);
    }

        // const  cashedURL = URL.createObjectURL(uploadedFile);
        // setAvatarURL(cashedURL); 
    }

    return (
        <div className="relative">
            <img 
            src={avatarURL}
            alt="UploadImage"
            className="fixed top-0 left-0" />
            <form id="form" encType="multipart/form-data">
                <button
                    type="submit"
                    onClick={handleImageUpload}
                    className="relative-center">
                    <img
                        src={DefaultImageIcon}
                        alt="Upload Image Here."
                        className="object-cover" />
                </button>
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