import { useCallback, useEffect, useState } from "react"
import type { Category } from "../../types";
import { Edit2, Loader2, Plus, Trash2 } from "lucide-react";
import { CategoryService } from "../../services/CategoryService";
import CategoryModal from "./CategoryModal";

const CategoryList = () => {
    const [categories, setCategories] = useState<Category[]>([]);
    const [loading, setLoading] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
    
    // Fetch new list after add/edit
    const fetchCategories = useCallback(async () => {
        setLoading(true);
        try {
            const response = await CategoryService.getAll();
            setCategories(response.data);
        } catch (error) {
            console.error("Failed to fetch categories: ", error);
        } finally {
            setLoading(false);
        }
    }, []);
    
    // Initial fetch
    useEffect(() => {
        let isMounted = true;

        CategoryService.getAll().then(res => {
            if (isMounted) {
                setCategories(res.data);
                setLoading(false);
            }
        });

        return () => { isMounted = false; };
    }, []);

    // Loading state
    if (loading) {
        return (
            <div className="flex justify-center items-center h-64">
                <Loader2 className="animate-spin text-blue-600" size={40} />
            </div>
        );
    }

    // CRUD Handlers
    const handleEdit = (category: Category) => {
        setSelectedCategory(category);
        setIsModalOpen(true);
    };

    const handleAdd = () => {
        setSelectedCategory(null);
        setIsModalOpen(true);
    };

    const handleDelete = async (id: number) => {
        if (!globalThis.confirm("Are you sure you want to delete this category?")) return;

        try {
            await CategoryService.delete(id);
            fetchCategories();
        } catch (error) {
            alert("Failed to delete category: " + (error instanceof Error ? error.message : "Unknown error"));
        }
    }

    // Main render
    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold !text-slate-800">Categories</h1>
                <button 
                    className="flex items-center bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
                    onClick={() => handleAdd()}
                >
                    <Plus size={20} className="mr-2"/> Add Category
                </button>
            </div>

            <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-slate-50 border-b border-slate-200">
                        <tr>
                            <th className="p-4 font-semibold text-slate-600">ID</th>
                            <th className="p-4 font-semibold text-slate-600">Category Name</th>
                            <th className="p-4 font-semibold text-slate-600">Description</th>
                            <th className="p-4 font-semibold text-slate-600 text-right">Actions</th>  
                        </tr>
                    </thead>

                    <tbody className="divide-y divide-slate-200">
                        {categories.map((cat) => (
                        <tr key={cat.id} className="hover:bg-slate-50 transition">
                            <td className="p-4 text-slate-500">#{cat.id}</td>
                            <td className="p-4 font-medium text-slate-900">{cat.name}</td>
                            <td className="p-4 text-slate-600 max-w-xs truncate">{cat.description}</td>
                            <td className="p-4 text-right space-x-2">
                                <button className="p-2 text-slate-400 hover:text-blue-600 transition" 
                                        onClick={() => handleEdit(cat)}
                                        title="Edit">
                                    <Edit2 size={18} />
                                </button>
                                <button className="p-2 text-slate-400 hover:text-red-600 transition" 
                                        onClick={() => handleDelete(cat.id)}
                                        title="Delete">
                                    <Trash2 size={18} />
                                </button>
                            </td>
                        </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <CategoryModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSuccess={fetchCategories}
                initialData={selectedCategory}
            />
        </div>
    );
    
}

export default CategoryList;