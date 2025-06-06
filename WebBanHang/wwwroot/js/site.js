// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    showQuantityCart();
});
let showQuantityCart = () => {
    $.ajax({
        url: "/customer/cart/GetQuantityOfCart",
        success: function (data) {
                 $(".showcart").text(data.qty);
        }
    });
}

$(document).on("click", ".addtocart", function (evt) {
    evt.preventDefault();

    let id = $(this).data("productid");
    $.ajax({
        url: "/customer/cart/AddToCart",
        data: { productId: id },
        success: function (data) {
            Swal.fire({
                icon: "success",
                title: "Đã thêm vào giỏ hàng!",
                text: "Bạn có thể tiếp tục mua sắm hoặc đi đến giỏ hàng."
            });
            showQuantiyCart();
        },
        error: function () {
            Swal.fire("Lỗi", "Không thể thêm sản phẩm vào giỏ", "error");
        }
    });
});