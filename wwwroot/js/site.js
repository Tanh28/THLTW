$(document).ready(function () {
    $(".add-to-cart").click(function (e) {
        e.preventDefault(); // Ngăn trang bị reload

        var productId = $(this).data("product-id");
        var quantity = $(this).data("quantity");

        $.ajax({
            url: "/ShoppingCart/AddToCart",
            type: "POST",
            data: { productId: productId, quantity: quantity },
            success: function (response) {
                if (response.success) {
                    $("#cartItemCount").text(response.cartItemCount);
                    alert("Sản phẩm đã được thêm vào giỏ hàng!");
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Có lỗi xảy ra. Vui lòng thử lại!");
            }
        });
    });
});
