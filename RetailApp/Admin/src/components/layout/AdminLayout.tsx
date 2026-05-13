import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";
import Sidebar from "./Sidebar";

const AdminLayout = () => {
    return (
        <div className="flex min-h-screen w-full bg-slate-50">
            <div className="w-64 fixed inset-y-0 left-0">
                <Sidebar />
            </div>

            {/* Main Area */}
            <div className="ml-64 flex-1 flex flex-col min-h-screen">
                <Navbar/>

                <main className="p-8 w-full">
                    <Outlet/>
                </main>
            </div>
        </div>
    );
};

export default AdminLayout;