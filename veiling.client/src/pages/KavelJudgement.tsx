import "../components/KavelInfo.css";
import KavelInfo from "../components/KavelInfo";
import HeaderLoggedout from "../components/HeaderLoggedout";
import ApproveOrDeny from "../components/AproveOrDenyTextBox";
import { useState } from "react";

function KavelJudgement() {
    const [currentKavelId, setCurrentKavelId] = useState(-1);

    const handleKavelFetched = (kavelId: number) => {
        setCurrentKavelId(kavelId);
        console.log('Current kavel ID:', kavelId);
    };

    const handleApproval = async (approval: boolean, reasoning: string) => {
    if (currentKavelId === null) {
        console.error('No kavel selected');
        return;
    }

    try {
        const response = await fetch(`/api/kavels/${currentKavelId}/approve`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ 
                approval, 
                reasoning 
            })
        });

        if (response.ok) {
            const data = await response.json();
            console.log('Success:', data);
        } else {
            console.error('Failed to update approval');
        }
    } catch (error) {
        console.error('Error: ', error);
    }
};

    return (
        <div>
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
                <KavelInfo 
                    sortOnApproval={true} 
                    onKavelFetched={handleKavelFetched}
                />
                <ApproveOrDeny onSubmitApproval={handleApproval}/>
            </div>
        </div>
    );
}

export default KavelJudgement;
