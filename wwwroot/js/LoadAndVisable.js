$(document).delegate(".visableBasket", "click", function () {
        var basket = $('.basket')
        if (basket.hasClass('visibilityOFF')) {
            basket.removeClass('visibilityOFF');
        }
        else {
            basket.addClass('visibilityOFF');
        }
    });
$(document).delegate(".visableOrder", "click", function () {
        var orderPositions =
            $(this).parent().parent().next(".orderPositions");
        if (orderPositions.hasClass('visibilityOFF')) {
            orderPositions.removeClass('visibilityOFF');
        }
        else {
            orderPositions.addClass('visibilityOFF');
        }
    });

$(document).delegate(".loadValue", "click", function () {
    var currentPosition = $(this).parent().children('[name=currentPosition]');
    var maxPage = $(this).parent().children('[name=maxPage]');
    var form = $(this).parent();
    var product = $(this).parent('form').serializeArray();
    var paginButton = $(`#pag${currentPosition.val()}`).children('.paginationButton');
    $.ajax({
        type: "GET",
        data: product,
        url: '/home/ProductsListAjax',
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                var backColor = "bc" + result[i].jsHexColor;
                var border = "borderProductBox" + result[i].jsHexColor;
                var typeName = result[i].jsSale ? "Cкидка!" : result[i].jsParentTypeName;

                $(`<div class=\"productBox ${border}\">
                    <span class=\"productType ${backColor}\">${typeName}</span>
                    <img class=\"image\" src=\"${result[i].jsMainPicturePath}\">
                    <div class="textsBox">
                        <span class="textBig">${result[i].jsName}</span>
                        <span class="textBig">${result[i].jsPrice} руб.</span>
                        <span class="textSmile">${result[i].jsMainAttribute}</span>
                    </div>
                    <div class="buttonBox">
                        <a class="buttonOther bcDefault" href="/Home/AboutToProduct/${result[i].jsId}?currentPosition=${result[i].jsCurrentPosition}">Инфо</a>
                        <form action="/home/BuyOneClick">
                            <input type="hidden" name="id" value=${result[i].jsId} />
                            <input type="submit" class="buttonBuy ${backColor}" value="Купить!" />
                        </form>
                        <form>
                            <input type="hidden" name="id" value="${result[i].jsId}" />
                            <input type="hidden" name="price" value="${result[i].jsPrice}" />
                            <input type="hidden" name="name" value="${result[i].jsName}" />
                            <input type="hidden" name="color" value="${result[i].jsHexColor}" />
                            <input type="hidden" name="picturePath" value="${result[i].jsMainPicturePath}" />
                            <input type="button" class="inBasket buttonOther bcDefault" value="В корзину" />
                        </form>
                    </div>
                </div>
           `).insertBefore($(".newPositions"));
            }
            currentPosition.val(parseInt(currentPosition.val()) + 1);
            paginButton.prop('disabled', true);
            paginButton.removeClass("paginationButton");
            paginButton.addClass("paginationButtonDisable");
            if (parseInt(maxPage.val()) != parseInt(currentPosition.val())) {
                form.remove();
            }
        }
    });
});
$(document).delegate(".loadValueSearch", "click", function () {
    var currentPosition = $(this).parent().children('[name=currentPosition]');
    var maxPage = $(this).parent().children('[name=maxPage]');
    var form = $(this).parent();
    var product = $(this).parent('form').serializeArray();
    var paginButton = $(`#pag${currentPosition.val()}`).children('.paginationButton');
    $.ajax({
        type: "GET",
        data: product,
        url: '/home/SearchAjax',
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                var backColor = "bc" + result[i].jsHexColor;
                var border = "borderProductBox" + result[i].jsHexColor;
                var typeName = result[i].jsSale ? "Cкидка!" : result[i].jsParentTypeName;

                $(`<div class=\"productBox ${border}\">
                    <span class=\"productType ${backColor}\">${typeName}</span>
                    <img class=\"image\" src=\"${result[i].jsMainPicturePath}\">
                    <div class="textsBox">
                        <span class="textBig">${result[i].jsName}</span>
                        <span class="textBig">${result[i].jsPrice} руб.</span>
                        <span class="textSmile">${result[i].jsMainAttribute}</span>
                    </div>
                    <div class="buttonBox">
                        <a class="buttonOther bcDefault" href="/Home/AboutToProduct/${result[i].jsId}?currentPosition=${result[i].jsCurrentPosition}">Инфо</a>
                        <form action="/home/BuyOneClick">
                            <input type="hidden" name="id" value=${result[i].jsId} />
                            <input type="submit" class="buttonBuy ${backColor}" value="Купить!" />
                        </form>
                        <form>
                            <input type="hidden" name="id" value="${result[i].jsId}" />
                            <input type="hidden" name="price" value="${result[i].jsPrice}" />
                            <input type="hidden" name="name" value="${result[i].jsName}" />
                            <input type="hidden" name="color" value="${result[i].jsHexColor}" />
                            <input type="hidden" name="picturePath" value="${result[i].jsMainPicturePath}" />
                            <input type="button" class="inBasket buttonOther bcDefault" value="В корзину" />
                        </form>
                    </div>
                </div>
           `).insertBefore($(".newPositions"));
            }
            currentPosition.val(parseInt(currentPosition.val()) + 1);
            paginButton.prop('disabled', true);
            paginButton.removeClass("paginationButton");
            paginButton.addClass("paginationButtonDisable");
            if (parseInt(maxPage.val()) != parseInt(currentPosition.val())) {
                form.remove();
            }
        }
    });
});
$(document).delegate(".loadOrders", "click", function () {
    var currentPosition = $(this).parent().children('[name=currentPosition]');
    var maxPage = $(this).parent().children('[name=maxPage]');
    var form = $(this).parent();
    var product = $(this).parent('form').serializeArray();
    var paginButton = $(`#pag${currentPosition.val()}`).children('.paginationButton');
    $.ajax({
        type: "GET",
        data: product,
        url: '/Account/CardAjax',
        success: function (result) {
            var orderAndOrderPositions = ``;
            for (var i = 0; i < result.orders.length; i++) {

                var order =
                    `<div class="orderBox">
                    <div>${result.orders[i].jsId}</div>
                    <div>${result.orders[i].jsStatus}</div>
                    <div>${result.orders[i].jsTotalFixPrice} р.</div>
                    <div>${result.orders[i].jsDataReservation}</div>
                    <div><button class="accountEditButton visableOrder">V</button></div>
                 </div>
                 <div class="orderPositions visibilityOFF" value="1">
                        <div class="orderPositionBox ">
                            <div></div><div>Название</div><div>Тип</div><div>Цена</div><div>Кол-во</div><div></div>
                        </div>`;
                var orderPositions = ``;
                for (var j = 0; j < result.orderPositions.length; j++) {
                    if (result.orderPositions[j].jsOrderId == result.orders[i].jsId) {
                        orderPositions += `<div class="orderPositionBox">
                                 <div> <img src=${result.orderPositions[j].jsMainPicturePath} width = 50px /></div >
                                <div>${result.orderPositions[j].jsProductName}</div>
                                <div>${result.orderPositions[j].jsProductTypeName}</div>
                                <div>${result.orderPositions[j].jsFixPrice}</div>
                                <div>${result.orderPositions[j].jsCount}</div>
                                <div><a class="accountEditButton" href="/Home/AboutToProduct?id=${result.orderPositions[j].jsProductId}&currentPosition=${result.orders[i].jsCurrentPosition}">О товаре</a></div>
                             </div >`
                    }
                }
                orderAndOrderPositions = order + orderPositions + `</div>`;
            }
            currentPosition.val(parseInt(currentPosition.val()) + 1);
            paginButton.prop('disabled', true);
            paginButton.removeClass("paginationButton");
            paginButton.addClass("paginationButtonDisable");
            if (parseInt(maxPage.val()) != parseInt(currentPosition.val())) {
                form.remove();
            }
            $(orderAndOrderPositions
            ).insertBefore($(".newPositions"));
        }
    });
});
$(document).delegate(".loadUsers", "click", function () {
    var currentPosition = $(this).parent().children('[name=currentPosition]');
    var maxPage = $(this).parent().children('[name=maxPage]');
    var form = $(this).parent();
    var product = $(this).parent('form').serializeArray();
    var paginButton = $(`#pag${currentPosition.val()}`).children('.paginationButton');
    $.ajax({
        type: "GET",
        data: product,
        url: '/admin/UsersViewAjax',
        success: function (result) {
            for (var i = 0; i < result.length; i++) {
                $(`<table border="1">
            <tr>
            <td>${result[i].jsId}</td>
            <td>${result[i].jsName}</td>
            <td>${result[i].jsEmail}</td>
            <td>${result[i].jsAllRoles}</td>
            <td><a href="/Admin/Edit?id=${result[i].jsId}&currentPosition=${result[i].jsCurrentPosition}">edit</a></td>
            <td><a href="/Admin/SetRoles?id=${result[i].jsId}&currentPosition=${result[i].jsCurrentPosition}">setRoles</a></td>
            <td><a href="/Admin/DroppingPassword?id=${result[i].jsId}&currentPosition=${result[i].jsCurrentPosition}">droppingPassword</a></td>
            <td><a href="/Admin/Delete?id=${result[i].jsId}&currentPosition=${result[i].jsCurrentPosition}">delete</a></td>
            </tr>
                </table>`).insertBefore($(".newPositions"));
            }
            currentPosition.val(parseInt(currentPosition.val()) + 1);
            paginButton.prop('disabled', true);
            paginButton.removeClass("paginationButton");
            paginButton.addClass("paginationButtonDisable");
            if (parseInt(maxPage.val()) != parseInt(currentPosition.val())) {
                form.remove();
            }
        }
    });
}); 
