var Entidades = (function () {
    function Entidades() {

    }

    //Método encargado de abrir la ventana para editar
    Entidades.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            Entidades.InitControls();
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    Entidades.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    Entidades.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            Entidades.InitControls();
            showModalResult();
        });
    };

    Entidades.Save = function () {

        var format = getObject("formEntidadescrud");
        
        if (formValidation("formEntidadescrud")) {
            if (!isValidInteger("DigitoChequeo", 1, "Digito Chequeo")) {
                return;
            }
            
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            showLoading("Guardando...");
            $.ajax({
                ContentType: "application/json",
                url: 'RecaudoEntidades/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("RecaudoEntidades/CreateOrUpdate", "Post", format, "Entidades.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Entidades.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Entidades.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    Entidades.LoadStates = function (idTipoEstado) {
        $.ajax({
            method: 'POST',
            data: { idTipoEstado },
            url: 'RecaudoEntidades/ObtenerEstados',
            success: function (data) {
                html = '<option>Seleccione</option>';
                $.each(data, function (index, item) {
                    html += `<option value="${item.Value}">${item.Text}</option>`;
                });
                $('#Estado').html(html);
            }
            
        });
    };

    Entidades.InitControls = function () {
        $("#FechaConstitucion").val($("#hdf_FechaConstitucion").val());
        $("#FechaAutorizacion").val($("#hdf_FechaAutorizacion").val());
        $("#FechaInscripcion").val($("#hdf_FechaInscripcion").val());
        $("#FechaFinalInscripcion").val($("#hdf_FechaFinalInscripcion").val());
        $('#FechaConstitucion, #FechaAutorizacion, #FechaInscripcion, #FechaFinalInscripcion').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
        $('#FechaConstitucion, #FechaAutorizacion, #FechaInscripcion, #FechaFinalInscripcion').mask('00/00/0000', { placeholder: "__/__/____" });
        
        $('.select2').select2();
        $('#TipoEntidades').change(function () {
            MatInfoAdicional.GetListEntityCodeByTypeEntity($(this).val());
        });
    }

    return Entidades;
})();