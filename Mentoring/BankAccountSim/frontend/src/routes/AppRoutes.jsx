import { Route, Routes } from "react-router";
import AccountList from "../pages/AccountList";
import AccountDetails from "../pages/AccountDetails";
import TransactionHistory from "../pages/TransactionHistory";

export default function AppRoutes() {
    return (
        <Routes>
            <Route path="/" element={<AccountList />} />
            <Route path="/:accountNumber/details" element={<AccountDetails />} />
            <Route path="/:accountNumber/history" element={<TransactionHistory />} />
        </Routes>
    );
}