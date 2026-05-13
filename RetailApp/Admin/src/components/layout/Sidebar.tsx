import { ClipboardList, LayoutDashboard, LogOut, Package, Tags, Users } from "lucide-react";
import { Link, useLocation } from "react-router-dom";

const menuItems = [
    { icon: LayoutDashboard, label: 'Dashboard', path: '/' },
    { icon: Tags, label: 'Category', path: '/category' },
    { icon: Package, label: 'Product', path: '/product' },
    { icon: Users, label: 'Customer', path: '/customer' },
    { icon: ClipboardList, label: 'Order', path: '/order' },
];

const Sidebar = () => {
    const location = useLocation();

    return (
        <aside className="w-64 bg-slate-900 text-white h-screen fixed left-0 top-0 flex flex-col">
            <div className="p-6 text-2xl font-bold border-b border-slate-800 text-blue-400">
                Retail <span className="text-white">Admin</span>
            </div>

            <nav className="flex-1 p-4 space-y-2 mt-4">
                {menuItems.map(item => {
                    const isActive = location.pathname === item.path;
                    return (
                        <Link
                            key={item.path}
                            to={item.path}
                            className={`flex items-center space-x-3 p-3 rounded-lg transition-colors ${
                                isActive 
                                ? 'bg-blue-600 text-white' 
                                : 'text-slate-400 hover:bg-slate-800 hover:text-white'
                            }`}
                        >
                            <item.icon size={20} />
                            <span className="font-medium">{item.label}</span>
                        </Link>
                    );
                })}
            </nav>

            <div className="p-4 border-t border-slate-800">
                <button className="flex items-center space-x-3 p-3 w-full text-slate-400 hover:text-red-400 transition-colors">
                    <LogOut size={20} />
                    <span className="font-medium">Logout</span>
                </button>
            </div>
        </aside>
    );
};

export default Sidebar;