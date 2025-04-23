import axios from "axios";

const API_URL = "http://localhost:5016/api"; 

export const fetchProducts = async () => {
    const response = await fetch(`${API_URL}/products`, {
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });
    if (!response.ok) throw new Error("Failed to fetch products");
    return await response.json();
};

export const placeOrder = async (productId, quantity) => {
    const response = await fetch(`${API_URL}/orders`, {
        method: "POST",
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`,
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ productId, quantity })
    });

    return await response.json();
};

export const fetchCustomerOrders = async () => {
    const response = await fetch(`${API_URL}/orders/myorders`, {
        headers: {
            "Authorization": `Bearer ${localStorage.getItem("token")}`
        }
    });
    if (!response.ok) throw new Error("Failed to fetch orders");
    return await response.json();
};


const getProducts = async () => {
    const response = await axios.get(API_URL);
    return response.data;
};


export default { getProducts }; 
