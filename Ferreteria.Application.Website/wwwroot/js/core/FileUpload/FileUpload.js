var uploadBoxDiv,
    uploadBoxInputFile,
    uploadBoxLabel,
    baseUrl;

var setUploadBoxLabel = function (file) {
    uploadBoxLabel.text(file !== '' && file.length > 0
        ? file[0].name
        : isAdvancedUpload
            ? 'Seleccione o arrastre un archivo'
            : 'Seleccione un archivo');
};

function fileResult(data) {
    if (data.IsValid) {
        $('#frm-file-upload').attr('action', baseUrl + 'UploadFile');
        $('#frm-file-upload').attr('data-ajax-success', 'fileUploadResult');
        $('#frm-file-upload').attr('data-ajax-begin', 'showLoading("Cargando Archivo...")');
        $('#btn-validate-file').removeClass('d-none');
        $('#btn-validate-file').click()
    } else {
        showAlert(`${data.Message}`, 'danger')
    }
}

var isAdvancedUpload = function () {
    const div = document.createElement('div');
    return ('draggable' in div || 'ondragstart' in div && 'ondrop' in div) &&
        'FormData' in window &&
        'FileReader' in window;
}();

function fileUploadResult(data) {
    if (data.IsValid) {
        showAlert("Archivo cargado correctamente.", 'success');
        setTimeout(function () {
            location.reload();
        }, 2000);

    } else {
        showAlert(`${data.Message}`, 'danger')
    }
}

var FileUpload = (function () {

    function FileUpload() {

    }

    FileUpload.Init = function () {
        //Elementos de carga
        uploadBoxDiv = $('#upload-box');
        uploadBoxInputFile = $('#AttachedFile');
        uploadBoxLabel = $('#lblAttachedFile');

        $('.select2').select2();
        baseUrl = $('#BaseUrl').val();
        uploadBoxDiv.on('click',
            function () {
                //Verifica si esta habilitada la seleccion por click
                if ($(this).hasClass('clickme')) {
                    uploadBoxInputFile.click();
                }
                return false;
            }
        );

        uploadBoxInputFile.on('change',
            function (e) {

                var sizeFile = this.files[0].size / (1024 * 1024)
                if (sizeFile > $("#FileSizeDefault").val()) {
                    setUploadBoxLabel('')
                    showAlert(`El peso del archivo (${sizeFile.toFixed(2) }MB) supera el peso máximo permitido por el sistema (${$("#FileSizeDefault").val()}MB)`, 'danger');
                    this.value = "";
                    return;
                }

                setUploadBoxLabel(e.target.files);
                if ($("#EntityType").val() != '' && $("#EntityCode").val() != '') {
                    $('#frm-file-upload').attr('action', baseUrl + 'ValidateFile');
                    $('#frm-file-upload').attr('data-ajax-begin', 'showLoading("Validando Archivo...")');
                    $('#btn-validate-file').removeClass('d-none');
                } else {
                    $('#btn-validate-file').addClass('d-none');
                }
            });


        if ($("#validateCoop").val() == "S") {
            FileUpload.DisableLoadFormats()
            $("#EntityType").change(function () {
                FileUpload.GetEntities($(this).val());
            });
        } else {
            FileUpload.EnableLoadFormats()
        }

    };



    FileUpload.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("FileUpload/GetEntityList", "GET", { entityType: entityTypeId }, "FileUpload.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };


    FileUpload.UploadFile = function (parameter) {
        showLoading("Cargando archivo...");
        $.ajaxSetup({ cache: false });
        $.ajax({
            ContentType: "application/json",
            url: baseUrl + "UploadFile",
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


    FileUpload.VerifyValuesFilters = function () {
        setUploadBoxLabel('')
        $("#AttachedFile").val('')
        if ($("#EntityType").val() != '' && $("#EntityCode").val() != '') {
            FileUpload.EnableLoadFormats()
        } else {
            FileUpload.DisableLoadFormats()
        }
    };




    FileUpload.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };

    /**Agrega estilos para las acciones de carga de archivos */
    FileUpload.EnableLoadFormats = function () {
        uploadBoxLabel.prop('tabindex', '0');
        $('#icon-file').css('opacity', '1');
        uploadBoxDiv.removeClass('noclick').addClass('clickme');
        $('#btn-validate-file').addClass('d-none');
    }


    /**Remueve los estilos para las acciones de carga de archivos */
    FileUpload.DisableLoadFormats = function () {
        uploadBoxLabel.prop('tabindex', '-1');
        $('#icon-file').css('opacity', '0.6');
        uploadBoxDiv.removeClass('clickme').addClass('noclick');
        $('#btn-validate-file').addClass('d-none');
    }



    return FileUpload;
})();