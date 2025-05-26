var baseUrl, baseDirectory

var FileDownload = (function () {

    function FileDownload() {

    }

    FileDownload.Init = function () {
        $('.select2').select2();
        baseUrl = $('#UrlBase').val();
        baseDirectory = FileDownload.GetStringValue($('#BaseDirectory').val());

        if ($("#validateCoop").val() == "S") {
            $("#EntityType").change(function () {
                $("#bread-crumbs").html('');
                $("#directory-files").html('');
                FileDownload.GetEntities($(this).val());
            });
        } else {
            FileDownload.GetDirectoriesOrFiles(null);
        }

    };



    FileDownload.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("FileDownload/GetEntityList", "GET", { entityType: entityTypeId }, "FileDownload.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };


    FileDownload.VerifyValuesFilters = function () {
        
        if ($("#EntityType").val() != '' && $("#EntityCode").val() != '') {
            FileDownload.GetDirectoriesOrFiles(null);
        }
    };


    FileDownload.GetDirectoriesOrFiles = function (directory, typeFile, fileName) {


        if (typeFile != undefined && typeFile == 'Archivo') {
            console.log(directory)
            console.log(fileName)
            onclick = ajaxCallPDF('FileDownload/DownloadFile', 'POST', { pathFile: `${FileDownload.GetStringValue(directory)}`}, fileName)
            return;
        }

        if (directory != null) {
            var relativePath = directory.replace(baseDirectory, "");
            $("#bread-crumbs").html(FileDownload.GetBreadCrumbs(relativePath.split("/")))
        } else {
            var directoryPath = baseDirectory.replaceAll("\\", "/") + `/${$("#EntityCode").val()}`;
            var relativePath2 = directoryPath.replace(baseDirectory, "");
            FileDownload.GetBreadCrumbs(relativePath2.split("/"))
        }
        ExecuteAjax(baseUrl + `GetDirectoriesOrFiles?entityCode=${$("#EntityCode").val()}&directory=${directory}`, "Get", null, "FileDownload.ResultDirectories");
    };

    FileDownload.ResultDirectories = function (data) {
        var content = "";

        if (data.length > 0) {
            $.each(data, function (key, value) {
                content += `
                <a onclick="FileDownload.GetDirectoriesOrFiles('${value.Path}', '${value.Type}', '${value.FileName}')" class="nav-link" style="cursor: pointer;">
                    <i class="fas ${(value.Type == 'Carpeta' ? 'fa-folder  folder-color fa-lg fa-2x' : 'fa fa-file-download fa-lg fa-2x')}"></i>
                    <span style="color: #6e707e !important;">${value.FileName}</span> 
                    <span style="color: #6e707e !important;">${(value.Type == 'Carpeta' ? '' : " - [" + value.Date + "]")}</span> 
                </a>`;
            });
        } else {
            content = "<div class='text-center'> Esta carpeta está vacía.</div>";
        }

        $("#directory-files").html(content);
    };


    FileDownload.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };


    FileDownload.GetBreadCrumbs = function (rutas) {
        var breadcrumb = $("#bread-crumbs");
        breadcrumb.empty();

        $.each(rutas, function (index, ruta) {
            if (baseDirectory.includes(ruta)) {
                return true;
            }

            var elemento = $("<span class='breadcrumb-item'>" + ruta + "</span>");

            // Añadir estilo al pasar el ratón
            elemento.hover(
                function () {
                    $(this).css("cursor", "pointer");
                },
                function () {
                    $(this).css("cursor", "auto");
                }
            );

            // Manejar el clic en cada elemento de la miga de pan
            elemento.on("click", function () {
                var rutaSeleccionada = rutas.slice(0, index + 1).join("/");
                FileDownload.GetDirectoriesOrFiles(rutaSeleccionada);
            });

            breadcrumb.append(elemento);

            // Añadir el símbolo de > después de cada elemento, excepto el último
            if (index < rutas.length - 1) {
                breadcrumb.append("<span> > </span>");
            }
        });
    };

    FileDownload.DownloadFile = function (pathFile) {
        showLoading("Descargando archivo...");
        $.ajaxSetup({ cache: false });
        $.ajax({
            ContentType: "application/json",
            url: baseUrl + "DownloadFile",
            type: 'POST',
            data: parameter,
            success: function (data) {
                hideLoading();
                if (data.IsValid) {
                    showAlert("Archivo cargado correctamente", 'success');
                } else {
                    showAlert(data.Message, 'danger');
                }
            },
            error: function (jqXHR, exception) {
                showAlert("No fue posible cargar el archivo, comuniquese con el administrador del sistema.", 'danger');
                hideLoading();
            }
        });
    };

    FileDownload.GetStringValue = function (base64Value) {
        var decodedValue = decodeURIComponent(escape(atob(base64Value)));

        return decodedValue;
    };


    
    return FileDownload;
})();