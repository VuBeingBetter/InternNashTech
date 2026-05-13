const BASE_URL = import.meta.env.VITE_API_BASE_URL;

export const getImageUrl = (imagePath: string | null | undefined): string => {
    // No Image
    if (!imagePath) return 'https://placehold.co/200x200?text=No+Image';

    // Image from Internet
    if (imagePath.startsWith('http')) return imagePath;

    // Image from Server
    return `${BASE_URL}/images/products/${imagePath}`;
}