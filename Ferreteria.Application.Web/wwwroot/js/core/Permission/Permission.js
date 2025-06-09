var Permission = (function () {
    function Permission() {

    }

    //Método encargado de abrir la ventana para editar
    Permission.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            showLoading();
            $('#modal-partial-content').html(response);
            showModalResult();
            setAlphanumericInput();
            hideLoading();
        });
    };

    //Método encargado de recargar la información
    Permission.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    Permission.Delete = function (url) {
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
    Permission.Disabled = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Permission.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Permission.OpenCreate = function (url) {
        showLoading();
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            setAlphanumericInput();
            hideLoading();
        });
    };

    Permission.Save = function () {
        var periodicity = getObject("formPermissioncrud");

        if (formValidation("formPermissioncrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Permission/Validations',
                type: 'POST',
                data: periodicity,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Permission/CreateOrUpdate", "Post", periodicity, "Permission.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del permiso.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Permission.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        Permission.Reload();
    };

    return Permission;
})();