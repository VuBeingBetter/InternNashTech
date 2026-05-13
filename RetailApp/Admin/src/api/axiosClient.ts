import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_BASE_URL;

const axiosClient = axios.create({
    baseURL: BASE_URL + '/api',
});

export default axiosClient;