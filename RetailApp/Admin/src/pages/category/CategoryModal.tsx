import { useForm } from "react-hook-form";
import type { Category } from "../../types";
import { CategoryService } from "../../services/CategoryService";
import { X } from "lucide-react";
import { useEffect } from "react";

interface Props {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
    initialData?: Category | null;
}

const CategoryModal = ({ isOpen, onClose, onSuccess, initialData }: Props) => {
    const { register, handleSubmit, reset, formState: { errors } } = useForm<Omit<Category, 'id'>>();

    useEffect(() => {
        if (initialData) reset(initialData);
        else reset({ name: '', description: '' });
    }, [initialData, reset]);

    if (!isOpen) return null;

    const onSubmit = async (data: Omit<Category, 'id'>) => {
        try {
            if (initialData) await CategoryService.update({ ...data, id: initialData.id });
            else await CategoryService.create(data);
            reset();
            onSuccess();
            onClose();
        } catch (error) {
            alert("Failed to add category: " + (error instanceof Error ? error.message : "Unknown error"));
        }
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 bg-opacity-50 backdrop-blur-sm p-4">
            <div className="bg-white rounded-xl shadow-xl w-full max-w-md overflow-y-auto flex flex-col">
                {/* Header */}
                <div className="flex justify-between items-center p-6 border-b">
                    <h2 className="text-xl font-bold !text-slate-800">
                        {initialData ? "Edit Category" : "Add New Category"}
                    </h2>
                    <button title="Exit" type="button" onClick={onClose} className="text-slate-400 hover:text-slate-600">
                        <X size={24} />
                    </button>
                </div>
                
                {/* Form */}
                <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-slate-700 mb-1">
                            <span>Name</span>
                            <input
                                {...register("name", { required: "Category name is required" })}
                                className={`w-full p-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none ${errors.name ? 'border-red-500' : 'border-slate-200'}`}
                                placeholder="e.g. Electronics"
                            />
                        </label>
                        
                        {errors.name && <p className="text-red-500 text-xs">{errors.name.message}</p>}
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700 mb-1">
                            <span>Description</span>
                            <textarea
                                {...register("description")}
                                className="w-full p-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none h-24"
                                placeholder="Briefly describe the category..."
                            />
                        </label>
                    </div>

                    <div className="flex space-x-3 pt-4">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 border border-slate-200 rounded-lg text-slate-600 hover:bg-slate-50 transition"
                        >Cancel</button>
                        <button
                            type="submit"
                            className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                        >
                            {initialData ? "Save" : "Add"}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default CategoryModal;