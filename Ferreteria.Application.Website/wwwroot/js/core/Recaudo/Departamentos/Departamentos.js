var Departamentos = (function () {
    function Departamentos() {

    }

    //Método encargado de abrir la ventana para editar
    Departamentos.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            $('#Departamento').attr('readonly', 'readonly');
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    Departamentos.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    Departamentos.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            $('.inputPositiveInput2d').numeric({ negative: false, decimalPlaces: 0 })
            showModalResult();
        });
    };

    Departamentos.Save = function () {

        var format = getObject("formDepartamentoslcrud");
        
        if (formValidation("formDepartamentoslcrud")) {

            if (!isValidInteger("Departamento", 2, "Departamento")) {
                return;
            }

            if (!isValidInteger("Indicativo", 2, "Indicativo")) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            showLoading("Guardando...");
            $.ajax({
                ContentType: "application/json",
                url: 'RecaudoDepartamentos/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("RecaudoDepartamentos/CreateOrUpdate", "Post", format, "Departamentos.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });

            
        }
    };

    Departamentos.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Departamentos.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return Departamentos;
})();