import { useCallback, useEffect, useState } from "react";
import type { Product } from "../../types";
import { ProductService } from "../../services/ProductService";
import { Edit2, Loader2, Package, Plus, Search, Trash2 } from "lucide-react";
import ProductModal from "./ProductModal";
import { getImageUrl } from "../../utils/imageHelper";

const ProductList = () =>  {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
    const [searchTerm, setSearchTerm] = useState("");

    const fetchProduct = useCallback(async () => {
        setLoading(true);
        try {
            await ProductService.getAll().then(res => {
                setProducts(res.data);
            });
        } catch (error) {
            console.error("Failed to fetch products: ", error);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        let isMounted = true;

        ProductService.getAll().then(res => {
            if (isMounted) {
                setProducts(res.data);
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

    const handleAdd = () => {
        setSelectedProduct(null);
        setIsModalOpen(true);
    }

    const handleEdit = (product: Product) => {
        setSelectedProduct(product);
        setIsModalOpen(true);
    }

    const handleDelete = async (id: number) => {
        if (!globalThis.confirm("Are you sure you want to delete this product?")) return;
        try {
            await ProductService.delete(id);
            setProducts(prev => prev.filter(p => p.id !== id));
        } catch (error) {
            console.error("Failed to delete product: ", error);
        }
    }

    const filteredProducts = products.filter(
        p => p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
             p.categoryName?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <div className="w-full space-y-6">
            {/* Header */}
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold !text-slate-800">Products</h1>
                <button 
                    className="flex items-center bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
                    onClick={handleAdd}
                >
                    <Plus size={20} className="mr-2"/> Add Product
                </button>
            </div>

            {/* Filter and Searchbar */}
            <div className="flex items-center bg-white p-4 rounded-xl border border-slate-200 shadow-sm">
                <div className="relative flex-1 max-w-md">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400" size={18} />
                    <input 
                        type="text"
                        className="w-full pl-10 pr-4 py-2 bg-slate-50 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none border-slate-200 transition-all"
                        value={searchTerm}
                        onChange={e => setSearchTerm(e.target.value)}
                        placeholder="Search by name or category..."
                    />
                </div>
            </div>

            {/* Table */}
            <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-slate-50 border-b border-slate-200">
                        <tr>
                            <th className="p-4 text-slate-600 uppercase tracking-wide">Product</th>
                            <th className="p-4 text-slate-600 uppercase tracking-wide">Category</th>
                            <th className="p-4 text-slate-600 uppercase tracking-wide">Price</th>
                            <th className="p-4 text-slate-600 uppercase tracking-wide">Stock Qty.</th>
                            <th className="p-4 text-slate-600 uppercase tracking-wide">Timeline</th>
                            <th className="p-4 text-slate-600 uppercase tracking-wide">Actions</th>
                        </tr>
                    </thead>

                    <tbody className="divide-y divide-slate-200">
                        {filteredProducts.map(product => (
                            <tr key={product.id} className="hover:bg-slate-50 transition">
                                {/* First column with image and name */}
                                <td className="p-4">
                                    {/* Image */}
                                    <div className="flex items-center space-x-3">
                                        <div className="w-12 h-12 border border-slate-200 bg-slate-100 rounded-lg overflow-hidden mr-4 flex-shrink-0">
                                            { product.imageUrl ? (
                                                <img 
                                                    src={getImageUrl(product.imageUrl)}
                                                    alt={product.name}
                                                    className="w-full h-full object-cover"
                                                    onError={e => (e.currentTarget.src = "https://placehold.co/100x100?text=No+Img")}
                                                />
                                            ) : (
                                                <div className="w-full h-full flex items-center justify-center text-slate-400">
                                                    <Package size={24} />
                                                </div>
                                            ) }
                                        </div>
                                    </div>
                                    {/* Name */}
                                    <span className="font-medium text-slate-900">{product.name}</span>
                                </td>

                                {/* Category */}
                                <td className="p-4 text-slate-600">{product.categoryName || 'N/A'}</td>
                            
                                {/* Price */}
                                <td className="p-4 text-slate-600">${product.price.toFixed(2)}</td>

                                {/* Stock Quantity */}
                                <td className="p-4 text-slate-600">
                                    <span className={`${product.stockQuantity <= 5 ? 'text-red-500 font-bold' : 'text-slate-600'}`}>
                                        {product.stockQuantity}
                                    </span>
                                </td>
                                
                                {/* Timeline */}
                                <td className="p-4 text-xs text-slate-500">
                                    <div className="flex flex-col">
                                        <span className="text-sm">
                                            <span className="font-semibold uppercase">Created:</span> {new Date(product.createdDate).toLocaleDateString()}
                                        </span>
                                        <span className="text-sm">
                                            <span className="font-semibold uppercase">Updated:</span> {new Date(product.updatedDate).toLocaleDateString()}
                                        </span>
                                    </div>
                                </td>

                                <td className="p-4 text-slate-600">
                                    <button className="p-2 text-slate-400 hover:text-blue-600 transition" 
                                            title="Edit" 
                                            onClick={() => handleEdit(product)}
                                    >
                                        <Edit2 size={18} />
                                    </button>
                                    <button className="p-2 text-slate-400 hover:text-red-600 transition" 
                                            title="Delete" 
                                            onClick={() => handleDelete(product.id)}
                                    >
                                        <Trash2 size={18} />
                                    </button>
                                </td>
                            </tr>
                        ))}

                        {filteredProducts.length === 0 && (
                            <tr>
                                <td colSpan={5} className="p-12 text-center text-slate-400">
                                    No products found.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            <ProductModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSuccess={fetchProduct}
                initialData={selectedProduct}
            />
        </div>
    );
}

export default ProductList;