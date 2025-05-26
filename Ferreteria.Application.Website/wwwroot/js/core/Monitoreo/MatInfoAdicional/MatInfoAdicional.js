var MatInfoAdicional = (function () {
    function MatInfoAdicional() {

    }

    //Método encargado de abrir la ventana para editar
    MatInfoAdicional.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    MatInfoAdicional.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    MatInfoAdicional.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de realizar el borrado logico de un formato.
    MatInfoAdicional.DisabledFormat = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este formato?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    MatInfoAdicional.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    
    MatInfoAdicional.Save = function () {
        if ($("#Circular").val() != '' && $("#Circular").val() <= 0) {
            showAlert("El campo Circular, debe ser mayor a cero.", 'danger');
            return;
        }
        var format = getObject("formconstantescrud");
        if (formValidation("formconstantescrud")) {

            if ($("#Valor").val() != '' && $("#Valor").val() <= 0) {
                $("#Valor").addClass("errorValidate");
                $("#Valor").attr("data-errormessage", "Debe ser mayor a cero.");
                showAlert("El campo Valor Tasa, debe ser mayor a cero.", 'danger');
                showTooltip();
                return;
            }

            if (!isValidInteger("Valor", 9, "Valor")) {
                return;
            }

            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'MonitoreoMatInfoAdicional/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("MonitoreoMatInfoAdicional/CreateOrUpdate", "Post", format, "MatInfoAdicional.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    debugger;

                    showAlert("No fue posible realizar la validación del registro.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    MatInfoAdicional.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            MatInfoAdicional.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    MatInfoAdicional.InitControls = function () {
        $("#FechaCorte").val($("#hdf_FechaCorte").val());
        $('#FechaCorte').mask('00/00/0000', { placeholder: "__/__/____" });
        $('#FechaCorte').datepicker({ language: 'esp', clearBtn: true, format: "dd/mm/yyyy" });
        $('.select2').select2();
        $('#TipoEntidad').change(function () {
            MatInfoAdicional.GetListEntityCodeByTypeEntity($(this).val());
        });
    }

    MatInfoAdicional.GetListEntityCodeByTypeEntity = function (typeEntity) {
        Petitions.Get('MonitoreoMatInfoAdicional/GetListEntityCodeByTypeEntity?typeEntity=' + typeEntity, function (response) {

            //Se remueven todos los items, se deja opcion seleccione. 
            $('#CodigoEntidad').find('option').remove().end().val('');
            //Se agregan los items que vienen en la variable data.
            var listitems = "";
            listitems = '<option value> Seleccione </option>';
            $.each(response, function (key, value) {
                console.log(value)
                listitems += '<option value=' + value.CodigoEntidad + '>' + value.CodigoEntidad + ' - '+ value.Sigla + '</option>';
            });
            $('#CodigoEntidad').append(listitems);
            $('.select2').select2();
        });
    }



    return MatInfoAdicional;
})();