import { BrowserRouter, Route, Routes } from "react-router-dom";
import AdminLayout from "../components/layout/AdminLayout";
import CategoryList from "../pages/category/CategoryList";
import ProductList from "../pages/product/ProductList";
import CustomerList from "../pages/customer/CustomerList";

const AppRoutes = () => (
    <BrowserRouter>
        <Routes>
            <Route path="/" element={<AdminLayout />}>
                <Route index element={<div>Dashboard Page</div>}/>
                <Route path="category" element={<CategoryList/>}/>
                <Route path="product" element={<ProductList/>}/>
                <Route path="customer" element={<CustomerList/>}/>
                <Route path="order" element={<div>Order Page</div>}/>
                <Route path="*" element={<div>Page Not Found</div>}/>
            </Route>
            
        </Routes>
    </BrowserRouter>
);

export default AppRoutes;