const API_BASE_URL = 'http://localhost:5064/api';

// --- Προϊόντα ---
async function fetchProducts() {
    try {
        const response = await fetch(`${API_BASE_URL}/products`);
        if (!response.ok) throw new Error('Σφάλμα κατά τη λήψη των προϊόντων');
        return await response.json();
    } catch (error) {
        console.error("Σφάλμα κατά τη λήψη προϊόντων:", error);
        return null;
    }
}

// --- Καλάθι ---
async function fetchCart(userId) {
    try {
        const response = await fetch(`${API_BASE_URL}/cart/${userId}`);
        if (!response.ok) return null;
        return await response.json();
    } catch (error) {
        console.error("Σφάλμα κατά τη φόρτωση του καλαθιού:", error);
        return null;
    }
}

async function addToCartAPI(userId, productId, quantity, price = 0) {
    try {
        const response = await fetch(`${API_BASE_URL}/cart/add`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId, productId, quantity, price })
        });
        return response.ok;
    } catch (error) {
        console.error("Σφάλμα κατά την προσθήκη στο καλάθι:", error);
        return false;
    }
}

async function removeFromCartAPI(itemId) {
    try {
        const response = await fetch(`${API_BASE_URL}/cart/remove/${itemId}`, { method: 'DELETE' });
        return response.ok;
    } catch (error) {
        console.error("Σφάλμα κατά την αφαίρεση προϊόντος:", error);
        return false;
    }
}

// --- Παραγγελίες ---
async function placeOrder(orderDetails) {
    try {
        const response = await fetch(`${API_BASE_URL}/orders`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(orderDetails)
        });

        if (!response.ok) {
            const errorData = await response.json();
            alert(errorData.message || 'Πρέπει να συνδεθείτε για να ολοκληρώσετε την παραγγελία.');
            return null;
        }

        return await response.json();
    } catch (error) {
        console.error("Σφάλμα κατά την υποβολή παραγγελίας:", error);
        return null;
    }
}

// --- Χρήστες / Αυθεντικοποίηση ---
async function loginUser(credentials) {
    try {
        const response = await fetch(`${API_BASE_URL}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(credentials)
        });

        const data = await response.json();

        if (!response.ok) {
            alert(data.message || 'Σφάλμα κατά τη σύνδεση');
            return null;
        }

        return data;
    } catch (error) {
        console.error("Σφάλμα κατά τη σύνδεση:", error);
        return null;
    }
}

async function registerUser(userDetails) {
    try {
        const response = await fetch(`${API_BASE_URL}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(userDetails)
        });

        const data = await response.json();

        if (!response.ok) {
            alert(data.message || 'Σφάλμα κατά την εγγραφή');
            return null;
        }

        return data;
    } catch (error) {
        console.error("Σφάλμα κατά την εγγραφή:", error);
        return null;
    }
}

function logoutUser() {
    fetch(`${API_BASE_URL}/auth/logout`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
    }).then(() => {
        alert("Αποσύνδεση επιτυχής!");
        localStorage.removeItem("authToken");
        window.location.href = "index.html";
    }).catch(error => {
        console.error("Σφάλμα κατά την αποσύνδεση:", error);
    });
}
