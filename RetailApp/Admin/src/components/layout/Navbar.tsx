import { Bell, UserCircle } from "lucide-react";

const Navbar = () => {
    return (
        <header className="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-8 sticky top-0 z-10">
            {/* Blank */}
            <div className="relative w-96"></div>

            <div className="flex items-center space-x-6">
                <button className="text-slate-500 hover:text-blue-600 relative">
                    <Bell size={22}/>
                    <span className="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] rounded-full h-4 w-4 flex items-center justify-center">3</span>
                </button>
                <div className="flex items-center space-x-3 border-l pl-6 border-slate-200">
                    <div className="text-right">
                        <p className="text-sm font-medium text-slate-900">Admin User</p>
                        <p className="text-xs text-slate-500">IT Manager</p>
                    </div>
                    <UserCircle size={32} className="text-slate-400" />
                </div>
            </div>
        </header>
    );
}

export default Navbar;