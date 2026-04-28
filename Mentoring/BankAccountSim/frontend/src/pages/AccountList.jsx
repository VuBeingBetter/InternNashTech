import { useEffect, useState } from "react";
import { Link } from "react-router";
import axiosClient from "../api/axiosClient";

export default function AccountList() {
    const [accounts, setAccounts] = useState([]);
    
    const [search, setSearch] = useState("");
    const [debouncedSearch, setDebouncedSearch] = useState("");

    const [page, setPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const response = await axiosClient.get(`account?page=${page}&search=${debouncedSearch}`);
                setAccounts(response.data?.data || []);
                setTotalPages(response.data?.totalPages || 1);
            } catch (error) {
                console.error("Error fetching data:", error);
                setAccounts([]);
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, [page, debouncedSearch]);

    useEffect(() => {
        const timer = setTimeout(() => {
            setDebouncedSearch(search);
        }, 500);

        return () => clearTimeout(timer);
    }, [search]);

    return (
        <div className="p-6 max-w-6xl mx-auto">
            <h2 className="text-2xl font-bold mb-4 p-2">Bank Accounts</h2>

            {/* Search Bar */}
            <div className="mb-4 flex gap-2">
                <input
                    className="border p-2 rounded w-full"
                    placeholder="Search account..."
                    value={search}
                    onChange={(e) => {
                        setPage(1);
                        setSearch(e.target.value)
                    }}
                />
            </div>
            
            {/* Loading effect */}
            {loading ? (
                <div className="text-center p-10 text-gray-500">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500 mx-auto"></div>
                    <p>Loading accounts...</p>
                </div>
            ) : (
                // Account List
                <table className="w-full border-collapse border border-gray-300 table-fixed">
                    <thead className="bg-gray-800 text-white">
                        <tr>
                            <th className="p-2">Account</th>
                            <th className="p-2">Owner</th>
                            <th className="p-2">Balance</th>
                            <th className="p-2">Status</th>
                        </tr>
                    </thead>

                    <tbody>
                        {accounts?.map(account => (
                            <tr key={account.accountNumber} className="border-b">
                                <td className="p-2 text-left truncate">{account.accountNumber}</td>
                                <td className="p-2 text-left">{account.ownerName}</td>
                                <td className="p-2 text-left">${account.balance}</td>
                                <td className="p-2">
                                    <span className={account.status === "ACTIVE" ? "text-green-600" : "text-red-600"}>
                                        {account.status}
                                    </span>
                                </td>
                                <td className="p-2 text-blue-500 hover:underline cursor-pointer">
                                    <Link to={`/${account.accountNumber}/details`}>Details</Link>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
            

            {/* Pagination */}
            <div className="mt-4 flex gap-2 justify-center">
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