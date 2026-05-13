import axiosClient from "../api/axiosClient";
import type { Customer } from "../types";

export const CustomerService = {
    getAll: () => axiosClient.get<Customer[]>('/customer'),
    getById: (id: number) => axiosClient.get<Customer>(`/customer/${id}`)
};