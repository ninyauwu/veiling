import React, {useState, useRef} from 'react';
import DefaultImage from "../assets/Upload_Img.png";
import DefaultImageIcon from "../assets/Upload_icon.svg";

const ImageUpload = () => {
    const [avatarURL, setAvatarURL] = useState(DefaultImage);

    const fileUploadRef = useRef(); 

    const handleImageUpload = (event) => {
        event.preventDefault();
        fileUploadRef.current.click();
    }

    const uploadImageDisplay = () => {
        const uploadedFile = fileUploadRef.current.files[0];

        const  cashedURL = URL.createObjectURL(uploadedFile);

        setAvatarURL(cashedURL); 
    }

    return (
        <div className="relative h-69 w-69 m-8">
            <img 
            src={avatarURL}
            alt="UploadImage"
            className="h-69 w-69" />
            <form id="form" encType="multipart/form-data">
                <button
                    type="submit"
                    onClick={handleImageUpload}
                    className="flex-center absolute bottom-12 right-10 h-9 w-9">
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