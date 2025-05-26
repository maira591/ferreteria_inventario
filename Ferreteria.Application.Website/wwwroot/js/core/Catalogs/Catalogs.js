//Clase encargada del crud de catálogos.
var Catalogs = (function () {
    function Catalogs() {

    }

    //Método utilizado para abrir o cerrar el detalles de los catálogos.
    Catalogs.CollapseOrExpand = function (idCatalog) {

        var isExpanded = $(`#iconCollapse_${idCatalog}`).data('isexpanded');

        if (isExpanded === 'true') {
            $(`#iconCollapse_${idCatalog}`).removeClass('fa fa-chevron-down');
            $(`#iconCollapse_${idCatalog}`).addClass('fa fa-chevron-right');
            $(`#divCatalog_${idCatalog}`).fadeOut(300);

        } else {
            $(`#iconCollapse_${idCatalog}`).removeClass('fa fa-chevron-right');
            $(`#iconCollapse_${idCatalog}`).addClass('fa fa-chevron-down');
            $(`#divCatalog_${idCatalog}`).fadeIn(500);
        }

        $(`#iconCollapse_${idCatalog}`).data('isexpanded', (isExpanded === 'true') ? 'false' : 'true');
    };

    //Método encargado de abrir la ventana para editar
    Catalogs.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los catálogos
    Catalogs.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de recargar la tabla de detalle de un catálogo.
    Catalogs.ReloadValueCatalogs = function (urlLoad, catalogId) {
        Petitions.Get(urlLoad, function (response) {
            $(`#divValueCatalogs_${catalogId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado de un cátalogo.
    Catalogs.DeleteCatalog = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este catálogo?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Catalogs.Reload();
                } else {
                    showAlert("Ocurrió un error eliminando el catálogo, verifique que no tenga datos asociados.");
                }
            });
        });
    };

    //Método encargado de borrar un valor de detalle de un catálogo.
    Catalogs.DeleteValueCatalog = function (url, catalogId) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este valor?', function () {
            HideModalQuestion();

            Petitions.Get(url, function (response) {

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    reloadValueCatalog(catalogId);
                } else {
                    showAlert("Ocurrió un error eliminando la información, verifique que no tenga datos asociados.");
                }
            });
        });
    };

    Catalogs.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    Catalogs.ExportCatalogs = function (url) {
        var fileName = 'Catálogos.xlsx';
        $.ajax({
            url: url,
            method: 'post',
            beforeSend: function () {
                showLoading('Cargando...')
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

    return Catalogs;
})();

function saveCatalog() {
    var catalog = getObject("formcatalogcrud");

    if (formValidation("formcatalogcrud")) {
        ExecuteAjax("Catalog/CreateOrUpdate", "Post", catalog, "resultSaveCatalog");
    }
}
function resultSaveCatalog(response) {
    if (response.Success) {
        showAlert(response.Message, 'success');
    } else {
        showAlert(response.Message, 'danger');
    }
    hideModalPartial();
    Catalogs.Reload();
}

//Valores catalogo
function saveValueCatalog() {
    var valuesCatalog = getObject("formvaluecatalogs");

    if (formValidation("formvaluecatalogs")) {
        ExecuteAjax("ValueCatalog/CreateOrUpdate", "Post", valuesCatalog, "resultSaveValueCatalog");
    }
}

function resultSaveValueCatalog(response) {
    if (response.Success) {
        showAlert(response.Message, 'success');
    } else {
        showAlert(response.Message, 'danger');
    }
    hideModalPartial();
    reloadValueCatalog($("#CatalogId").val());
}

function reloadValueCatalog(catalogId) {
    var urlReload = $("#urlValueCatalog").val().replace('0', catalogId);    
    Catalogs.ReloadValueCatalogs(urlReload, catalogId);
}