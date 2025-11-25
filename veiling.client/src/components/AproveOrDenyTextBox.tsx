import { useState } from 'react';
import SimpeleKnop from './SimpeleKnop';
import './ApproveOrDenyTextBox.css';


interface ApproveOrDenyProps {
    onSubmitApproval: (approval: boolean, reasoning: string) => void;
}

export default function ApproveOrDeny({ onSubmitApproval }: ApproveOrDenyProps) {
    const [reasoning, setReasoning] = useState('');
    const [approval, setApproval] = useState(Boolean);

    const handleApprove = () => {
        setApproval(true);
        console.log('Approved with reasoning:', reasoning);
        onSubmitApproval(approval, reasoning);
    };

    const handleDeny = () => {
        setApproval(false);
        console.log('Denied with reasoning:', reasoning);
        onSubmitApproval(approval, reasoning);
    };

    return (
        <div className="min-h-screen bg-white-100 p-8">
            <div className="max-w-2xl mx-auto bg-white rounded-sm shadow-lg overflow-hidden">
                <div className="bg-gray-200 px-6 py-4">
                    <h1 className="text-3xl font-bold text-gray-800">Approve or Deny</h1>
                </div>
            
                <div className="p-6">
                    <label htmlFor="reasoning" className="block text-xl font-medium text-gray-400 mb-4">
                    Reasoning
                    </label>
                    <textarea
                        id="reasoning"
                        value={reasoning}
                        onChange={(e) => setReasoning(e.target.value)}
                        className="w-full h-64 p-4 border border-gray-300 rounded-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none text-gray-700"
                        placeholder="Enter your reasoning here..."
                    />
            
                    <div className="flex gap-4 mt-6">
                        <SimpeleKnop 
                            onClick={handleApprove}
                            appearance='primary'
                        >
                        Approve
                        </SimpeleKnop>
                        <SimpeleKnop 
                            onClick={handleDeny}
                            appearance='secondary'
                        >
                        Deny
                        </SimpeleKnop>
                    </div>
                </div>
            </div>
        </div>
    );
}