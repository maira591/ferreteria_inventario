var TasaMora = (function () {
    function TasaMora() {

    }

    //Método encargado de abrir la ventana para editar
    TasaMora.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    TasaMora.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    TasaMora.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    TasaMora.Save = function () {

        var format = getObject("formtasamoralcrud");
        
        if (formValidation("formtasamoralcrud")) {

            if (!isValidInteger("ValorTasaMora", 2, "Valor Tasa Mora")) {
                return;
            }

            if ($("#ValorTasaMora").val() <= 0) {
                $("#ValorTasaMora").addClass("errorValidate");
                $("#ValorTasaMora").attr("data-errormessage", "Debe ser mayor a 0.");
                showAlert("El campo Valor Tasa Mora debe ser mayor a 0", 'danger');
                showTooltip();
                return true;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            showLoading("Guardando...");
            ExecuteAjax("RecaudoTasaMora/CreateOrUpdate", "Post", format, "TasaMora.ResultSaveFormat");
        }
    };

    TasaMora.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            TasaMora.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    return TasaMora;
})();