var Supplier = (function () {
    function Supplier() {

    }

    //Método encargado de abrir la ventana para editar
    Supplier.OpenEdit = function (urlEdit) {
        showLoading();
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            setAlphanumericInput();
            setNumericInput();

            hideLoading();
        });
    };

    //Método encargado de recargar la información
    Supplier.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    Supplier.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Periodicity.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };


    //Método encargado de realizar el borrado de un tipo de carga.
    Supplier.Disabled = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Supplier.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Supplier.OpenCreate = function (url) {
        showLoading();
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            setAlphanumericInput();
            setNumericInput();
            hideLoading();
        });
    };

    Supplier.Save = function () {
        var obj = getObject("formSuppliercrud");

        if (formValidation("formSuppliercrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Supplier/Validations',
                type: 'POST',
                data: obj,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Supplier/CreateOrUpdate", "Post", obj, "Supplier.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del proveedor.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Supplier.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        Supplier.Reload();
    };

    return Supplier;
})();