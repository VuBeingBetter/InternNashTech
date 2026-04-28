import { useState, useEffect } from "react";
import { Link, useParams, useNavigate } from "react-router";

import axiosClient from "../api/axiosClient";
import TransactionForm from "../components/ui/TransactionForm";

export default function AccountDetails() {
    const { accountNumber } = useParams();
    const [account, setAccount] = useState(null);
    const [loading, setLoading] = useState(true);
    const [activeForm, setActiveForm] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchDetails = async () => {
            try {
                const response = await axiosClient.get(`/account/${accountNumber}`);
                setAccount(response.data);
            } catch (error) {
                console.error("Error fetching details:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchDetails();
    }, [accountNumber]);

    if (loading) {
        return <div className="p-6">Loading...</div>;
    }

    if (!account) {
        return <div className="p-6">Account not found</div>;
    }

    const handleToggleStatus = async () => {
        try {
            await axiosClient.post(`/account/${account.accountNumber}/toggle-status`);
            window.axiosClient.reload();
        } catch (error) {
            console.error("Full error object:", error);
        
            // Lấy thông báo lỗi thông minh hơn
            const msg = error.response?.data?.message || 
                        (typeof error.response?.data === 'string' ? error.response.data : "Unknown error");

            alert("Failed to set status: " + msg);
        }
    }

    return (
        <div className="p-6 max-w-2xl mx-auto">
            <Link to="/" className="text-blue-500 hover:underline mb-4 block">
                &larr; Back to Account List
            </Link>

            <div className="bg-gray shadow-md rounded-lg p-6 border max-w-3xl mx-auto">
                <h2 className="text-2xl font-bold mb-4">Account Details</h2>

                <div className="space-y-4">
                    <p><strong>Account Number:</strong> {account.accountNumber}</p>
                    <p><strong>Owner:</strong> {account.ownerName}</p>
                    <p><strong>Balance:</strong> ${account.balance.toFixed(2)}</p>
                    <p>
                        <strong>Status: </strong>
                        <span className={account.status === "ACTIVE" ? "text-green-600" : "text-red-600"}>
                            {account.status}
                        </span>
                    </p>
                </div>

                <button 
                    onClick={handleToggleStatus}
                    className={`px-2 py-1 rounded text-white mt-2 ${
                        account.status === 'ACTIVE' ? 'bg-red-600 hover:bg-red-700 active:bg-red-800' 
                                                    : 'bg-green-600 hover:bg-green-700 active:bg-green-800'
                    }`}
                >
                    {account.status === 'ACTIVE' ? 'Freeze Account' : 'Unfreeze Account'}
                </button>

                <div className="mt-6 flex gap-3 justify-center">
                    
                    <button className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 active:bg-green-700"
                            onClick={() => setActiveForm('deposit')}>Deposit</button>
                    <button className="bg-yellow-500 text-white px-4 py-2 rounded hover:bg-yellow-600 active:bg-yellow-700"
                            onClick={() => setActiveForm('withdraw')}>Withdraw</button>
                    <button className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 active:bg-blue-700"
                            onClick={() => setActiveForm('transfer')}>Transfer</button>
                    <button className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600 active:bg-gray-700"
                            onClick={() => navigate(`/${account.accountNumber}/history`)}>History</button>
                </div>

                <div className="mt-8 flex justify-center">
                    <div className="w-full max-w-md">
                        {activeForm === 'deposit' && (<TransactionForm 
                            title="Deposit" 
                            endpoint="/transaction/deposit" 
                            accountNumber={account.accountNumber} 
                            onSuccess={() => window.location.reload()} // Refresh để cập nhật số dư
                        />)}
                        {activeForm === 'withdraw' && (<TransactionForm 
                            title="Withdraw" 
                            endpoint="/transaction/withdraw" 
                            accountNumber={account.accountNumber}
                            onSuccess={() => window.location.reload()}
                        />)}
                        {activeForm === 'transfer' && (<TransactionForm 
                            title="Transfer" 
                            endpoint="/transaction/transfer" 
                            accountNumber={account.accountNumber}
                            onSuccess={() => window.location.reload()}
                        />)}
                    </div>
                    
                </div>
            </div>
        </div>
    );
    
}