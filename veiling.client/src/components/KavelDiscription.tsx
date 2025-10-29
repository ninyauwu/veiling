import { useState } from 'react';

function ImageDescription() {
    const [description, setDescription] = useState('');

    return (
        <div className="w-100 ">
            <textarea
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Description"
                className="w-full bg-gray-800 p-4 text-black placeholder-gray-600 border border-gray-400 focus:outline-none focus:border-gray-600 resize-none"
                rows={4}
            />
        </div>
    );
}

export default ImageDescription;