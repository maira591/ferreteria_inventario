// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/**
 * Show a alert msg in the page
 * @param {string} msg Message to display
 * @param {string} typeAlert Opcional tipo de alerta a mostrar success, primary, warning, danger, info. DEFAULT danger
 * @param {boolean} clickToHide Opcional Se debe hacer click para ocultar. DEFAULT se oculta a los 5 segundos
 */
function showAlert(msg, typeAlert, clickToHide, offsetx, offsety) {
    hideLoading();
    $.notify({
        // options
        message: msg
    }, {
        // settings
        type: typeAlert === undefined ? "danger" : typeAlert,
        z_index: 100000,
        showProgressbar: false,
        mouse_over: "pause",
        offset: {
            x: offsetx === undefined ? 20 : offsetx,
            y: offsety === undefined ? 80 : offsety
        },
        delay: clickToHide ? 0 : 5000,
        animate: {
            enter: "animated fadeInDown",
            exit: "animated fadeOutUp"
        }
    });
}


function addKeyboardEvents() {
    $('.gel-tab-key').off('keyup');
    //Enter and Space key
    $('.gel-tab-key').keyup(function (e) {
        e.preventDefault();

        if (e.keyCode === 13 || e.keyCode === 32) {
            $(this).click();
        }
    });
}

/** Hide the modal partial */
function hideModalPartial() {
    setTimeout(function () { $("#modal-partial").modal("hide"); }, 300);
}
/**Refresh the current page */
function reload() {
    window.location.reload();
}
/**
 * Show loading modal with promise
 * @param {string} msg Message to display while loading
 * @return {void}
 */
function showLoading(msg) {
    return new Promise((resolve, reject) => {
        var message = msg ? msg : "Cargando...";
        $("#modal-loading-msg").html(message);
        $("#modal-loading").modal("show");
        $("#modal-loading-back").show();

        setTimeout(function () {
            resolve();
        }, 650);
    });
}
/**
 * Show loading modal
 * @param {any} msg Message to display while loading
 * @return {void}
 */
function setTextLoading(msg) {
    var message = msg ? msg : "Cargando...";
    $("#modal-loading-msg").html(message);
}

/** Hide loading modal */
function hideLoading() {
    $("#modal-loading").modal("hide");
    $("#modal-loading-back").hide();
}

/**
 * Shows a modal question
 * @param {string} title title of modal
 * @param {any} message message to show
 * @param {any} functionYes function on select yes
 * @param {any} functionNo function on select no
 * @param {any} onlyYesButton display only button yes
 * @param {any} yesButtonText set yes button text 
 */
function ShowModalQuestion(title, message, functionYes, functionNo, onlyYesButton, yesButtonText, notButtonText) {
    $("#titleModalQuestion").html(title);
    $("#messageModalQuestion").html(message);

    $("#btnSiModalQuestion").off("click");
    $("#btnNoModalQuestion").off("click");
    $("#btnNoModalQuestion").on("click", function () {
        HideModalQuestion();
    });

    if (typeof (functionYes) !== "undefined") {
        $("#btnSiModalQuestion").off("click");
        $("#btnSiModalQuestion").on("click", functionYes);
    }

    if (typeof (functionNo) !== "undefined") {
        $("#btnNoModalQuestion").off("click");
        $("#btnNoModalQuestion").on("click", functionNo);
    }

    if (yesButtonText !== undefined) {
        $("#btnSiModalQuestion").html(yesButtonText);
    }

    if (notButtonText !== undefined) {
        $("#btnNoModalQuestion").html(notButtonText);
    }

    if (onlyYesButton) {
        $("#btnNoModalQuestion").hide();
    }

    $("#backModalQuestion").show();
    $("#modalQuestion").modal("show");
}

/** Hide the Modal Question
 * @returns {any} void
 */
function HideModalQuestion() {
    return new Promise((resolve, reject) => {
        $("#backModalQuestion").hide();
        $("#modalQuestion").modal("hide");
        setTimeout(function () {
            resolve();
        },
            650);
    });
}

function CreateAllTooltips() {
    tippy('[data-tippy-content]', {
        arrow: true,
        arrowType: 'round',
        animation: 'scale'
    });
}

function ExecuteAjax(url, type, values, funcionSuccess, parameter) {
    showLoading();
    $.ajaxSetup({ cache: false });

    $.ajax({
        ContentType: "application/json",
        url: url,
        type: type,
        data: values,
        success: function (data) {
            if (funcionSuccess.indexOf(".") >= 0) {
                executeFunction(funcionSuccess, window, data, parameter)
            }
            else {
                window[funcionSuccess](data, parameter);
            }            
            hideLoading();
        },
        error: function (jqXHR, exception) {            
            showAlert("Unexpected error", "warning");
            hideLoading();
        }
    });
}

/**
 * Show the modal partial validation form
 * @param {object} data object
 */
function showModalResult(data) {
    if (typeof data !== "undefined")
        if (data.message === "Access denied") {
            window.location.href = data.url;
            return;
        }

    var options = { "backdrop": "static", keyboard: true };
    $("#modal-partial").modal(options);
    showModalPartial();
    setAlphanumericInput();
    setInputRestriction();
    setNumericInput();
}

/** Show the modal partial */
function showModalPartial() {

    $("#modal-partial").off("shown.bs.modal");
    $("#modal-partial").on("shown.bs.modal", function () {
        $("body").removeClass("modal-open").addClass("modal-open");
    });

    $("#modal-partial").off("hidden.bs.modal");
    $("#modal-partial").on("hidden.bs.modal", function () {
        $("body").removeClass("modal-open");
    });

    $("#modal-partial").modal("show");    

    addKeyboardEvents();
    CreateAllTooltips();
}

function getObject(formId) {
    return $("#" + formId).serializeArray();
}

//Valida el contenido de un formulario
function formValidation(formId, showmessage = true) {
    removeTooltip()

    var validations = [];
    validations.push(requiredFieldValidation(formId));
    validations.push(regularExpValidation(formId));
    validations.push(lengthValidation(formId));

    showTooltip();
    if ($.inArray(false, validations) !== -1 && showmessage) {
        showAlert("Hay inconsistencias en el formulario, revise los campos demarcados con color rojo.", "warning")        
    }

    return $.inArray(false, validations) === -1;
}

function showTooltip() {
    $(".errorValidate").mouseover(function () {
        if ($(this).attr("data-errormessage") !== undefined) {
            if ($(this).attr("data-errormessage").length > 0) {
                $(this).parent().append("<div class='tooltipError'>" + $(this).attr("data-errormessage") + "</div>");
                if ($(this).parent().prop("tagName") == "TD") {
                    $('.tooltipError').css('right', 'auto');
                    $('.tooltipError').css('top', 'auto');
                }
            }
        }
    });
    $(".errorValidate").mouseout(function () {
        $(this).parent().find(".tooltipError").remove();
    });

    $(".errorValidateBorder").mouseover(function () {
        if ($(this).attr("data-errormessage") !== undefined) {
            if ($(this).attr("data-errormessage").length > 0) {
                $(this).parent().append("<div class='tooltipError'>" + $(this).attr("data-errormessage") + "</div>");
                if ($(this).parent().prop("tagName") == "TD") {
                    $('.tooltipError').css('right', 'auto');
                    $('.tooltipError').css('top', 'auto');
                }
            }
        }
    });
    $(".errorValidateBorder").mouseout(function () {
        $(this).parent().find(".tooltipError").remove();
    });
}
function removeTooltip() {
    $(".errorValidate").off("mouseover");
    $(".errorValidate").off("mouseout");

    $(".errorValidateBorder").off("mouseover");
    $(".errorValidateBorder").off("mouseout");
}

//Validacion para campos obligatorios
function requiredFieldValidation(formId) {
    var requested = $("#" + formId).find(".required");
    var ok = true;    
    $("#" + formId + " .errorValidate").removeClass("errorValidate");
    $("#" + formId + " .errorValidateBorder").removeClass("errorValidateBorder");
    $.each(requested, function (index, value) {
        var group = $(this)[0].tagName;
        var tipo = $(this)[0].type;
        switch (group) {
            case "SELECT":
                if ($(this).val().length === 0) {
                    if ($(this).hasClass("select2") || $(this).hasClass("multiselect")) {
                        $(this).next().attr("data-errormessage", "Este campo es obligatorio");
                        $(this).next().addClass("errorValidate");
                        if (tipo === "select-multiple") {
                            $(this).next().find(".multiselect").addClass("errorValidate");
                        }
                        else {
                            $(this).next().find(".select2-selection").addClass("errorValidate");
                        }                        
                    }
                    else {
                        $(this).attr("data-errormessage", "Este campo es obligatorio");
                        $(this).addClass("errorValidate");
                    }
                    ok = false;
                }
                break;
            case "INPUT":
                switch (tipo) {
                    case "text":
                        if ($(this).val() === "") {
                            $(this).attr("data-errormessage", "Este campo es obligatorio");
                            $(this).addClass("errorValidate");
                            ok = false;
                        }
                        break;
                    case "number":
                        if ($(this).val() === "") {
                            $(this).attr("data-errormessage", "Este campo es obligatorio");
                            $(this).addClass("errorValidate");
                            ok = false;
                        }
                        break;
                    case "password":
                        if ($(this).val() === "") {
                            $(this).attr("data-errormessage", "Este campo es obligatorio");
                            $(this).addClass("errorValidate");
                            ok = false;
                        }
                        break;
                }
                break;
            case "TEXTAREA":
                if ($(this).val() === "") {
                    if ($(this).next().hasClass("ace_editor")) {
                        $(this).next(".ace_editor").addClass("errorValidateBorder")
                        $(this).next(".ace_editor").attr("data-errormessage", "Este campo es obligatorio");
                    }

                    $(this).attr("data-errormessage", "Este campo es obligatorio");
                    $(this).addClass("errorValidate");
                    ok = false;
                }
                break;
        }
    });
    return ok;
}
//valida la estructura de un Email
function regularExpValidation(formId) {    
    var _listEmail = $("#" + formId).find(".email");
    var ok = true;

    $.each(_listEmail, function (i, item) {

        $(this).removeClass("errorValidate");
        var group = $(this)[0].tagName;
        var type = $(this)[0].type;

        switch (group) {
            case "INPUT":
                switch (type) {
                    case "text":
                        if ($(this).val().length > 0) {                            
                            if (!validarEmail($(this).val())) {
                                $(this).attr("data-errormessage", "El Email es invalido");
                                $(this).addClass("errorValidate");
                                ok = false;
                            }
                        } else {
                            if (this.classList.contains("required")) {

                                $(this).attr("data-errormessage", "Este campo es obligatorio");
                                $(this).addClass("errorValidate");
                                ok = false;
                            }
                        }
                        break;
                }
                break;
        }
    });    
    return ok;
}

function lengthValidation(formId) {
    var ok = true;
    var range = $("#" + formId).find(".lengthval");

    $.each(range, function (index, value) {
        if ($(value).val().length <= parseInt($(value).data("minlength"))) {
            $(this).attr("data-errormessage", $(this).attr("data-mensaje"));
            $(this).addClass("errorValidate");
            ok = false;
        }
    });
    return ok;
}
//Set input as only text and numbers.
function setAlphanumericInput() {
    $(".textNumber").each(function (index, elemento) {
        $(elemento).keypress(function () {
            return isAlphaNum();
        });
    });

    $(".textNumber2").each(function (index, elemento) {
        $(elemento).keypress(function () {
            return isAlphaNum2();
        });
    });
}
function isAlphaNum() {
    var format = /[`!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~ ]/;
    return !format.test(event.key);
}
function isAlphaNum2() {
    var format = /[`!@#$%^&*()+\-=\[\]{};':"\\|,.<>\/?~ ]/;
    return !format.test(event.key);
}
function setInputRestriction() {
    $(".textNumber").on("change", function () {
        validCharacter(this);
    });
    $(".textNumber2").on("change", function () {
        validCharacter2(this);
    });

}
function validCharacter(control) {
    $(control).val($(control).val().replace(/[^a-zA-Z0-9]/g, ''));
}
function validCharacter2(control) {
    $(control).val($(control).val().replace(/[^a-zA-Z0-9_]/g, ''));
}
function setNumericInput() {
    $(".inputPositiveInput").numeric({ negative: false, decimal: false });
    $(".inputPositiveInput2d").numeric({ negative: false, decimalPlaces: 2 });
    $(".inputPositiveInput4d").numeric({ negative: false, decimalPlaces: 4 });
    $(".inputPositiveInput6d").numeric({ negative: false, decimalPlaces: 6 });
}
//Replace all coincidences in text
function replaceAll(text, findValue, replaceText) {
    return text.replace(new RegExp(regularExpReplace(findValue), 'g'), replaceText);
}
function regularExpReplace(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}
//Execute a function by its name, taking into account the namespace.
function executeFunction(functionName, context /*, args */) {

    try {
        var args = Array.prototype.slice.call(arguments, 2);
        var namespaces = functionName.split(".");
        var func = namespaces.pop();
        for (var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }
        return context[func].apply(context, args);
    } catch(ex) {
        hideLoading();
        showAlert("No fue posible ejecutar la función " + functionName, 'danger');        
    }
}
function setSelect2() {
    $('.select2').select2({
        dropdownParent: $('#modal-partial')
    }); 
}

function formatDate(date, format) {
    if (new Date(date) instanceof Date) {
        return moment(date).format(format);
    }
    return "";    
}

function isValidInteger(idElement, maxIntegerValue, field) {

    var integerValue = $("#" + idElement).val().split('.')[0].length;

    $("#" + idElement).removeClass("errorValidate");
    $("#" + idElement).attr("data-errormessage", "");

    if (integerValue > maxIntegerValue) {
        $("#" + idElement).addClass("errorValidate");
        $("#" + idElement).attr("data-errormessage", "Error en la parte entera.");
        showAlert("La parte entera del campo " + field + " no puede ser mayor a " + maxIntegerValue + " dígitos.", 'danger');
        showTooltip();
    } 

    return !(integerValue > maxIntegerValue);
}

function setDateTimePicker() {
    $('.timepicker').datetimepicker({
        format: 'd/m/Y H:i:s',
        formatTime: "H:i",
        formatDate: 'd/m/Y',
        timepicker: true,
        datepicker: false,
        step: 30
    });
}

function ConvertStringToDate(stringDate) {
    var dateSplit = stringDate.split("/");
    var date = new Date(`${dateSplit[2]}/${dateSplit[1]}/${dateSplit[0]}`);
    return date;
}

function createDate(dateString) {

    var dateParts = dateString.split("/");

    return new Date(Date.UTC(+dateParts[2], dateParts[1] - 1, +dateParts[0]));
}


function ConvertNullValue(value) {
    return value === null ? "No disponible" : value;
}

function getCurrentDate() {

    var fechaActual = new Date();
    var año = fechaActual.getFullYear();
    var mes = (fechaActual.getMonth() + 1).toString().padStart(2, '0');
    var dia = fechaActual.getDate().toString().padStart(2, '0');

    var fechaFormateada = año + "-" + mes + "-" + dia;

    return fechaFormateada;
}


function ajaxCallPDF(url, type, values, fileName) {
    if (!fileName) {
        fileName = "file.pdf";
    }
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: url,
        method: type,
        data: values,
        beforeSend: function () {
            showLoading("Generando archivo...");
        },
        xhrFields: { responseType: 'blob' },
        success: function (data) {
            var blob = new Blob([data], { type: 'application/pdf' });
            var downloadUrl = URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = downloadUrl;
            a.download = fileName;
            a.target = '_blank';
            a.click();

            showLoading("Archivo descargado correctamente.");
        },
        error: function (jqXHR, exception) {
            hideLoading();
            showAlert("No fue posible descargar el archivo.", "danger");
        },
        complete: function () {
            hideLoading();
        }
    });
}

function ConvertStringToDate(stringDate) {
    var dateSplit = stringDate.split("-");
    var year = parseInt(dateSplit[0]);
    var month = parseInt(dateSplit[1]);

    // Obtén el último día del mes
    var lastDay = new Date(year, month, 0).getDate();

    // Formatea la fecha con ceros a la izquierda si es necesario
    var formattedMonth = (month < 10) ? '0' + month : month;
    var formattedLastDay = (lastDay < 10) ? '0' + lastDay : lastDay;

    var formattedDate = `${year}-${formattedMonth}-${formattedLastDay}`;
    return formattedDate;
}