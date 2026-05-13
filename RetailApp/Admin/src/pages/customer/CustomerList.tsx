import { useEffect, useState } from "react";
import type { Customer } from "../../types";
import { CustomerService } from "../../services/CustomerService";
import { Loader2 } from "lucide-react";

const CustomerList = () => {
    const [customers, setCustomers] = useState<Customer[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        let isMounted = true;

        CustomerService.getAll().then(res => {
            if (isMounted) {
                setCustomers(res.data);
                setLoading(false);
            }
        });

        return () => { isMounted = false; };
    }, []);

    if (loading) {
        return (
            <div className="flex justify-center items-center h-64">
                <Loader2 className="animate-spin text-blue-600" size={40} />
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold !text-slate-800">Customers</h1>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-slate-50 border-b border-slate-200">
                        <tr>
                            <th className="p-4 font-semibold text-slate-600">ID</th>
                            <th className="p-4 font-semibold text-slate-600">Full Name</th>
                            <th className="p-4 font-semibold text-slate-600">Email</th>
                            <th className="p-4 font-semibold text-slate-600 text-right">Phone Number</th>  
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200">
                        {customers.map(customer => (
                            <tr key={customer.id} className="hover:bg-slate-50 transition">
                                <td className="p-4">#{customer.id}</td>
                                <td className="p-4 font-medium text-slate-900">{customer.firstName} {customer.lastName}</td>
                                <td className="p-4 text-slate-600">{customer.email}</td>
                                <td className="p-4 text-slate-600 text-right">{customer.phoneNumber || 'N/A'}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default CustomerList;