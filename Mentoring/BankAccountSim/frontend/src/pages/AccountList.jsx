import { useEffect, useState } from "react";
import axiosClient from "../api/axiosClient";

export default function AccountList() {
    const [accounts, setAccounts] = useState([]);
    const [search, setSearch] = useState("");
    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await axiosClient.get(`account?page=${page}&search=${search}`);
                setAccounts(response.data.Data);
                setTotalPages(response.data.TotalPages);
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        }

        fetchData();
    }, [page, search]);

    return (
        <div className="p-6 max-w-6xl mx-auto">
            <h2 className="text-2xl font-bold mb-4">Bank Accounts</h2>

            {/* Search Bar */}
            <div className="mb-4 flex gap-2">
                <input
                    className="border p-2 rounded w-full"
                    placeholder="Search account..."
                    value={search}
                    onChange={(e) => setSearch(e.target.value)}
                />
            </div>

            {/* Account List */}
            <table className="w-full border-collapse border border-gray-300">
                <thead className="bg-gray-800 text-white">
                    <tr>
                        <th className="p-2">Account</th>
                        <th className="p-2">Owner</th>
                        <th className="p-2">Balance</th>
                        <th className="p-2">Status</th>
                    </tr>
                </thead>

                <tbody>
                    {accounts.map(account => (
                        <tr key={account.accountNumber} className="border-b" to={`/${account.accountNumber}/details`}>
                            <td className="p-2">{account.accountNumber}</td>
                            <td className="p-2">{account.ownerName}</td>
                            <td className="p-2">{account.balance}</td>
                            <td className="p-2">
                                <span className={account.status === "ACTIVE" ? "text-green-600" : "text-red-600"}>
                                    {account.status}
                                </span>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* Pagination */}
            <div className="mt-4 flex gap-2">
                <button
                    disabled={page === 1}
                    onClick={() => setPage(page => page - 1)}
                    className="bg-blue-500 text-white p-2 rounded disabled:bg-gray-300"
                >Prev</button>

                <span className="p-2">Page {page} of {totalPages}</span>

                <button
                    disabled={page === totalPages}
                    onClick={() => setPage(page => page + 1)}
                    className="bg-blue-500 text-white p-2 rounded disabled:bg-gray-300"
                >Next</button>
            </div>
        </div>
    );
}