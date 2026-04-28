import { useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import axiosClient from "../api/axiosClient";

export default function TransactionHistory() {
    const { accountNumber } = useParams();
    const [transactions, setTransactions] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchTransactions = async () => {
            try {
                const response = await axiosClient.get(`/transaction/${accountNumber}/history`);
                console.log(response.data);
                setTransactions(response.data?.transactions);
            } catch (error) {
                console.error("Error fetching transactions:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchTransactions();
    }, [accountNumber]);

    if (loading) {
        return <div className="p-6">Loading...</div>;
    }

    return (
        <div className="p-6 max-w-4xl mx-auto">
            <Link to={`/${accountNumber}/details`} className="text-blue-500 hover:underline mb-4 block">&larr; Back to Account</Link>
            
            <h2 className="text-2xl font-bold mb-4 p-2">Transaction History - {accountNumber}</h2>

            <table className="w-full border-collapse border border-gray-300 table-fixed">
                <thead className="bg-gray-800 text-white">
                    <tr>
                        <th className="p-3">Date</th>
                        <th className="p-3">Type</th>
                        <th className="p-3">Amount</th>
                        <th className="p-3">Description</th>
                    </tr>
                </thead>
                <tbody>
                    {transactions.length > 0 ? (
                        transactions.map((tx, index) => (
                            <tr key={index} className="border-b text-center">
                                <td className="p-3 text-center">{new Date(tx.createdAt).toLocaleDateString()}</td>
                                <td className="p-3 font-semibold">
                                    <span className={tx.type === 'DEPOSIT' ? 'text-green-600' : 
                                        tx.type === 'WITHDRAW' ? 'text-yellow-600' : 'text-blue-600'}>
                                        {tx.type}
                                    </span>
                                </td>
                                <td className="p-3 text-left">${tx.amount?.toFixed(2)}</td>
                                <td className="p-3 text-left">{tx.description || "N/A"}</td>
                            </tr>
                        ))
                    ) : (
                        <tr><td colSpan="4" className="p-4 text-center">No transactions found.</td></tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}