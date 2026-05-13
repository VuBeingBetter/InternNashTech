import { Controller, useForm } from "react-hook-form";
import { useEffect, useState } from "react";
import { X } from "lucide-react";
import Select from "react-select";

import type { Product } from "../../types";
import { ProductService } from "../../services/ProductService";
import { CategoryService } from "../../services/CategoryService";
import type { SpecField, ProductFormValues } from "../../components/common/ProductFormValues";
import JsonSpecEditor from "./JsonSpecEditor";
import { getImageUrl } from "../../utils/imageHelper";

interface Props {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
    initialData?: Product | null;
}

const ProductModal = ({ isOpen, onClose, onSuccess, initialData }: Props) => {
    const { register, handleSubmit, control, reset, formState: { errors } } = useForm<ProductFormValues>({
        defaultValues: {
            name: '',
            price: 0,
            stockQuantity: 0,
            imageUrl: '',
            specFields: []
        }
    });
    const [categories, setCategories] = useState<{ value: number; label: string }[]>([]);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [previewUrl, setPreviewUrl] = useState<string>('');

    // Fetch categories for dropdown
    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await CategoryService.getAll();
                setCategories(response.data.map((cat: { id: number; name: string }) => ({ value: cat.id, label: cat.name })));
            } catch (error) {
                console.error("Failed to fetch categories: ", error);
            }
        };

        if (isOpen) fetchCategories();
    }, [isOpen]);

    // Reset when open modal
    useEffect(() => {
        if (isOpen) {
            if (initialData) {
                // Parse description from JSON string to array of {key, value} for form
                let initialSpecs: SpecField[] = [];
                try {
                    const desc = initialData.description;
                    const parsedDesc = typeof desc === 'string' ? JSON.parse(desc) : desc;
                    
                    if (parsedDesc) {
                        initialSpecs = Object.entries(parsedDesc).map(([key, value]) => ({ 
                            key, 
                            value: String(value) 
                        }));
                    }
                } catch (e) {
                    console.error("Failed to parse description JSON", e);
                }

                reset({ ...initialData, specFields: initialSpecs });
            } else {
                reset({ name: '', price: 0, stockQuantity: 0, imageUrl: '', categoryId: undefined, specFields: [] });
            }
        } 
        
        return () => {
            setSelectedFile(null);
            setPreviewUrl(prev => {
                if (prev) URL.revokeObjectURL(prev);
                return '';
            });
        };
        
    }, [initialData, reset, isOpen]);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        e.preventDefault();
        const file = e.target.files?.[0];
        if (file) {
            if (previewUrl) URL.revokeObjectURL(previewUrl);
            setSelectedFile(file);
            setPreviewUrl(URL.createObjectURL(file)); // Temp URL for preview
        }
    }

    // This does not depend on useEffect because we want to revoke URL immediately when closing modal, not waiting for component unmount
    const handleClose = () => {
        reset(); // Reset react-hook-form
        setSelectedFile(null);
        if (previewUrl) {
            URL.revokeObjectURL(previewUrl);
            setPreviewUrl('');
        }
        onClose();
    };

    if (!isOpen) return null;

    const onSubmit = async (data: ProductFormValues) => {
        const formData = new FormData();

        if (initialData) {
            formData.append('Id', initialData.id.toString());
        }

        // Append information
        formData.append('Name', data.name);
        formData.append('Price', Number(data.price).toFixed(2));
        formData.append('StockQuantity', Number(data.stockQuantity).toString());
        formData.append('CategoryId', data.categoryId?.toString() || '');

        // Convert specFields to JSON for backend
        const specObject: Record<string, string> = {};
        data.specFields.forEach(item => {
            if (item.key.trim()) specObject[item.key.trim()] = item.value;
        });
        formData.append('Description', JSON.stringify(specObject));

        if (selectedFile) {
            formData.append('ImageFile', selectedFile); // New image file
        } else if (initialData?.imageUrl) {
            formData.append('ImageUrl', initialData.imageUrl); // Keep existing image URL if no new file selected
        }

        try {
            if (initialData) {
                await ProductService.update(initialData.id, formData); 
            } else {
                await ProductService.create(formData);
            }
            onSuccess();
            onClose();
        } catch (error) {
            console.error(error);
        }
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 bg-opacity-50 backdrop-blur-sm p-4">
            <div className="bg-white rounded-xl shadow-xl w-full max-w-md max-h-[90vh] overflow-hidden flex flex-col">
                {/* Header */}
                <div className="flex justify-between items-center p-6 border-b shrink-0">
                    <h2 className="text-xl font-bold !text-slate-800">
                        {initialData ? "Edit Product" : "Add New Product"}
                    </h2>
                    <button title="Exit" type="button" onClick={handleClose} className="text-slate-400 hover:text-slate-600">
                        <X size={24} />
                    </button>
                </div>

                {/* Form */}
                <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-4 overflow-y-auto">
                    {/* Product Name */}
                    <div>
                        <label htmlFor="productName" className="block text-sm font-medium text-slate-700 mb-1">Name</label>
                        <input
                            id="productName"
                            {...register("name", { required: "Product name is required" })}
                            className={`w-full p-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none ${errors.name ? 'border-red-500' : 'border-slate-200'}`}
                            placeholder="e.g. iPhone 17 256GB"
                        />
                        {errors.name && <p className="text-red-500 text-xs">{errors.name.message}</p>}
                    </div>

                    {/* Price */}
                    <div>
                        <label htmlFor="productPrice" className="block text-sm font-medium text-slate-700 mb-1">Price</label>
                        <input
                            id="productPrice"
                            {...register("price", { 
                                required: "Product price is required", 
                                min: { value: 0, message: "Price must be greater than 0" },
                                valueAsNumber: true
                            })}
                            type="number"
                            step="0.01"
                            className={`w-full p-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none ${errors.price ? 'border-red-500' : 'border-slate-200'}`}
                            placeholder="e.g. 999.99"
                            onWheel={(e) => e.currentTarget.blur()}
                        />
                        {errors.price && <p className="text-red-500 text-xs">{errors.price.message}</p>}
                    </div>

                    {/* Stock Quantity */}
                    <div>
                        <label htmlFor="productStockQuantity" className="block text-sm font-medium text-slate-700 mb-1">Stock Quantity</label>
                        <input
                            id="productStockQuantity"
                            {...register("stockQuantity", { 
                                required: "Stock quantity is required" }
                            )}
                            type="number"
                            className={`w-full p-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none ${errors.stockQuantity ? 'border-red-500' : 'border-slate-200'}`}
                            placeholder="e.g. 50"
                            onWheel={(e) => e.currentTarget.blur()}
                        />
                        {errors.stockQuantity && <p className="text-red-500 text-xs">{errors.stockQuantity.message}</p>}
                    </div>
                    
                    {/* Image */}
                    <div className="space-y-2">
                        <label htmlFor="productImage" className="block text-sm font-medium text-slate-700 mb-2">Product Image</label>
                        <label className="cursor-pointer">
                            <span className="justify-center px-2 py-1 bg-white border border-slate-300 rounded-lg text-sm font-medium text-slate-700 hover:bg-slate-50 hover:border-blue-400 hover:text-blue-600 transition-all shadow-sm flex items-center gap-2">
                                Browse Image
                            </span>
                            <input
                                id="productImage"
                                {...register("imageUrl")}
                                type="file"
                                className="hidden"
                                onChange={handleFileChange}
                                accept="image/*"
                            />
                        </label>
                            
                        <p className="text-[10px] text-slate-400 mt-2 italic">Support: JPG, PNG, WebP (Max 2MB)</p>
                        <img 
                            src={previewUrl || getImageUrl(initialData?.imageUrl)} 
                            className="w-full h-full object-cover" 
                            alt="Preview" 
                            onError={(e) => {
                                e.currentTarget.src = 'https://placehold.co/400x300?text=No+Image+Available';
                            }}
                        />
                    </div>

                    {/* Category selection */}
                    <div className="md:col-span-2">
                        <label htmlFor="productCategory" className="block text-sm font-medium text-slate-700 mb-1">Category</label>
                        <Controller
                            {...register("categoryId", { required: "Please select a category" })}
                            name="categoryId"
                            control={control}
                            rules={{ required: "Please select a category" }}
                            render={({ field }) => (
                                <Select
                                    {...field}
                                    options={categories}
                                    value={categories.find(cat => cat.value === field.value) || null}
                                    onChange={(option) => field.onChange(option?.value)}
                                    placeholder="Select category..."
                                    classNamePrefix="react-select"
                                    isClearable
                                    isSearchable
                                />
                            )}    
                        />
                        {errors.categoryId && <p className="text-red-500 text-xs">{errors.categoryId.message}</p>}
                    </div>
                    
                    {/* Description (JSON Editor) */}
                    <div className="pt-2">
                        <label htmlFor="productDescription" className="block text-sm font-medium text-slate-700 mb-2">Descriptions (Specifications)</label>
                        <JsonSpecEditor control={control} name="specFields" />                        
                    </div>
                    
                    {/* Buttons */}
                    <div className="flex space-x-3 pt-4">
                        <button
                            type="button"
                            onClick={handleClose}
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
}

export default ProductModal;