var GraphicGroup = (function () {
    function GraphicGroup() {

    }

    //Método utilizado para abrir o cerrar detalles.
    GraphicGroup.CollapseOrExpand = function (id) {

        var isExpanded = $(`#iconCollapse_${id}`).data('isexpanded');

        if (isExpanded === 'true') {
            $(`#iconCollapse_${id}`).removeClass('fa fa-chevron-down');
            $(`#iconCollapse_${id}`).addClass('fa fa-chevron-right');
            $(`#divParent_${id}`).fadeOut(300);

        } else {
            $(`#iconCollapse_${id}`).removeClass('fa fa-chevron-right');
            $(`#iconCollapse_${id}`).addClass('fa fa-chevron-down');
            $(`#divParent_${id}`).fadeIn(500);
        }

        $(`#iconCollapse_${id}`).data('isexpanded', (isExpanded === 'true') ? 'false' : 'true');
    };

    //Método encargado de abrir la ventana para editar
    GraphicGroup.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información.
    GraphicGroup.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de recargar los detalles
    GraphicGroup.ReloadFormatColumns = function (urlLoad, graphicGroupId) {
        Petitions.Get(urlLoad, function (response) {
            $(`#divChildren_${graphicGroupId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado de un registro.
    GraphicGroup.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    GraphicGroup.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    //Método encargado de borrar un valor de detalle de un registro.
    GraphicGroup.DeleteHomologationValue = function (url, catalogId) {
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

    GraphicGroup.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    GraphicGroup.Save = function () {
        var format = getObject("formgraphicgroupcrud");
        if (formValidation("formgraphicgroupcrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'GraphicGroup/Validations',
                type: 'POST',
                data: format,
                success: function (data) {

                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("GraphicGroup/CreateOrUpdate", "Post", format, "GraphicGroup.ResultSave");
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

    GraphicGroup.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            GraphicGroup.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    GraphicGroup.ExportData = function (url) {
        var fileName = 'GruposGraficas.xlsx';
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

    return GraphicGroup;
})();