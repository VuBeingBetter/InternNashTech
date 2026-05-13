export interface Category {
    id: number;
    name: string;
    description: string;
}

export interface Product {
    id: number;
    name: string;
    price: number;
    stockQuantity: number;
    description: string; // JSON
    imageUrl: string;
    categoryId: number;
    categoryName?: string;
    createdDate: string;
    updatedDate: string;
}

export interface Customer {
    id: number;
    firstName: string;
    lastName: string;
    fullName: string;
    email: string;
    phoneNumber?: string;
}