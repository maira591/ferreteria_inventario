var Category = (function () {
    function Category() {

    }

    //Método encargado de abrir la ventana para editar
    Category.OpenEdit = function (urlEdit) {
        showLoading();
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            setAlphanumericInput();
            

            hideLoading();
        });
    };

    //Método encargado de recargar la información
    Category.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado
    Category.Disabled = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Category.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Category.OpenCreate = function (url) {
        showLoading();
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            setAlphanumericInput();
            
            hideLoading();
        });
    };

    Category.Save = function () {
        var obj = getObject("formCategorycrud");

        if (formValidation("formCategorycrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Category/Validations',
                type: 'POST',
                data: obj,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Category/CreateOrUpdate", "Post", obj, "Category.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del proveedor.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Category.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        Category.Reload();
    };

    return Category;
})();