// Έλεγχος (client-side) εάν ο χρήστης είναι συνδεδεμένος
function isUserAuthenticated() {
    return !!localStorage.getItem('authToken');
}

// Δυναμική ενημέρωση της μπάρας πλοήγησης (navbar)
function updateNavbar() {
    const navLinks = document.getElementById('nav-links');
    if (!navLinks) return;

    navLinks.innerHTML = '';

    const homeLi = document.createElement('li');
    homeLi.innerHTML = `<a href="index.html">Αρχική</a>`;
    navLinks.appendChild(homeLi);

    const cartLi = document.createElement('li');
    cartLi.innerHTML = `<a href="cart.html">Καλάθι</a>`;
    navLinks.appendChild(cartLi);

    if (isUserAuthenticated()) {
        const logoutLi = document.createElement('li');
        const logoutLink = document.createElement('a');
        
        logoutLink.textContent = 'Αποσύνδεση';
        logoutLink.href = '#';
        
        logoutLink.addEventListener('click', (event) => {
            event.preventDefault(); // Αποτρέπει το προεπιλεγμένο navigation
            localStorage.removeItem('authToken');
            logoutUser();
        });
    
        logoutLi.appendChild(logoutLink);
        navLinks.appendChild(logoutLi);
    } else {
        navLinks.innerHTML += `<li><a href="login.html">Σύνδεση</a></li>`;
        navLinks.innerHTML += `<li><a href="register.html">Εγγραφή</a></li>`;
    }
}

// Φόρτωση των προϊόντων στην αρχική σελίδα
async function loadProducts() {
    const products = await fetchProducts();
    if (!products) return;

    const productList = document.getElementById('product-list');
    if (!productList) return;

    productList.innerHTML = '';
    products.forEach(product => {
        const productCard = `
            <div class="product-card">
                <h3>${product.name}</h3>
                <img src="${product.imageUrl}" alt="${product.name}" />
                <p>${product.description}</p>
                <p>Τιμή: ${product.price}€</p>
                <button onclick="addToCart(${product.id}, 1)">Προσθήκη στο Καλάθι</button>
            </div>
        `;
        productList.innerHTML += productCard;
    });
}

// Φόρτωση και εμφάνιση του καλαθιού αγορών
async function loadCart() {
    const userId = 1;
    const cart = await fetchCart(userId);
    const cartItemsContainer = document.getElementById("cart-items");
    const totalAmountSpan = document.getElementById("total-amount");

    if (!cartItemsContainer || !totalAmountSpan) return;

    if (!cart || !cart.items || cart.items.length === 0) {
        cartItemsContainer.innerHTML = "<p>Το καλάθι σας είναι άδειο.</p>";
        totalAmountSpan.textContent = "0.00€";
        return;
    }

    cartItemsContainer.innerHTML = "";
    let total = 0;

    cart.items.forEach(item => {
        total += item.price * item.quantity;

        const cartItemDiv = document.createElement("div");
        cartItemDiv.classList.add("cart-item");
        cartItemDiv.innerHTML = `
            <p>${item.product.name} — ${item.quantity} x ${item.price.toFixed(2)}€</p>
            <button onclick="removeFromCart(${item.id}).then(() => loadCart())">❌</button>
        `;
        cartItemsContainer.appendChild(cartItemDiv);
    });

    totalAmountSpan.textContent = `${total.toFixed(2)}€`;
}

// Προσθήκη στο καλάθι (frontend)
async function addToCart(productId, quantity) {
    const success = await addToCartAPI(1, productId, quantity, 0);
    if (success) {
        alert('Το προϊόν προστέθηκε στο καλάθι!');
    }
}

// Αφαίρεση από το καλάθι (frontend)
async function removeFromCart(itemId) {
    const success = await removeFromCartAPI(itemId);
    if (success) {
        alert('Το προϊόν αφαιρέθηκε από το καλάθι.');
    }
}

// Ολοκλήρωση παραγγελίας (frontend)
document.addEventListener("DOMContentLoaded", function () {
    const checkoutButton = document.getElementById("checkout-btn");

    if (checkoutButton) {
        checkoutButton.addEventListener("click", async () => {
            await checkout();
        });
    }
});

async function checkout() {
    const checkoutData = {
        name: "Demo User",
        address: "123 Demo Street",
        paymentMethod: "Credit Card"
    };

    try {
        const response = await fetch("/api/orders/checkout", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(checkoutData)
        });

        if (!response.ok) {
            throw new Error("Η παραγγελία απέτυχε!");
        }

        alert("Η παραγγελία ολοκληρώθηκε επιτυχώς!");
        document.getElementById("cart-items").innerHTML = "";
    } catch (error) {
        console.error("Σφάλμα κατά την ολοκλήρωση αγοράς:", error);
        alert("Κάτι πήγε στραβά. Δοκιμάστε ξανά.");
    }
}

// Εγγραφή και σύνδεση (frontend)
if (document.getElementById('login-form')) {
    document.getElementById('login-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const credentials = {
            username: document.getElementById('username').value,
            password: document.getElementById('password').value
        };

        const response = await loginUser(credentials);
        if (response) {
            alert(response.message || 'Επιτυχής σύνδεση!');
            localStorage.setItem('authToken', response.token);
            window.location.href = 'index.html';
        } else {
            alert('Αποτυχημένη σύνδεση.');
        }
    });
}

if (document.getElementById('register-form')) {
    document.getElementById('register-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const userDetails = {
            username: document.getElementById('username').value,
            email: document.getElementById('email').value,
            password: document.getElementById('password').value
        };

        const response = await registerUser(userDetails);
        if (response && response.message) {
            alert(response.message);
            window.location.href = 'login.html';
        } else {
            alert('Σφάλμα κατά την εγγραφή.');
        }
    });
}

// DOMContentLoaded
document.addEventListener('DOMContentLoaded', () => {
    updateNavbar();
    if (document.getElementById('product-list')) loadProducts();
    if (document.getElementById('cart-items')) loadCart();
});
