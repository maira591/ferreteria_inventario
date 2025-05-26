var uploadBoxDiv,
    uploadBoxInputFile,
    uploadBoxLabel,
    baseUrl,
    cooperativeType,
    loadType,
    periodicityType,
    cut,
    slCuts;

const ErrorsTypes = {
    formatErrors: 1,
    dataValidations: 2,
    verticalValidation: 3,
    preventive: 'Preventiva',
    corrective: 'Correctiva'
}
var tableDataErrors;
var tableFormatErrors;
var isDataErrorsAvailable = false;
var isDataFormatAvailable = false;
var hasProcessErrors = false;

/** Obtiene eventos para soporte de Drag and Drop de archivos en el navegador */
var isAdvancedUpload = function () {
    const div = document.createElement('div');
    return ('draggable' in div || 'ondragstart' in div && 'ondrop' in div) &&
        'FormData' in window &&
        'FileReader' in window;
}();

/** Acciones al carga la página */
$(function () {
    $('#nav-loadformats').parent().addClass('active');
    baseUrl = $('#baseUrl').val();
    $.ajaxSetup({
        global: false
    });

    addKeyboardEvents();
    //Elementos de carga
    uploadBoxDiv = $('#upload-box');
    uploadBoxInputFile = $('#AttachedFile');
    uploadBoxLabel = $('#lblAttachedFile');
    cooperativeType = $('#CooperativeType');
    loadType = $('#LoadType');
    //periodicityType = $('#PeriodicityType');
    cut = $('#Cut');
    slCuts = $('#CutDate');
    cut.hide();

    if (isAdvancedUpload) {
        uploadBoxLabel.text('Seleccione o arrastre un archivo .zip');
        let droppedFile = false;

        uploadBoxDiv
            .addClass('has-advanced-upload')
            .on('drag dragstart dragend dragover dragenter dragleave drop',
                function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                })
            .on('dragover dragenter',
                function () {
                    uploadBoxDiv.addClass('is-dragover');
                })
            .on('dragleave dragend drop',
                function () {
                    uploadBoxDiv.removeClass('is-dragover');
                })
            .on('drop',
                function (e) {
                    limpiarErrores();
                    droppedFile = e.originalEvent.dataTransfer.files;
                    uploadBoxInputFile.prop('files', droppedFile);
                    let fechaCorte = slCuts.val();
                    if (fechaCorte !== '') {
                        setUploadBoxLabel(droppedFile);
                        //validarFechaCorte(fechaCorte, function () {
                        //Se agrega paso de seleccion de archivo
                        pasoInicioCarga();
                        //});
                    } else {
                        alertFechaCorte();
                    }
                });
    } else {
        uploadBoxLabel.text('Seleccione un archivo .zip');
    }

    uploadBoxDiv.on('click',
        function () {
            //Verifica si esta habilitada la seleccion por click
            if ($(this).hasClass('clickme')) {
                if (slCuts.val() === '') {
                    //alertFechaCorte();
                    return false;
                }
                uploadBoxInputFile.click();
            }
            return false;
        }
    );

    /**
     * Acciones cuando se cambia el archivo seleccionado
     */
    uploadBoxInputFile.on('change',
        function (e) {
            limpiarErrores();
            changeStep('');
            /** Valida que la fecha de corte seleccionada sea igual a la actual */
            let fechaCorte = slCuts.val();
            if (fechaCorte !== '') {
                setUploadBoxLabel(e.target.files);
                pasoInicioCarga();
            }
            else {
                alertFechaCorte();
            }
        });

    /**
     * Acciones cuando se cambia el tipo de cooperativa
     */
    cooperativeType.on('change',
        function () {
            limpiarErrores();
            changeStep('');
            if ($(this).val() !== '') {
                if (slCuts.val() !== '') {
                    generarIdAgrupador();
                    limpiarArchivo();
                    executeInterval = false;
                    execProcessStates();
                }
            } else {
                limpiarIntervalo();
                disableLoadFormats();
            }
            ajustarInterface();
        });

    /**
     * Acciones cuando se cambia el tipo de periodicidad
     */
    loadType.on('change',
        function () {
            if ($(this).val() !== '') {
                obtenerTipoPeriodicidad().then((value) => {
                    cambiarCampoFechaCorte(value);
                });
                if (slCuts.val() !== '') {
                    executeInterval = false;
                    execProcessStates();
                }

            } else {
                limpiarIntervalo();
                disableLoadFormats();
                changeStep('');
                $("#Cuts").empty();
                $("#Cuts").append("<option value> Seleccione...</option>")
            }
            ajustarInterface();
        });

    /**
     * Acciones cuando se cambia la fecha de corte
     */
    slCuts.on('change',
        function () {
            limpiarErrores();
            changeStep('');
            if ($(this).val() !== '' && cooperativeType.val() !== '') {
                ValidateCargueExtemporaneo()
                generarIdAgrupador();
                limpiarArchivo();
                executeInterval = false;
                execProcessStates();
            } else {
                limpiarIntervalo();
                disableLoadFormats();
            }
            ajustarInterface();
        });

    /**
     * Acciones cuando se cambia la fecha de corte para periodicidad semanal
     */
    cut.on('change',
        function () {
            var fechaSeleccionada = $(this).val();
            var fechaActual = new Date().toLocaleDateString('en-GB', { timeZone: 'UTC' });

            var separadorFechaSeleccionada = fechaSeleccionada.split("/"),
                separadorFechaActual = fechaActual.split("/");

            var diaSeleccionado = separadorFechaSeleccionada[0],
                mesSeleccionado = separadorFechaSeleccionada[1],
                anioSeleccionado = separadorFechaSeleccionada[2];

            var diaActual = separadorFechaActual[0],
                mesActual = separadorFechaActual[1],
                anioActual = separadorFechaActual[2];

            if (diaSeleccionado > diaActual && mesSeleccionado >= mesActual && anioSeleccionado >= anioActual ||
                diaSeleccionado < diaActual && mesSeleccionado > mesActual && anioSeleccionado >= anioActual ||
                anioSeleccionado > anioActual) {

                ShowModalQuestion('Atención',
                    `La fecha de corte seleccionada no puede ser mayor a la fecha actual`,
                    HideModalQuestion,
                    null,
                    true,
                    'Aceptar');
                $('#Cut').val('');
            }
        });

    /**
     * Acción del boton 'Volver a Intentar'
     */
    $('#btn-sendEnd').click(function () {
        const corte = slCuts.val();

        if (corte !== '') {
            enviarFinProceso(corte);
            return;
        }
        alertFechaCorte();
    });
    setPaginationInErrorTables()
});

function pasoInicioCarga() {
    generarIdAgrupador();
    agregarFlujoProceso('InicioCarga', 'SeleccionZip').then(function () {
        executeInterval = false;
        execProcessStates();
        ajustarInterface();
    });
}
/**
 * Cambia el texto del label que se muestra del archivo seleccionado
 * @param {any} file ARCHIVO SELECCIONADO
 */
var setUploadBoxLabel = function (file) {
    uploadBoxLabel.text(file !== '' && file.length > 0
        ? file[0].name
        : isAdvancedUpload
            ? 'Seleccione o arrastre un archivo .zip'
            : 'Seleccione un archivo .zip');
};

/**
 * Funcion para validar la fecha de corte seleccionada
 * @param {any} fechaCorte Fecha de corte
 * @param {function} funcionAEjecutar Funcion a ejecutar al finalizar la operación
 */
//function validarFechaCorte(cutDate, funcionAEjecutar) {
//    showLoading('Validando Fecha Corte...');

//    $.when($.post(baseUrl + 'ValidateCutDate',
//        { cutDate },
//        function (data) {
//            if (!data.isValid) {
//                ShowModalQuestion('Atención',
//                    `Recuerde que al realizar el cargue de cortes anteriores se requiere recalcular los cortes comprendidos entre <b>${data.ammountCutsToRecalculate}</b> ya que se pueden presentar inconsistencias en el cálculo.`,
//                    HideModalQuestion,
//                    null,
//                    true,
//                    'Aceptar');
//            }
//        },
//        'json')).then(function () {
//            hideLoading();
//            funcionAEjecutar();
//        });
//}

/**
 * Envia un fin proceso al carga
 * @param {string} fechaCorte Envia un fin proceso 
 */
function enviarFinProceso(fechaCorte) {
    $.ajax({
        beforeSend: function () { showLoading('Enviando Solicitud...'); },
        url: baseUrl + 'SendEndProcess',
        method: 'POST',
        data: { fechaCorte: fechaCorte, codigoEntidad: $('#OrganizationCode').val() },
        success: function (data) {
            showAlert(`<span class='icon-info h5'></span> Respuesta: <b>${data.Code} - ${data.Message}</b>.`,
                "info");
        },
        complete: function () {
            reload();
        }
    });
}

/** Valida los datos que deben seleccionarse para mostrar el botón 'Validar' o 'Cargar' */
function ajustarInterface() {
    const tipoCoop = cooperativeType.val();
    const corte = slCuts.val();
    const tipoCarga = loadType.val();
    const archivo = uploadBoxInputFile.val();
    $('#btn-upload').addClass('d-none');
    if (corte !== '' && archivo !== '' && tipoCoop !== '' && tipoCarga !== '') {
        $('#frm-file-upload').attr('action', baseUrl + 'ValidateFile');
        $('#frm-file-upload').attr('data-ajax-begin', 'showLoading("Validando Archivo...")');
        $('#btn-validate-file').removeClass('d-none');
    } else {
        $('#btn-validate-file').addClass('d-none');
    }
}


function OnBegin() { showLoading('Validando...'); }
/**
 * Obtiene la informacion devuelta al procesar un archivo
 * @param {any} data Objeto con la información del la respuesta
 */
function fileResult(data) {
    console.info(data);
    let html =
        `<table class="table table-bordered table-striped table-sm table-hover small">
                    <thead>
                        <tr>
                            <th>Resultados Validaciones</th>
                        </tr>
                    </thead>
                <tbody>`;

    $(data.Results).each(function (i, item) {
        const errorIcon = item.IsValid
            ? '<i class="text-success fas fa-check-circle" id="icon-file"></i> '
            : '<i class="text-danger fas fa-window-close" id="icon-file"/></i> ';
        html += `<tr><td class="h6">${errorIcon} ${item.Msg}
            ${item.HasPossibleSolution ? `<span data-tippy-content="${item.PossibleSolution}" class="icon-question" style="margin-left: 4px;" tabindex="0"></span>` : ''}
            </td></tr>`;
    });

    html += '</tbody></table>';

    $('#div-results').html(html);

    if (data.IsUpload) {
        disableLoadFormats();
        $('#div-results').hide();
        $('#btn-validate-file').addClass('d-none');
        $('#btn-upload').addClass('d-none');
        $('#div-results').html('');

        if (data.IsValid) {
            //Se agrega paso de carga de Zip
            agregarFlujoProceso('CargaFtp', 'CargaZip').then(function () {
                encolarProceso().then(function (response) {
                    executeInterval = response.encola;
                    execProcessStates();
                });
            });
        } else {
            showAlert(data.Msg, 'info');
            agregarFlujoProceso('FinProcesoError', 'CargaZip').then(function () {
                executeInterval = false;
                execProcessStates();
            });
        }
        hideLoading();
        return;
    } else if (data.IsValid) {
        //Se agrega paso de validacion de archivo OK
        agregarFlujoProceso('ValidoOk', 'VerificacionZip').then(function () {
            executeInterval = false;
            execProcessStates();
        });

        $('#btn-validate-file').addClass('d-none');
        $('#btn-upload').removeClass('d-none');
        $('#frm-file-upload').attr('action', baseUrl + 'UploadFile');
        $('#frm-file-upload').attr('data-ajax-begin', 'showLoading("Cargando Archivo...")');
        hideLoading();
        return;
    }
    limpiarArchivo();
    //Se agrega paso de validacion de archivo ERROR
    agregarFlujoProceso('ValidoError', 'VerificacionZip').then(function () {
        executeInterval = false;
        execProcessStates();
    });
    $('#btn-validate-file').addClass('d-none');
    $('#btn-upload').addClass('d-none');
    hideLoading();
}

var executeInterval = false;
var intervalStates = null;
/** Obtiene el estado de la subida en el servicio */
function getProcessState() {
    console.info('getProcessState');
    $('#spin-loading-process').removeClass('d-none');
    executeInterval = true;
    intervalStates = window.setInterval(execProcessStates, $('#hfInterval').val());
}

/**
 * Obtiene los estados del proceso del archivo en el FTP
 * @param {bool} executeInterval Si se ejecuta en ciclo
 */
function execProcessStates() {
    let spinLoadingProcess = $('#spin-loading-process');
    let fechaCorte = slCuts.val();
    console.info(`ExecProcess START: ${executeInterval}`);
    isDataErrorsAvailable = false;
    isDataFormatAvailable = false;

    $.ajax({
        beforeSend: function () {
            spinLoadingProcess.removeClass('d-none');
        },
        url: baseUrl + 'ProcessState',
        data: {
            codigoEntidad: $('#OrganizationCode').val(),
            fechaCorte: fechaCorte
        },
        method: 'POST',
        success: function (data) {
            console.log(data);
            let html = '';
            data.uploadResults.forEach(function (item) {
                html = itemResultTable(html, item);
                if (!data.isFinishProcess && (item.Code === 'CargaFtp' || item.Code === 'EnProceso')) {
                    $('#esb-loader').removeClass('d-none');
                    executeInterval = true;
                }
            });

            let htmlLastResult = '';
            if (data.lastResult !== null && data.lastResult !== undefined) {
                htmlLastResult = itemResultTable('', data.lastResult);
                if (!data.isFinishProcess && (data.lastResult.Code === 'CargaFtp' || data.lastResult.Code === 'EnProceso')) {
                    $('#esb-loader').removeClass('d-none');
                    executeInterval = true;
                }
            }

            $('#table-results-body').html(htmlLastResult);
            $('#table-old-results-body').html(html);
            $('#table-results').show('slow');
            $('#table-old-results').show('slow');
            $('#table-old-results-title').show('slow');
            if ($('#div-results').html() !== "") {
                $('#div-results').show('slow');
            }

            if (data.isFinishProcess) {
                if (intervalStates !== null) {
                    executeInterval = false;
                    limpiarIntervalo();
                }
                enableLoadFormats();
                limpiarArchivo();
                obtenerErrores();

                if (data.isError || correctiveErrorsCount > 0 || hasProcessErrors) {
                    changeStep('FinProcesoError');
                } else {
                    changeStep('FinProcesoOk');
                }

                generarIdAgrupador();
                $('#esb-loader').addClass('d-none');

            } else if (!executeInterval) {
                enableLoadFormats();
                if (correctiveErrorsCount > 0) {
                    changeStep('FinProcesoError');
                }
            } else {
                changeStep(data.lastFlowState);
                if (data.lastFlowState === 'CargaFtp') {
                    executeInterval = true;
                }
                disableLoadFormats();
            }
        },
        complete: function () {
            console.info('ExecProcess END: ' + executeInterval);
            if (executeInterval === true) {
                $('#btn-sendEnd').hide();
                limpiarIntervalo();
                getProcessState();
            } else {
                spinLoadingProcess.addClass('d-none');
                $('#btn-sendEnd').show();
            }
            CreateAllTooltips();
        }
    });
}

function itemResultTable(html, item) {
    const errorIcon = ['FinProcesoError'].indexOf(item.Code) !== -1
        ? '<span class="icon-cross text-danger"></span> '
        : '';

    html +=
        `<tr>
                        <td>${item.Date}</td>
                        <td>${errorIcon} ${item.Description}</td>
                        <td>`;
    html += errorIcon !== ''
        ? 'Error técnico <span style="margin-left:4px" class="icon-question" data-tippy-content=\'' +
        item.Detalles +
        '\' tabindex="0"></span>'
        : item.Detalles;

    html += `</td></tr>`;

    if (item.Detalles.toLowerCase().indexOf("error") >= 0) {
        hasProcessErrors = true;
    }

    return html;
}

/**
 * Agrega un flujo de proceso
 * @param {any} codigoEstado Estado de la carga
 * @param {any} codigoProceso Proceso en el que se encuentra
 * @returns {any} null
 */
function agregarFlujoProceso(codigoEstado, codigoProceso) {
    console.info('agregarFlujoProceso: ' + codigoEstado);
    return new Promise((resolve, reject) => {
        $.post(baseUrl + 'AddFlowProccess',
            {
                codigoEntidad: $('#OrganizationCode').val(),
                fechaCorte: slCuts.val(),
                codigoEstado,
                codigoProceso,
                agrupador: $('#hfIdAgrupador').val()
            },
            function (response) {
                //limpiarErrores();
                changeStep(codigoEstado); resolve(response);
            }
        );
    });
}

/**
 * * Agrega el proceso en cola
 */
function encolarProceso() {
    console.info('encolarProceso');
    return new Promise((resolve, reject) => {
        $.post(baseUrl + 'EnQueue',
            {
                codigoEntidad: $('#OrganizationCode').val(),
                loadType: loadType.val(),
                cooperativeType: cooperativeType.val(),
                fechaCorte: slCuts.val(),
                agrupador: $('#hfIdAgrupador').val()
            },
            function (response) { changeStep(response.Estado); resolve(response); }
        );
    });
}

/**Agrega estilos para las acciones de carga de formatos */
function enableLoadFormats() {
    console.info('enableLoadFormats');
    uploadBoxLabel.prop('tabindex', '0');
    $('#icon-file').css('opacity', '1');
    uploadBoxDiv.removeClass('noclick').addClass('clickme');
}

/**Remueve los estilos para las acciones de carga de formatos */
function disableLoadFormats() {
    console.info('disableLoadFormats');
    uploadBoxLabel.prop('tabindex', '-1');
    $('#icon-file').css('opacity', '0.6');
    uploadBoxDiv.removeClass('clickme').addClass('noclick');
}

/**
 * Modifica las clases de los pasos
 * @param {any} stepElements paso al cual ajusta el wizard
 */
function ajustarPasos(stepElements) {
    for (let i = 0; i < stepElements.length; i++) {
        $(stepElements[i].ObjectDom).removeClass('done doing error');
        $(stepElements[i].ObjectDomText).hide();
        if (stepElements[i].AdditionalCssClass !== undefined && stepElements[i].AdditionalCssClass !== '') {
            $(stepElements[i].ObjectDom).addClass(stepElements[i].AdditionalCssClass);
        }

        if (stepElements[i].IsShow) {
            $(stepElements[i].ObjectDomText).show();
        }
    }
}

/**
 * Cambia de estado los elementos del wizard
 * @param {any} codigoEstado codigo del estado al cual cambiar
 */
function changeStep(codigoEstado) {
    //console.log("codigoEstado: " + codigoEstado);
    switch (codigoEstado) {
        case 'ValidoOk':
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-1'
                },
                {
                    ObjectDom: '#step-2-number',
                    ObjectDomText: '#step-2',
                    AdditionalCssClass: 'doing',
                    IsShow: true
                },
                {
                    ObjectDom: '#step-3-number',
                    ObjectDomText: '#step-3'
                }
            ]);
            break;
        case 'ValidoError':
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'error',
                    ObjectDomText: '#step-1',
                    IsShow: true
                },
                {
                    ObjectDom: '#step-2-number',
                    ObjectDomText: '#step-2'
                },
                {
                    ObjectDom: '#step-3-number',
                    ObjectDomText: '#step-3'
                }
            ]);
            break;
        case 'ErrorEncolamiento':
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-1'
                },
                {
                    ObjectDom: '#step-2-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-2'
                },
                {
                    ObjectDom: '#step-3-number',
                    AdditionalCssClass: 'error',
                    ObjectDomText: '#step-3',
                    IsShow: true
                }
            ]);
            break;
        case 'CargaFtp':
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-1'
                },
                {
                    ObjectDom: '#step-2-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-2'
                },
                {
                    ObjectDom: '#step-3-number',
                    AdditionalCssClass: 'doing',
                    ObjectDomText: '#step-3',
                    IsShow: true
                }
            ]);
            break;
        case 'Encola':
        case 'EnProceso':
        case 'InicioProcesarFormatos':
        case 'InicioInsertarInformacionFormatos':
        case 'FinInsertarInformacionFormatos':
        case 'FinProcesarFormatos':
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-1'
                },
                {
                    ObjectDom: '#step-2-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-2'
                },
                {
                    ObjectDom: '#step-3-number',
                    AdditionalCssClass: 'doing',
                    ObjectDomText: '#step-3',
                    IsShow: true
                }
            ]);
            break;
        case 'FinProcesoOk':
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-1'
                },
                {
                    ObjectDom: '#step-2-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-2'
                },
                {
                    ObjectDom: '#step-3-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-3',
                    IsShow: true
                }
            ]);
            break;
        case 'FinProcesoError':
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-1'
                },
                {
                    ObjectDom: '#step-2-number',
                    AdditionalCssClass: 'done',
                    ObjectDomText: '#step-2'
                },
                {
                    ObjectDom: '#step-3-number',
                    AdditionalCssClass: 'error',
                    ObjectDomText: '#step-3',
                    IsShow: true
                }
            ]);
            break;
        default:
            ajustarPasos([
                {
                    ObjectDom: '#step-1-number',
                    AdditionalCssClass: 'doing',
                    ObjectDomText: '#step-1',
                    IsShow: true
                },
                {
                    ObjectDom: '#step-2-number',
                    ObjectDomText: '#step-2'
                },
                {
                    ObjectDom: '#step-3-number',
                    ObjectDomText: '#step-3'
                }
            ]);
            break;
    }
}

/** Borra los errores de las tablas y la cantidad */
function limpiarErrores() {
    console.info('Limpiar Errores');
    correctiveErrorsCount = 0;
    $('#div-results').html('');
    $('#div-results').hide();
    $('#esb-loader').addClass('d-none');
    const msgVacio = '<tr><td colspan="2">La consulta no devolvió resultados.</td></tr>';
    $('#table-format-errors-body').html(msgVacio);
    $('#table-data-errors-body').html(msgVacio);
    $('#count-format-errors').removeClass().html('');
    $('#count-data-errors').removeClass().html('');
    $('#count-data-alerts').removeClass().html('');
    $('#table-results-body').html('');
    $('#table-old-results-body').html('');
    $('#table-results').hide();
    $('#table-old-results').hide();
    $('#table-old-results-title').hide();
    $('#btn_downloadformaterrors').hide();
    $('#btn_downloaddataerrors').hide();
    $('#info-erros').hide();
    $('#errors-count').html('');
    hasProcessErrors = false;
}

/** Limpia el intervalo para la consulta de errores */
function limpiarIntervalo() {
    if (intervalStates !== null) {
        window.clearInterval(intervalStates);
    }
}

/** Genera un id de proceso para la carga actual */
function generarIdAgrupador() {
    $.ajax({
        async: false,
        url: baseUrl + 'GenerateGrouperId',
        success: function (data) {
            $('#hfIdAgrupador').val(data);
        }
    });
}

/** Mostrar advertencia de cargue extemporáneo */
function ValidateCargueExtemporaneo() {
    $.ajax({
        url: baseUrl + `ValidateCargueExtemporaneo?fechaCorte=${$('#Cuts').val()}&cooperativeTypeId=${$('#CooperativeType').val()}&loadTypeId=${$('#LoadType').val()}`,
        success: function (data) {
            if (data != "") {
                showAlert(data, "warning", true)
            }
        }
    });
}

/** Obtiene el tipo de periodicidad para la carga */
async function obtenerTipoPeriodicidad() {
    var periodicityData = null;
    await $.ajax({
        url: baseUrl + 'GetPeriodicityType',
        beforeSend: function(){ showLoading('Cargando fechas...'); },
        data: {
            loadTypeId: loadType.val(),
        },
        method: 'GET',
        success: function (data) {
            periodicityData = data;
        },
        complete: function(){ hideLoading(); }
    });
    return periodicityData;
}

/** Obtiene el listado de fechas de corte de acuerdo a la periodicidad */
function generarFecha() {
    showLoading();
    $.ajax({
        url: baseUrl + 'GenerateDate',
        data: {
            loadTypeId: loadType.val(),
        },
        method: 'GET',
        success: function (data) {
            $("#Cuts").empty();
            $("#Cuts").append("<option value> Seleccione...</option>")
            $.each(data, function (index, row) {
                $("#Cuts").append("<option value='" + row.Value + "'>" + row.Text + "</option>")
            });
        },
        complete: function () {
            hideLoading();
        }
    });
}

/** Configura el campo fecha de corte de acuerdo al tipo de periodicidad seleccionada */
function cambiarCampoFechaCorte(periodicidad) {

    if (periodicidad !== 'Esporadica') {
        $("#Cuts").show();
        $("#Cut").hide();
        $("#Cut").val('');
        $("#CutDate").val('');
        slCuts = $('#CutDate').empty();
        generarFecha();
    }
    else {
        $("#Cuts").hide();
        $("#Cut").show();
        $("#Cut").datepicker({ format: 'dd/mm/yyyy' });
        $("#CutDate").val('');
        slCuts = $('#Cut').empty();
    }

    if (slCuts.val() === '') {
        limpiarIntervalo();
        disableLoadFormats();
        changeStep('');
    }
}

var correctiveErrorsCount = 0;
function obtenerErrores() {
    console.log('obtenerErrores');
    let fechaCorte = slCuts.val();
    let spinFormatsTab = $('#spin-formats-tab'),
        spinDataTab = $('#spin-data-tab');
    var showDownLoadFormatErrors = false;
    var showDownLoadDataErrors = false;

    $.ajax({
        async: false, 
        url: baseUrl + 'GetErrors',
        data: {
            codigoEntidad: $('#OrganizationCode').val(),
            fechaCorte: fechaCorte
        },
        beforeSend: function () {
            spinFormatsTab.removeClass('d-none');
            spinDataTab.removeClass('d-none');
        },
        method: 'GET',
        success: function (data) {
            if (data.Errors.length > 0) {
                tableDataErrors.destroy();
                tableFormatErrors.destroy();

                $("#table-format-errors-body").empty();
                $("#table-data-errors-body").empty();

                let formatErrors = data.Errors.filter(element => element.ErrorType === ErrorsTypes.formatErrors);
                let dataErrors = data.Errors.filter(element => (element.ErrorType === ErrorsTypes.dataValidations ||
                    element.ErrorType === ErrorsTypes.verticalValidation));

                let preventiveErrors = dataErrors.filter(element => element.TypeValidation === ErrorsTypes.preventive);
                let correctiveErrors = dataErrors.filter(element => element.TypeValidation === ErrorsTypes.corrective);
                correctiveErrorsCount = correctiveErrors.length;

                $('#count-format-errors').removeClass().addClass("badge badge-danger").html(formatErrors.length);
                $('#count-data-errors').removeClass().addClass("badge badge-danger").html(correctiveErrorsCount);
                $('#count-data-alerts').removeClass().addClass("badge badge-warning").html(preventiveErrors.length);

                if (formatErrors.length > 0) {
                    showDownLoadFormatErrors = true;
                    isDataFormatAvailable = true;
                }
                if (correctiveErrorsCount > 0 || preventiveErrors.length > 0) {
                    showDownLoadDataErrors = true;
                    isDataErrorsAvailable = true;
                }

                $.each(formatErrors, function (index, element) {
                    $('<tr>' +
                        '<td>' + element.FormatName + '</td>' +
                        '<td>' + element.ColumnName + '</td>' +
                        '<td>' + element.RowPosition + '</td>' +
                        '<td>' + element.ErrorDescription + '</td>' +
                        '<td>' + formatDate(element.CreatedOn, 'YYYY-MM-DD HH:mm:ss') + '</td>' +
                        +'</tr>').appendTo("#table-format-errors-body").click();
                });

                $.each(dataErrors, function (index, element) {
                    $('<tr>' +
                        '<td>' + element.TypeValidation + '</td>' +
                        '<td>' + element.FormatName + '</td>' +
                        '<td>' + element.ErrorDescription + '</td>' +
                        '<td>' + (element.PossibleSolution === null ? '' : element.PossibleSolution) + '</td>' +
                        '<td>' + formatDate(element.CreatedOn, 'YYYY-MM-DD HH:mm:ss') + '</td>' +
                        +'</tr>').appendTo("#table-data-errors-body").click();
                });

                $('#errors-count').html('('+data.Count+')');

            } else {
                $('#btn_downloadformaterrors').hide();
                $('#btn_downloaddataerrors').hide();
                $('#info-erros').hide();
                $('#table-format-errors-body').html('<tr><td colspan="3">La consulta no devolvió resultados.</td></tr>');
                $('#table-data-errors-body').html('<tr><td colspan="3">La consulta no devolvió resultados.</td></tr>');
            }

            if (showDownLoadFormatErrors) {
                $('#info-erros').show();
                $('#btn_downloadformaterrors').show();
            }
            if (showDownLoadDataErrors) {
                $('#info-erros').show();
                $('#btn_downloaddataerrors').show();
            }
        },
        complete: function () {
            spinFormatsTab.addClass('d-none');
            spinDataTab.addClass('d-none');
            CreateAllTooltips();
            refreshErrorTables();
        }
    });
}

function exportErrors(errorType) {
    var fileName = "";

    if (errorType == ErrorsTypes.formatErrors) {
        fileName = "ErroresFormatos.xlsx"
    } else {
        fileName = "ErroresDatos.xlsx"
    }

    $.ajaxSetup({ cache: false });
    $.ajax({
        url: baseUrl + 'ExportExcelErrors',
        method: 'post',
        data: {
            codigoEntidad: $('#OrganizationCode').val(),
            fechaCorte: slCuts.val(),
            downLoadType: errorType
        },
        beforeSend: function () {
            $("#modal-loading-msg").html("Generando archivo...");
            $("#modal-loading").modal("show");
            $("#modal-loading-back").show();
        },
        xhrFields: { responseType: 'blob' },
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
}

/** Muestra la advertencia de seleccion de fecha de corte */
function alertFechaCorte() {
    showAlert("<span class='icon-warning h5'></span> Por favor seleccione una <b>Fecha de Corte</b>.", "info");
}

/** Limpia el archivo seleccionado de la página */
function limpiarArchivo() {
    uploadBoxInputFile.val('');
    setUploadBoxLabel('');
}

function setPaginationInErrorTables() {
    tableDataErrors = $('#table-data-errors').DataTable(
        {
            "columns": [
                { "width": "20%" },
                { "width": "20%" },
                { "width": "20%" },
                { "width": "20%" },
                { "width": "20%" }
            ]
        }
    );
    tableFormatErrors = $('#table-format-errors').DataTable(
        {
            "columns": [
                { "width": "20%" },
                { "width": "20%" },
                { "width": "20%" },
                { "width": "20%" },
                { "width": "20%" }
            ]
        }
    );

    $("#table-data-errors_wrapper").css("width", "99%");
    $("#table-format-errors_wrapper").css("width", "99%");
}

function refreshErrorTables() {
    if ($.fn.dataTable.isDataTable('#table-data-errors')) {
        tableDataErrors.clear().draw();
        tableDataErrors.destroy();
    }
    if ($.fn.dataTable.isDataTable('#table-format-errors')) {
        tableFormatErrors.clear().draw();
        tableFormatErrors.destroy();
    }
    setPaginationInErrorTables();
}
function tabChange(tab) {

    try {
        setTimeout(function () {
            tableFormatErrors = $('#table-format-errors').DataTable();
            tableDataErrors = $('#table-data-errors').DataTable();

            if (tab == 'format') {
                tableFormatErrors.columns.adjust().draw();
                if (!isDataFormatAvailable) {
                    $('#table-format-errors').dataTable().fnClearTable();
                }
            }
            else {
                tableDataErrors.columns.adjust().draw();
                if (!isDataErrorsAvailable) {
                    $('#table-data-errors').dataTable().fnClearTable();
                }
            }
        }, 100);
    } catch (e) { console.log("tabChange error") }

}