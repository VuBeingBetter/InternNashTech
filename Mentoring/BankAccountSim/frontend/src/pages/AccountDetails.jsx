import { useState, useEffect } from "react";
import { Link } from "react-router";
import { useParams } from "react-router";

import axiosClient from "../api/axiosClient";

export default function AccountDetails() {
    const { accountNumber } = useParams();
    const [account, setAccount] = useState(null);
    const [loading, setLoading] = useState(true);

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
    
    return (
        <div className="p-6 max-w-2xl mx-auto">
            <Link to="/" className="text-blue-500 hover:underline mb-4 block">
                &larr;Back to Accounts
            </Link>

            <div className="bg-white shadow-md rounded-lg p-6 border">
                <h2 className="text-2xl font-bold mb-4">Account Details</h2>

                <div className="space-y-4">
                    <p><strong>Account Number:</strong> {account.accountNumber}</p>
                    <p><strong>Owner:</strong> {account.ownerName}</p>
                    <p><strong>Balance:</strong> {account.balance.toFixed(2)}</p>
                    <p>
                        <strong>Status:</strong>
                        <span className={account.status === "ACTIVE" ? "text-green-600" : "text-red-600"}>
                            {account.status}
                        </span>
                    </p>
                </div>

                <div className="mt-6 flex gap-3">
                    <button className="bg-green-500 text-white px-4 py-2 rounded">Deposit</button>
                    <button className="bg-yellow-500 text-white px-4 py-2 rounded">Withdraw</button>
                    <button className="bg-blue-500 text-white px-4 py-2 rounded">Transfer</button>
                    <button className="bg-gray-500 text-white px-4 py-2 rounded">History</button>
                </div>
            </div>
        </div>
    );
    
}