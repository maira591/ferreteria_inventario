var Product = (function () {
    function Product() {

    }

    //Método encargado de abrir la ventana para editar
    Product.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            showLoading();
            $('#modal-partial-content').html(response);
            showModalResult();
            setNumericInput();
            $('.select2').select2();
            hideLoading();
        });
    };

    //Método encargado de recargar la información
    Product.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    Product.OpenCreate = function (url) {
        showLoading();
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            setNumericInput();
            $('.select2').select2();

            hideLoading();
        });
    };

    Product.Save = function () {
        var obj = getObject("formProductcrud");

        if (formValidation("formProductcrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Product/Validations',
                type: 'POST',
                data: obj,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Product/CreateOrUpdate", "Post", obj, "Product.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del permiso.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Product.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        Product.Reload();
    };

    return Product;
})();