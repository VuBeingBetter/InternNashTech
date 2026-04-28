import { useState } from 'react';
import axiosClient from '../../api/axiosClient';

export default function TransactionForm({ title, endpoint, accountNumber, onSuccess }) {
    const [amount, setAmount] = useState('');
    const [targetAccount, setTargetAccount] = useState(''); // Only for Transfer
    const [message, setMessage] = useState({ text: '', type: '' });

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            // Prep
            const payload = { 
                accountNumber, // Deposit, Withdraw
                fromAccountNumber: accountNumber, // Transfer
                toAccountNumber: targetAccount, 
                amount: parseFloat(amount) 
            };

            await axiosClient.post(endpoint, payload);
            setMessage({ text: 'Transaction successful!', type: 'success' });
            if (onSuccess) onSuccess(); // Callback to refresh the detail page
        } catch (error) {
            setMessage({ text: error.response?.data?.message || 'Error occurred', type: 'error' });
        }
    };

    return (
        <form onSubmit={handleSubmit} className="bg-white p-4 border rounded shadow-sm justify-center">
            <h3 className="font-bold mb-2">{title}</h3>
            
            {/* Input amount */}
            <input 
                type="number" className="border p-2 w-full mb-2" placeholder="Amount"
                value={amount} onChange={(e) => setAmount(e.target.value)} required 
            />

            {/* Only show if Transfer */}
            {endpoint.includes('transfer') && (
                <input 
                    type="text" className="border p-2 w-full mb-2" placeholder="To Account Number"
                    value={targetAccount} onChange={(e) => setTargetAccount(e.target.value)} required 
                />
            )}

            <button type="submit" className="bg-blue-600 text-white w-full py-2 rounded">Confirm</button>
            
            {message.text && (
                <p className={`mt-2 ${message.type === 'success' ? 'text-green-600' : 'text-red-600'}`}>
                    {message.text}
                </p>
            )}
        </form>
    );
}