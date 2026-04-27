import { Route, Routes } from "react-router";
import AccountList from "../pages/AccountList";
import AccountDetails from "../pages/AccountDetails";

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<AccountList />} />
            <Route path="/:accountNumber/details" element={<AccountDetails />} />
        </Routes>
    );
}