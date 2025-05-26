var Format = (function () {
    function Format() {

    }

    //Método utilizado para abrir o cerrar los detalles de los formatos.
    Format.CollapseOrExpand = function (idFormat) {

        var isExpanded = $(`#iconCollapse_${idFormat}`).data('isexpanded');

        if (isExpanded === 'true') {
            $(`#iconCollapse_${idFormat}`).removeClass('fa fa-chevron-down');
            $(`#iconCollapse_${idFormat}`).addClass('fa fa-chevron-right');
            $(`#divFormat_${idFormat}`).fadeOut(300);

        } else {
            $(`#iconCollapse_${idFormat}`).removeClass('fa fa-chevron-right');
            $(`#iconCollapse_${idFormat}`).addClass('fa fa-chevron-down');
            $(`#divFormat_${idFormat}`).fadeIn(500);
        }

        $(`#iconCollapse_${idFormat}`).data('isexpanded', (isExpanded === 'true') ? 'false' : 'true');
    };

    //Método encargado de abrir la ventana para editar
    Format.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    Format.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de recargar la tabla de detalle de un formato.
    Format.ReloadFormatColumns = function (urlLoad, catalogId) {
        Petitions.Get(urlLoad, function (response) {
            $(`#divFormatColumns_${catalogId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado de un formato.
    Format.DisabledFormat = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este formato?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Format.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    //Método encargado de borrar un valor de detalle de un formato.
    Format.DeleteFormatColumn = function (url, catalogId) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este valor?', function () {
            HideModalQuestion();

            Petitions.Get(url, function (response) {

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    reloadFormatColumn();
                } else {
                    showAlert("Ocurrió un error eliminando la información, verifique que no tenga datos asociados.");
                }
            });
        });
    };

    Format.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Format.SaveFormat = function () {

        var format = getObject("formformatcrud");
        
        if (formValidation("formformatcrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Format/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Format/CreateOrUpdate", "Post", format, "Format.ResultSaveFormat");
                    }
                },
                error: function (jqXHR, exception) {
                    debugger;

                    showAlert("No fue posible realizar la validación del formato.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Format.ResultSaveFormat = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Format.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    Format.GetSelectedPeriodicity = function () {
        var selectedOptions = $('#PeriodicityId option:selected');
        var ids = "";
        selectedOptions.each(function (index, element) {
            if (ids == "") {
                ids = element.value;
            }
            else {
                ids = ids + "," + element.value;
            }
        });
        $("#Periodicities").val(ids);
        return ids;
    };

    Format.ValidatePeriodicitySelection = function () {
        return Format.GetSelectedPeriodicity().length > 0;        
    };

    Format.SetPeriodicitySelection = function () {
        var splitPeriodicity = $("#Periodicities").val().split(',');

        for (var i = 0; i < splitPeriodicity.length; i++) {
            $('#PeriodicityId option').each(function () {
                if ($(this).val() === splitPeriodicity[i]) {
                    $(this).prop('selected', true);
                    return;
                }
            });
        };

        $('#PeriodicityId').multiselect('refresh');
    };

    //Methodo para exportar las plantillas de los formatos a Excel
    Format.ExportTemplateFormat = function (url, nameFormat) {
        $.ajax({
            url: url,
            method: 'get',
            beforeSend: function () {
                showLoading('Exportando...')
            },
            xhrFields: { responseType: 'blob' },
            success: function (data) {
                var blob = new Blob([data], { type: 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var downloadUrl = URL.createObjectURL(blob);
                var a = document.createElement("a");
                a.href = downloadUrl;
                a.download = `Plantilla - ${nameFormat}.xlsx`;
                a.target = '_blank';
                a.click();
            },
            error: function (jqXHR, exception) {
                showAlert("Unexpected error", "warning");
            },
            complete: function () {
                $("#modal-loading").modal("hide");
                $("#modal-loading-back").hide();
            }
        });
    };


    //Methodo para exportar las plantillas de los formatos a Excel
    Format.ExportData = function (url) {
        var fileName = 'Formatos.xlsx';
        $.ajax({
            url: url,
            method: 'post',
            beforeSend: function () {
                showLoading('Exportando...')
            },
            xhrFields: { responseType: 'blob' },
            data: { fileName },
            success: function (data) {
                var blob = new Blob([data], { type: 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var downloadUrl = URL.createObjectURL(blob);
                var a = document.createElement("a");
                a.href = downloadUrl;
                a.download = fileName;
                a.target = '_blank';
                a.click();
            },
            error: function (jqXHR, exception) {
                showAlert("Unexpected error", "warning");
            },
            complete: function () {
                $("#modal-loading").modal("hide");
                $("#modal-loading-back").hide();
            }
        });
    };

    return Format;
})();


//Columnas.
function saveFormatColumn() {
    var valuesFormat = getObject("formformatcolumns");

    if (formValidation("formformatcolumns")) {
        ExecuteAjax("FormatColumn/CreateOrUpdate", "Post", valuesFormat, "resultSaveFormatColumn");
    }
}

function resultSaveFormatColumn(response) {
    if (response.Success) {
        showAlert(response.Message, 'success');
    } else {
        showAlert(response.Message, 'danger');
    }
    hideModalPartial();
    reloadFormatColumn();
}

function reloadFormatColumn() {
    var urlReload = $("#urlFormatColumn").val();
    var catalogId = $("#FormatId").val();
    Format.ReloadFormatColumns(urlReload, catalogId);
}