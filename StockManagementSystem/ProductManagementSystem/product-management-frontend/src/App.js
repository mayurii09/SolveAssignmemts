import React from "react";
import { Routes, Route, Navigate, useLocation } from "react-router-dom";
import Login from "./components/Login";
import Register from "./components/Register";
import ProductList from "./components/ProductList";
import MyOrders from "./components/MyOrders";
import Navbar from "./components/Navbar";

const isAuthenticated = () => {
    return localStorage.getItem("token") !== null;
};

const ProtectedRoute = ({ element }) => {
    return isAuthenticated() ? element : <Navigate to="/login" replace />;
};


const App = () => {
    const location = useLocation();
    const hideNavbarRoutes = ["/login", "/register", "/"];

    return (
        <div>
            {!hideNavbarRoutes.includes(location.pathname) && isAuthenticated() && (
                <>
                    <Navbar />
                </>
            )}

            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/products" element={<ProtectedRoute element={<ProductList />} />} />
                <Route path="/my-orders" element={<ProtectedRoute element={<MyOrders />} />} />
            </Routes>
        </div>
    );
};

export default App;
