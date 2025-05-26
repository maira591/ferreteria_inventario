class Parametro {
    constructor(Id, Value) {
        this.Id = Id;
        this.Value = Value;
    }
}

var baseUrl, container, currentReportId;
$(function () {
    baseUrl = $('#baseUrl').val();
    container = $('#report-parameters');

    document.addEventListener("reloadstart", e => {
        $('#loading-spin').removeClass('d-none');
        $('#preview-title').removeClass('d-none');
        $('#preview-title').html("Cargando vista previa...");

    });

    document.addEventListener("reloadend", e => {
        $('#loading-spin').addClass('d-none');
        $('#preview-title').html("Vista previa");
    });
});

window.io = {
    open: function (verb, url, data, target) {
        var form = document.createElement("form");
        form.action = url;
        form.method = verb;
        form.target = target || "_self";
        if (data) {
            for (var key in data) {
                var input = document.createElement("textarea");
                input.name = key;
                input.value = typeof data[key] === "object"
                    ? JSON.stringify(data[key])
                    : data[key];
                form.appendChild(input);
            }
        }
        form.style.display = 'none';
        document.body.appendChild(form);
        form.submit();
        document.body.removeChild(form);
    }
};

var RunReport = (function () {
    function RunReport() { }

    /**
     * Carga los parametros del reporte
     * @param {any} reportId
     */
    RunReport.LoadReportParameters = function (reportId) {
        if (reportId !== "") {
            currentReportId = reportId;
            $.ajax({
                url: baseUrl + 'GetReportParameters',
                method: 'post',
                beforeSend: function () {
                    showLoading('Consultando parámetros...');
                    container.html('');
                    $('#report-preview').html('');
                },
                data: { id: currentReportId },
                success: function (data) {
                    var report = RunReport.GetSelectedReport();
                    if (report.PreView) { $('#btn-preview').removeClass('d-none'); }
                    $('#btn-export').removeClass('d-none');

                    $.each(data, function (index, item) {
                        RunReport.DrawControl(item);
                    });
                },
                error: function (jqXHR, exception) {
                    showAlert("Unexpected error", "warning");
                },
                complete: function () {
                    hideLoading();
                    $('#my-ajax-grid').html('');
                    $('#preview-title').addClass('d-none');
                    $('.date input').datepicker({ language: 'esp', clearBtn: true, format: 'dd/mm/yyyy' });
                }
            });
        }
        else {
            $('#preview-title').addClass('d-none');
            $('#btn-export').addClass('d-none');
            $('#btn-preview').addClass('d-none');
            $('#report-parameters').html('')
            $('#my-ajax-grid').html('');
        }
    };

    /**
     * Construye los controles para los parametros del reporte
     * @param {any} item
     */
    RunReport.DrawControl = function (item) {
        var required = item.IsRequired ? `data-val-required="El campo ${item.Name} es requerido." required` : '';
        var classRequired = item.IsRequired ? 'required' : '';
        var replaceKey = item.ReplaceKey.replace(/[{}]/g, "");
        var isOrganization = item.EntityCode !== null && item.EntityCodeParameter === replaceKey ? `readonly="readonly" value="${item.EntityCode}"`: "";
        var html = `<div class="row mb-1">
                                    <div class="col-sm-2">
                                        <label for="${replaceKey}">${item.Name}</label>
                                    </div>
                                    <div class="col-sm-4">`;

        switch (item.DataType) {
            case 'DATE':
                html += `<div class="input-group date">
                                    <input class="form-control ${classRequired} date-mask" id="${replaceKey}" ${required} type="text">
                                 </div>`;
                break;
            case 'VARCHAR2':
                html += `<input class="form-control ${classRequired}" id="${replaceKey}" ${required} maxlength="250" type="text"/>`;
                break;
            case 'NUMBER':
                html += `<input maxlength="30" pattern="/^-?\\d+\\.?\\d*$/" onKeyPress="if(this.value.length==30) return false;" type="number" id="${replaceKey}" class="form-control inputPositiveInput ${classRequired}" ${required} ${isOrganization} value="0"/>`;
                break;
            case 'LISTA':
                if (item.ListType === 2) { html += `<input type="hidden" id="${replaceKey}">`; }
                html += RunReport.GetList(item.Id, replaceKey, item.ListType, classRequired, isOrganization, item.EntityCode);
                break;
            default:
                break;
        }
        html += '</div></div>';
        container.append(html);
        if (item.ListType === 2) {
            $(`#slc_${replaceKey}`).multiselect({
                includeSelectAllOption: false,
                enableCaseInsensitiveFiltering: true,
                filterPlaceholder: 'Buscar ...',
                maxHeight: 150,
                onChange: function (option, checked) {
                    RunReport.GetSelectedOptions(replaceKey);
                },
                buttonTextAlignment: 'left',
                buttonWidth: '100%'
            });
        }

        RunReport.FormatInputs();
    };


    RunReport.FormatInputs = function () {
        $('.date-mask').mask('00/00/0000', { placeholder: "__/__/____" });
    };

    /**
     * Optiene las opciones seleccionadas de un multiselect
     * @param {any} element
     */
    RunReport.GetSelectedOptions = function (element) {
        var selectedOptions = $(`#slc_${element} option:selected`);
        var ids = "";
        selectedOptions.each(function (index, element) {
            if (ids === "") {
                ids = element.value;
            }
            else {
                ids = ids + "," + element.value;
            }
        });

        $(`#${element}`).val(ids);
        return ids;
    };

    /**
     * Genera un combo select
     * @param {any} query
     * @param {any} name
     * @param {any} listType
     * @param {any} classRequired
     */
    RunReport.GetList = function (id, name, listType, classRequired, isOrganization, organizationValue) {
        var html = '';
        $.ajax({
            async: false,
            method: "POST",
            url: baseUrl + "GetListOptions",
            beforeSend: function () {
                showLoading('Generando lista...');
            },
            data: { id },
            success: function (data) {
                var classMultiple = '', isMultiple = '', slcName = '';
                if (listType === 2) {
                    classMultiple = 'select2';
                    isMultiple = 'multiple="multiple"';
                    slcName = "slc_";
                }

                html = `<select id="${slcName}${name}" class="form-control ${classMultiple} ${classRequired}" ${isMultiple} ${isOrganization}>`;
                
                if (isOrganization !== "") {
                    var item = data.find(x => x.Id === organizationValue);
                    html += `<option value="${item.Id}">${item.Name}</option>`;
                }
                else {
                    $.each(data, function (index, item) {
                        html += `<option value="${item.Id}">${item.Name}</option>`;
                    });
                }

                html += '</select>';
            },
            error: function (jqXHR, exception) {
                showAlert("Unexpected error", "warning");
            },
            complete: function () {
                hideLoading();
            }
        });

        return html;
    };

    /**Ejecuta la previzualicación del reporte */
    RunReport.PreviewReport = function () {
        if (formValidation("paramsForm")) {
            RunReport.BuildQuery(RunReport.RenderGrid);
        }
    };

    /**Exporta el reporte */
    RunReport.ExportReport = function () {
        if (formValidation("paramsForm")) {
            var reportName = $('#reportsList').find(':selected').text();
            RunReport.BuildQuery();
            var url = `${baseUrl}ExportReport`;
            var data = { reportName };
            io.open('POST', url, data, '_blank');
        }
    };

    /**Establece el Query */
    RunReport.BuildQuery = function(callback){
        var params = RunReport.BuildParams();
        var parameters = { ReportId: currentReportId, Parameters: params };
        $.ajax({
            method: "POST",
            url: baseUrl + "SetQuery",
            beforeSend: function () {
                showLoading('Consultando...');
            },
            contentType: "application/json",
            data: JSON.stringify(parameters),
            success: function () {
                if (callback !== undefined) {
                    callback();
                }
            },
            error: function (jqXHR, exception, error) {
                showAlert('Ocurrió un error al ejecutar el reporte: ' + jqXHR.responseText);
            },
            complete: function () {
                hideLoading();
            }
        });
    };

    /**Construye los parametros*/
    RunReport.BuildParams= function () {
        var arrParameter = [];
        var parameters = $("#report-parameters :input");
        //var query = report.SQLSentence;
        $.each(parameters, function (index, item) {
            var params = item.value.replaceAll("'", "\"").replaceAll(",", "','");
            arrParameter.push(new Parametro(Id = item.id, Value = params));
        });
        return arrParameter;
    };

    /**Obtiene el reporte seleccionado */
    RunReport.GetSelectedReport = function () {
        var lstReports = JSON.parse($('#hfReports').val());
        var report = lstReports.find(rep => rep.Id === currentReportId);
        return report;
    };

    RunReport.RenderGrid = function () {
        RenderGrid();
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    return RunReport;
})();
