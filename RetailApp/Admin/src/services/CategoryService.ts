import axiosClient from "../api/axiosClient";
import type { Category } from "../types";

export const CategoryService = {
    getAll: () => axiosClient.get<Category[]>('/category'),
    getById: (id: number) => axiosClient.get<Category>(`/category/${id}`),
    create: (category: Omit<Category, 'id'>) => axiosClient.post<Category>('/category', category),
    update: (category: Category) => axiosClient.put<Category>(`/category/${category.id}`, category),
    delete: (id: number) => axiosClient.delete<void>(`/category/${id}`)
}