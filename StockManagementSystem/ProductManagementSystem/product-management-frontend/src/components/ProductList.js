import React, { useEffect, useState } from "react";
import { startSignalRConnection, subscribeToStockUpdate, subscribeToLowStockAlert } from '../services/SignalRService';
import { useLocation, useNavigate } from "react-router-dom";
import "./ProductList.css";

const ProductList = () => {
    const [products, setProducts] = useState([]);
    const [filteredProducts, setFilteredProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [customerName, setCustomerName] = useState("User");
    const [showModal, setShowModal] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState(null);
    const [quantity, setQuantity] = useState(1);
    const [message, setMessage] = useState("");
    const [isPlacingOrder, setIsPlacingOrder] = useState(false);
    const [filters, setFilters] = useState({ name: "", maxPrice: ""});
    const [currentPage, setCurrentPage] = useState(1);
    const productsPerPage = 8;

    const location = useLocation();
    const navigate = useNavigate();

    const fetchProducts = async () => {
        try {
            const token = localStorage.getItem("token");
            const response = await fetch("http://localhost:5016/api/products", {
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) throw new Error("Failed to fetch products");

            const data = await response.json();
            setProducts(data);
            setFilteredProducts(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        startSignalRConnection();

        const storedName = location.state?.customerName || sessionStorage.getItem("fullName");

        if (location.state?.customerName) {
            sessionStorage.setItem("fullName", location.state.customerName);
        }

        if (storedName) {
            setCustomerName(storedName);
        }

        fetchProducts();

        subscribeToStockUpdate((data) => {
            console.log("Stock update received:", data);

            if (data && data.productId && data.stockQuantity !== undefined) {
                const { productId, stockQuantity } = data;

                console.log(`Stock updated for: ${productId}, New Quantity: ${stockQuantity}`);

                setProducts((prevProducts) =>
                    prevProducts.map((p) =>
                        p.productId === productId ? { ...p, stockQuantity: stockQuantity } : p
                    )
                );

                setFilteredProducts((prevFiltered) =>
                    prevFiltered.map((p) =>
                        p.productId === productId ? { ...p, stockQuantity: stockQuantity } : p
                    )
                );
            }
        });



        subscribeToLowStockAlert((alertData) => {
            // We can show a toast/alert here or mark product as low stock
            alert(`Low Stock Alert! Product: ${alertData.productName}, Quantity: ${alertData.stockQuantity}`);

            setProducts((prevProducts) =>
                prevProducts.map((p) =>
                    p.productId === alertData.productId
                        ? { ...p, stockQuantity: alertData.stockQuantity }
                        : p
                )
            );

            setFilteredProducts((prevFiltered) =>
                prevFiltered.map((p) =>
                    p.productId === alertData.productId
                        ? { ...p, stockQuantity: alertData.stockQuantity }
                        : p
                )
            );
        });

    }, [location.state]);

    const indexOfLastProduct = currentPage * productsPerPage;
    const indexOfFirstProduct = indexOfLastProduct - productsPerPage;
    const currentProducts = filteredProducts.slice(indexOfFirstProduct, indexOfLastProduct);
    const totalPages = Math.ceil(filteredProducts.length / productsPerPage);

    const prevPage = () => {
        if (currentPage > 1) setCurrentPage(currentPage - 1);
    };

    const goToPage = (pageNumber) => setCurrentPage(pageNumber);

    const nextPage = () => {
        if (currentPage < totalPages) setCurrentPage(currentPage + 1);
    };

    const handleFilterChange = (e) => {
        const { name, value } = e.target;
        setFilters((prev) => ({
            ...prev,
            [name]: value
        }));
    };

    const fetchFilteredProducts = async () => {
        const token = localStorage.getItem("token");

        const queryParams = new URLSearchParams();
        if (filters.name) queryParams.append("search", filters.name);
        if (filters.maxPrice) queryParams.append("maxPrice", filters.maxPrice);
       
        try {
            const response = await fetch(`http://localhost:5016/api/products?${queryParams.toString()}`, {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

            if (!response.ok) throw new Error("Failed to apply filters");

            const data = await response.json();
            setProducts(data);
            setFilteredProducts(data);
            setCurrentPage(1);
        } catch (error) {
            setError(error.message);
        }
    };

    const openModal = (product) => {
        setSelectedProduct(product);
        setQuantity(1);
        setMessage("");
        setShowModal(true);
    };

    const handleModalBuy = async () => {
        if (!selectedProduct || quantity <= 0) {
            setMessage("Please enter a valid quantity.");
            return;
        }

        await placeOrder(selectedProduct.productId, quantity);
        setShowModal(false);
    };

    const placeOrder = async (productId, qty) => {
        const token = localStorage.getItem("token");
        setIsPlacingOrder(true);

        try {
            const response = await fetch("http://localhost:5016/api/orders/place", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`
                },
                body: JSON.stringify({ productId, quantity: qty })
            });

            const data = await response.json();
            console.log("Order response:", data);

            if (response.ok && data.success) {
                alert("Order placed successfully!");
                navigate("/my-orders");
            } else {
                alert("Order failed: " + (data.message || "Unknown error"));
            }
        } catch (error) {
            console.error("Order error:", error);
            alert("Something went wrong. Please try again.");
        } finally {
            setIsPlacingOrder(false);
        }
    };

    return (
        <div className="product-list-container">
            <h1>Welcome {customerName}</h1>

            <div className="filter-bar">
                <div className="filter-fields-container">
                <div className="filter-field">
                    <label htmlFor="name">Name</label>
                    <input type="text" name="name" placeholder="e.g. Smart" value={filters.name.substring(0, 1).toUpperCase() + filters.name.substring(1).toLowerCase()} onChange={handleFilterChange} />
                </div>

                <div className="filter-field">
                    <label htmlFor="maxPrice">Max Price</label>
                    <input type="number" name="maxPrice" placeholder="e.g. 1000" value={filters.maxPrice} onChange={handleFilterChange} />
                </div>
                </div>
                <div className="button-container">
                    <button className="btn" onClick={fetchFilteredProducts}>Apply Filters</button>
                </div>
            </div>

            {loading ? (
                <p>Loading products...</p>
            ) : error ? (
                <p className="error-message">{error}</p>
            ) : (
                <>
                    <div className="scrollable-products-container">
                        <div className="product-grid">
                            {currentProducts.map((product) => (
                                <div className="product-card" key={product.productId}>
                                    <h3>{product.name}</h3>
                                    <p>{product.description}</p>
                                    <p>Price: ${product.price}</p>
                                    <p>
                                        {product.stockQuantity === 0 ? (
                                            <span className="out-of-stock">Out of Stock</span>
                                        ) : (
                                            <>Stock: {product.stockQuantity}</>
                                        )}
                                    </p>
                                    <button className="btnBuy"
                                        disabled={product.stockQuantity === 0 || isPlacingOrder}
                                        onClick={() => openModal(product)}
                                        style={{
                                            backgroundColor: product.stockQuantity === 0 ? 'red' : '',
                                            color: product.stockQuantity === 0 ? 'white' : ''
                                        }}
                                    >
                                        {product.stockQuantity === 0 ? "Out of Stock" : "Buy"}
                                    </button>
                                </div>
                            ))}
                        </div>
                    </div>

                    {/* Modal: OUTSIDE map loop */}
                    {showModal && selectedProduct && (
                        <div className="modal-overlay">
                            <div className="modal-content">
                                <h2>Enter the Quantity</h2>
                                <p><strong>Product:</strong> {selectedProduct.name}</p>
                                <input
                                    type="number"
                                    min="1"
                                    max={selectedProduct.stockQuantity}
                                    value={quantity}
                                    onChange={(e) => setQuantity(Number(e.target.value))}
                                />
                                <div className="modal-buttons">
                                    <button onClick={handleModalBuy}>Confirm</button>
                                    <button onClick={() => setShowModal(false)}>Cancel</button>
                                </div>
                                {message && <p className="modal-message">{message}</p>}
                            </div>
                        </div>
                    )}

                    {/* Pagination */}
                    {totalPages > 1 && (
                        <div className="pagination">
                            <span className={`pagination-arrow ${currentPage === 1 ? "disabled" : ""}`} onClick={prevPage}>&laquo;</span>
                            {[...Array(totalPages)].map((_, index) => (
                                <span
                                    key={index}
                                    className={`pagination-number ${currentPage === index + 1 ? "active" : ""}`}
                                    onClick={() => goToPage(index + 1)}
                                >
                                    {index + 1}
                                </span>
                            ))}
                            <span className={`pagination-arrow ${currentPage === totalPages ? "disabled" : ""}`} onClick={nextPage}>&raquo;</span>
                        </div>
                    )}
                </>
            )}
        </div>
    );
};

export default ProductList;