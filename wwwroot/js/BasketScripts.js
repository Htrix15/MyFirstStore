
$(document).delegate(".inBasket", "click", function () {
    var product = $(this).parent('form').serializeArray();
    $.ajax({
        type: "GET",
        data: product,
        url: '/Home/ProductInBasketAjax',
        success: function (result) {
            if (result.jsCountProductToBasket <= 1) {
                $(`<div class=\"basketBox borderProductBox${result.jsColor} \" idBasket=\"${result.jsId}\">` +
                    `<img src=\"${result.jsPicturePath}\" width="100px" />` +
                    `<span class="basketMiniText"> ${result.jsName} </span>` +
                    `<div><span class="price basketMiniText">${result.jsPrice}</span><span class=\"basketMiniText\"> р.</span></div>` +
                    `<div class="basketMiniButtons">` +
                    `<button value=\"${result.jsId}\" class=\"removeProductForBasket minButton bc${result.jsColor} \"> - </button>` +
                    `<div><span class=\"countProduct basketMiniText\"> 1 </span><span class=\"basketMiniText\"> шт.</span></div>` +
                    `<button value=\"${result.jsId}\" class=\"addProductToBasket minButton bc${result.jsColor} \"> + </button>` +
                    ` </div>` +
                    `<button value=\"${result.jsId}\" class=\"removeAllProductForBasket big1Button bc${result.jsColor} \"> Удалить </button></div>`).insertBefore($(".newBasket"));
            }
            else {
                var basket = $(`[idBasket = ${result.jsId}]`); 
                basket.find(".countProduct").html(result.jsCountProductToBasket);
            }
            var oldCost = $('.basketCost').html();
            var newCost = parseFloat(oldCost) + result.jsPrice;
            $('.basketCost').html(newCost);
        }
    });
});
$(document).delegate(".addProductToBasket", "click", function () {
    var product = $(this).val();
    var but = $(this);
    $.ajax({
        type: "GET",
        data: { id: product },
        url: '/Home/AddProductToBasketAjax',
        success: function (result) {
            var basket = $(`[idBasket = ${product}]`);
            var price = basket.find(".price").html();
            var oldCost = $('.basketCost').html();
            var newCost = parseFloat(oldCost) + parseFloat(price);
            $('.basketCost').html(newCost);
            var newCount = `<span class=\"countProduct basketMiniText\"> ${result} </span><span class=\"basketMiniText\"> шт.</span>`;
            but.prev().html(newCount);
        }
    });
});
$(document).delegate(".removeProductForBasket", "click", function () {
    var product = $(this).val();
    var but = $(this);
    $.ajax({
        type: "GET",
        data: { id: product, all: false },
        url: '/Home/RemoveProductForBasketAjax',
        success: function (result) {
            if (result != 0) {
                var basket = $(`[idBasket = ${product}]`);
                var price = basket.find(".price").html();
                var oldCost = $('.basketCost').html();
                var newCost = parseFloat(oldCost) - parseFloat(price);
                $('.basketCost').html(newCost);
                var newCount = `<span class=\"countProduct basketMiniText\"> ${result} </span><span class=\"basketMiniText\"> шт.</span>`;
                but.next().html(newCount);
            }
            else {
                var basket = $(`[idBasket = ${product}]`);
                var price = basket.find(".price").html();
                var oldCost = $('.basketCost').html();
                var newCost = parseFloat(oldCost) - parseFloat(price);
                $('.basketCost').html(newCost);
                but.parent().parent('.basketBox').remove();
            }

        }
    });
});

$(document).delegate(".removeAllProductForBasket", "click", function () {
    var product = $(this).val();
    var but = $(this);
    $.ajax({
        type: "GET",
        data: { id: product, all: true },
        url: '/Home/RemoveProductForBasketAjax',
        success: function (result) {
            if (result != 0) {

                but.prev().html(result);
            }
            else {
                var basket = $(`[idBasket = ${product}]`);
                var price = basket.find(".price").html();
                var count = basket.find(".countProduct").html();
                price = parseFloat(price) * parseFloat(count);
                var oldCost = $('.basketCost').html();
                var newCost = parseFloat(oldCost) - parseFloat(price);
                $('.basketCost').html(newCost);

                but.parent('.basketBox').remove();
            }

        }
    });
});
$(document).delegate(".DelBasket", "click", function () {
        $.ajax({
            type: "GET",
            url: '/Home/DeleteBasketAjax',
            success: function (result) {
                $('.basketBox').remove();
                $('.basketCost').html(0);
            }
        });
    });


