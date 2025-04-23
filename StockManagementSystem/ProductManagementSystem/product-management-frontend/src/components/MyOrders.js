import React, { useEffect, useState } from "react";
import "./MyOrders.css";

const MyOrders = () => {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");


    useEffect(() => {
        const fetchOrders = async () => {
            try {
                const token = localStorage.getItem("token");
                if (!token) {
                    setError("User not authenticated. Please log in.");
                    setLoading(false);
                    return;
                }

                const response = await fetch("http://localhost:5016/api/orders/my-orders", {
                    method: "GET",
                    headers: {
                        "Authorization": `Bearer ${token}`,
                        "Content-Type": "application/json"
                    }
                });

                console.log("Fetch orders response status:", response.status);

                if (!response.ok) {
                    throw new Error(`Failed to fetch orders: ${response.statusText}`);
                }

                const data = await response.json();
                console.log("Fetched orders:", data);
                setOrders(data);
            } catch (error) {
                console.error("Fetch error:", error);
                setError(error.message);
            } finally {
                setLoading(false);
            }
        };

        fetchOrders();
    }, []);


    const formatDate = (dateStr) => {
        const date = new Date(dateStr);
        return date.toLocaleString(); 
    };

    return (
        <div className="orders-container">
            <h2 className="orders-heading">My Orders</h2>

            {loading ? (
                <p className="loading-message">Loading orders...</p>
            ) : error ? (
                <p className="error-message">{error}</p>
            ) : orders.length === 0 ? (
                <p className="no-orders-message">No orders found.</p>
            ) : (
                <div className="orders-grid">
                    {orders.map((order) => (
                        <div key={order.orderId} className="order-card">
                            <p><strong>Product:</strong> {order.productName}</p>
                            <p><strong>Quantity:</strong> {order.quantity}</p>
                            <p><strong>Total Price:</strong> ${order.totalPrice}</p>
                            <p><strong>Order Date:</strong> {formatDate(order.orderDate)}</p>
                            <p>
                                <strong>Status:</strong>{" "}
                                <span style={{ color: order.status === 'Success' ? 'green' : 'red' }}>
                                     {order.status}
                                </span>
                            </p>
                        </div>
                     ))}
                </div>
            )}
        </div>
    );
};

export default MyOrders;
