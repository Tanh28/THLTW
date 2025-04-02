function fetchProducts() {
  const apiUrl = `http://localhost:5218/api/ProductAPI`;
  fetch(apiUrl)
    .then(handleResponse)
    .then((data) => displayProducts(data))
    .catch((error) => console.error("Fetch error: ", error.message));
}

document.addEventListener("DOMContentLoaded", function () {
  fetchProducts();
  document.getElementById("btnAdd").addEventListener("click", addProduct);
});
function fetchProducts() {
  const apiUrl = "http://localhost:5030/api/ProductApi";
  fetch(apiUrl)
    .then(handleResponse)
    .then((data) => displayProducts(data))
    .catch((error) => console.error("Fetch error:", error.message));
}
// Handle fetch response, check for error, and parse JSON
function handleResponse(response) {
  if (!response.ok) throw new Error("Network response was not ok");
  return response.json();
}
// Display products in the HTML table
function displayProducts(products) {
  const bookList = document.getElementById("productList");
  bookList.innerHTML = ""; // Clear existing products
  products.forEach((product) => {
    bookList.innerHTML += createProductRow(product);
  });
}
// Create HTML table row for a product
function createProductRow(product) {
  return `
    <tr>
    <td>${product.id}</td>
    <td>${product.name}</td>
    <td>${product.price}</td>
    <td>${product.description}</td>
    <td>
    <button class="btn btn-danger delete-btn" dataid="${product.id}">Delete</button>
    <button class="btn btn-warning edit-btn" dataid="${product.id}">Edit</button>
    <button class="btn btn-primary view-btn" dataid="${product.id}">View</button>
    </td>
    </tr>
    `;
}
// Add a new product
function addProduct() {
  const productData = {
    name: document.getElementById("bookName").value,
    price: document.getElementById("price").value,
    description: document.getElementById("description").value,
  };
  fetch("http://localhost:5030/api/ProductApi", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(productData),
  })
    .then(handleResponse)
    .then((data) => {
      console.log("Product added:", data);
      fetchProducts(); // Refresh the product list
    })
    .catch((error) => console.error("Error:", error));
}

function getProduct() {
  const productId = 1;
  fetch(`https://your-api-url/api/products/${productId}`)
    .then((response) => response.json())
    .then((product) => {
      // Xử lý thông tin chi tiết sản phẩm
      console.log(product);
    })
    .catch((error) => console.error("Error:", error));
}

function updateProduct() {
  const productIdToUpdate = 1;
  const updatedProduct = {
    id: productIdToUpdate,
    name: "Updated Product",
    price: 150,
    description: "An updated product",
    // Thêm các thông tin khác
  };
  fetch(`https://your-api-url/api/products/${productIdToUpdate}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(updatedProduct),
  })
    .then((response) => {
      if (response.status === 204) {
        console.log("Product updated successfully.");
      } else {
        console.error("Failed to update product.");
      }
    })
    .catch((error) => console.error("Error:", error));
}

function deleteProduct() {
  const productIdToDelete = 1;
  fetch(`https://your-api-url/api/products/${productIdToDelete}`, {
    method: "DELETE",
  })
    .then((response) => {
      if (response.status === 204) {
        console.log("Product deleted successfully.");
      } else {
        console.error("Failed to delete product.");
      }
    })
    .catch((error) => console.error("Error:", error));
}
