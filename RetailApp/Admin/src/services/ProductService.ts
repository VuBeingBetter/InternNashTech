import axiosClient from "../api/axiosClient";
import type { Product } from "../types";

export const ProductService = {
    getAll: () => axiosClient.get<Product[]>('/product'),
    getById: (id: number) => axiosClient.get<Product>(`/product/${id}`),
    create: (product: FormData) => axiosClient.post<Product>('/product', product),
    update: (id: number, product: FormData) => axiosClient.put<Product>(`/product/${id}`, product),
    delete: (id: number) => axiosClient.delete<void>(`/product/${id}`)
}