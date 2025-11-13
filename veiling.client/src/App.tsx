import './App.css';
import ImageUpload from './components/ImageUpload';
import LoginWidget from './components/LoginWidget';

function App() {
    return (
        <div className="flex">
            <>
                <LoginWidget />
                <ImageUpload />
            </>
        </div>
    );
}

export default App;