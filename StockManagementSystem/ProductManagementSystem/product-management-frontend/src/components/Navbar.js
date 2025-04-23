import React from "react";
import { Link, useNavigate } from "react-router-dom";
import "./Navbar.css";

const Navbar = () => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem("token");
        navigate("/login");
    };

    return (
        <nav className="navbar">
            <div className="nav-left">
                <Link to="/products" className="nav-link">Products</Link>
                <Link to="/my-orders" className="nav-link">My Orders</Link>
            </div>
            <div className="nav-right">
                <button className="logout-button" onClick={handleLogout}>Logout</button>
            </div>
        </nav>
    );
};

export default Navbar;
