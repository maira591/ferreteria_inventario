var Homologation = (function () {
    function Homologation() {

    }

    //Método utilizado para abrir o cerrar detalles.
    Homologation.CollapseOrExpand = function (id) {

        var isExpanded = $(`#iconCollapse_${id}`).data('isexpanded');

        if (isExpanded === 'true') {
            $(`#iconCollapse_${id}`).removeClass('fa fa-chevron-down');
            $(`#iconCollapse_${id}`).addClass('fa fa-chevron-right');
            $(`#divHomologation_${id}`).fadeOut(300);

        } else {
            $(`#iconCollapse_${id}`).removeClass('fa fa-chevron-right');
            $(`#iconCollapse_${id}`).addClass('fa fa-chevron-down');
            $(`#divHomologation_${id}`).fadeIn(500);
        }

        $(`#iconCollapse_${id}`).data('isexpanded', (isExpanded === 'true') ? 'false' : 'true');
    };

    //Método encargado de abrir la ventana para editar
    Homologation.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información.
    Homologation.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de recargar los detalles
    Homologation.ReloadFormatColumns = function (urlLoad, homologationId) {
        Petitions.Get(urlLoad, function (response) {
            $(`#divHomologationValue_${homologationId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado de un registro.
    Homologation.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Homologation.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    //Método encargado de borrar un valor de detalle de un registro.
    Homologation.DeleteHomologationValue = function (url, catalogId) {
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

    Homologation.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Homologation.Save = function () {
        var format = getObject("formhomologationcrud");
        if (formValidation("formhomologationcrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Homologation/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Homologation/CreateOrUpdate", "Post", format, "Homologation.ResultSave");
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

    Homologation.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Homologation.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    Homologation.ExportData = function (url) {
        var fileName = 'Homologaciones.xlsx';
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

    return Homologation;
})();