export interface ProductFormValues {
    name: string;
    price: number;
    stockQuantity: number;
    imageUrl: string;
    categoryId?: number;
    specFields: SpecField[]; // Đảm bảo SpecField đã được import
}

export interface SpecField {
    key: string;
    value: string;
}