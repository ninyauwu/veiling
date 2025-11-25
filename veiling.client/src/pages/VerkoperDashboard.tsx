import ImageUpload from "../components/ImageUpload";
import KavelDescription from "../components/KavelDescription";
import HeaderLoggedout from "../components/HeaderLoggedout";
import KavelInvulTabel from "../components/KavelInvulTabel";
import SimpeleKnop from "../components/SimpeleKnop";


function VerkoperDashboard() {
    return (
        <div style={{
            maxWidth: '1400px',  
            margin: '0 auto',     
            padding: '0 40px',    
            width: '100%'
        }}>
            <HeaderLoggedout />
        <div
            style={{
                height: "96px",
            }}
        />
        <div
            style={{
                display: "flex",
                flexDirection: "row",
                gap: "60px",
            }}
        >
            <div
                style={{
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '8px',
                    width: '800px',
                    flexShrink: 0
                }}>
                <div style={{ height: '400px' }}>  {/* Wrap ImageUpload with fixed height */}
                    <ImageUpload />
                </div>
                <KavelDescription />
            </div>
            <div
            style={{flex: 1
            }}>
                <KavelInvulTabel />
            </div>
            <div
            style={{
                display: 'flex',
                position: 'fixed',
                gap: '10px',
                right: '40px',               
                top: '92%',                  
                transform: 'translateY(-50%)'
            }}>
                <SimpeleKnop 
                    label="Submit"
                    appearance="primary"
                    />
            </div>
        </div>
    </div>
);}

export default VerkoperDashboard;
