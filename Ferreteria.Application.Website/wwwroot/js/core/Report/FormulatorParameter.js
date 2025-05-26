var parameters;
var comparisionOperators;
var arithmeticOperators;
var logicOperators;
var tables;
var arrayFilters = [];

var FormulatorParameter = (function () {

    function FormulatorParameter() {

    }

    FormulatorParameter.GetListParameters = function () {
        showLoading("Cargando parámetros...")
        if ($("#Id").val() != '00000000-0000-0000-0000-000000000000') {
            Report.GetColumnsEdit($("#CodeTable").val())
        }

        Petitions.Get($('#baseUrl').val() + 'Report/GetListParameters', function (response) {
            parameters = response.parameters;
            comparisionOperators = JSON.parse(response.comparisionOperators)
            arithmeticOperators = JSON.parse(response.arithmeticOperators);
            logicOperators = JSON.parse(response.logicOperators);

            if ($("#IsOnlySQL").is(":checked")) {
                $(".contParameters").show();
                $("#tapParameters").addClass("disabledTab");
                $("#sql-tab").click()
            }

            if ($("#Id").val() != '00000000-0000-0000-0000-000000000000' && !$("#IsOnlySQL").is(":checked")) {
                FormulatorParameter.GetArrayFilters()
            }

            if ($("#ReportParameters").val() == '') {
                $("#FormulaCont").html(FormulatorParameter.GetInitialParameters());
                $('.select2').select2();
                CreateAllTooltips();
            }
            hideLoading()
        });
    };


    FormulatorParameter.GetInitialParameters = function () {
        var html = "";
        html += `<div class="row mt-2 pl-3 condition">`

        html += `<select class="select2 column"><option value="" selected>Seleccione una columna</option>`;

        $.each(columnsTable, function (key, value) {
            html += '<option value=' + value.Id + '>' + value.Name + '</option>';
        });

        html += '</select>';
        html += `${opComparacion("cc", "w")}`;

        html += `<select class="select2 parametro"><option value="" selected>Seleccione un parámetro</option>`;

        parameters.forEach(item => {
            html += `<option value="${item.Id}">${item.Name}</option>`;
        })

        html += '</select>';

        html += `${opLogica("whereLogica", "w")} </div>`;


        return html;
    }

    FormulatorParameter.GetArrayFilters = function () {
        var query = $('#SQLSentence').val();
        if (query.includes("WHERE")) {
            var filters = query.split("WHERE")[1];
            var numberParameters = filters.match(/{/g).length;

            for (var i = 0; i < numberParameters; i++) {

                var splitFilter = []
                var stringFilter = filters.substring(filters.indexOf("("), filters.indexOf(")") + (filters.includes("IN(") || filters.includes("NOT IN(") ? 2 : 1))
                filters = filters.replace(stringFilter, "");

                stringFilter = stringFilter.replace('"', "").replace('"', "").replace('{', "").replace('}', "")

                if (stringFilter.includes("IN(")) {
                    stringFilter = stringFilter.replace("(", "").replace(")", "").replace(")", "");
                    splitFilter = stringFilter.replace("IN(", "IN ").split(" ");
                }
                else if (stringFilter.includes("NOT IN(")) {
                    stringFilter = stringFilter.replace("(", "").replace(")", "").replace(")", "");
                    stringFilter = stringFilter.replace("NOT IN(", " NOT IN ").split(" ");
                }
                else {
                    splitFilter = stringFilter.replace("(", "").replace(")", "").split(" ");
                }

                if (filters.includes("AND")) {
                    splitFilter.push("AND")
                    filters = filters.replace("AND", "");
                }
                else if (filters.includes("OR")) {
                    splitFilter.push("OR")
                    filters = filters.replace("OR", "");
                }

                arrayFilters.push(splitFilter);
            }

            FormulatorParameter.GetInitialParametersEdit();
        }
    }

    FormulatorParameter.GetInitialParametersEdit = function () {
        var html = "";

        for (var index = 0; index < arrayFilters.length; index++) {
            var item = arrayFilters[index];

            html += `<div class="row mt-2 pl-3 condition">`

            html += `<select class="select2 column"><option value="" selected>Seleccione una columna</option>`;

            $.each(columnsTable, function (key, value) {
                html += `<option value="${value.Id}" ${value.Id == item[0] ? "selected" : ""}> ${value.Name} </option>`;
            });

            html += '</select>';
            html += `${opComparacion("cc", "w", item[1])}`;

            html += `<select class="select2 parametro"><option value="" selected>Seleccione un parámetro</option>`;
            var param = parameters.filter(x => x.ReplaceKey == `{${item[2]}}`)[0]
            $.each(parameters, function (key, value) {

                html += `<option value="${value.Id}" ${value.Id == param.Id ? "selected" : ""}> ${value.Name} </option>`;
            });

            html += '</select>';

            html += `${opLogica("whereLogica", "w", (item.length > 3 ? item[3] : null))} </div>`;
        }

        $("#FormulaCont").html(html);
        $('.select2').select2();
    }

    return FormulatorParameter;
})();

function opComparacion(func, tipo, valueSelected) {

    var html = '<select class="comparacion mr-2 ml-2" data-tippy-content="Comparación"';

    html += '>';

    comparisionOperators.forEach(item => {
        if (item.Type == tipo || item.Type === null) {
            html += `<option value="${item.Value}" ${item.Value == valueSelected ? "selected" : ""}>${item.Text}</option>`;
        }
    })

    html += '</select>';
    return html;
}

function opLogica(func, tipo, valueSelected) {
    var html = `<select class="logica ml-2" data-tippy-content="Logica" style="margin-left:0px;" onchange="${func}(this, '${tipo}');">`;

    if (valueSelected != "") {
        if (valueSelected == "AND") {
            valueSelected = "Y";
        } else if (valueSelected == "OR") {
            valueSelected = "O";
        }
    }

    logicOperators.forEach(item => {
        html += `<option value="${item.Value}" ${item.Value == valueSelected ? "selected" : ""}>${item.Text}</option>`;
    })

    html += '</select>';
    return html;
}

function whereLogica(element, tipo) {
    if (element.value !== '?') {
        if ($(element).parent().next('div').length == 0) {

            var columnElement = $(element).parent().children()[0];
            var parameterElement = $(element).parent().children()[3]

            var elementColumnToBorderError = columnElement.nextSibling.childNodes[0].childNodes[0]
            var elementParameterToBorderError = parameterElement.nextSibling.childNodes[0].childNodes[0]


            if ($(columnElement).val() == "") {
                $(elementColumnToBorderError).css("border", "1px #dd3f3f solid");
            } else {
                $(elementColumnToBorderError).css("border", "1px #bbb solid");
            }

            if ($(parameterElement).val() == "") {
                $(elementParameterToBorderError).css("border", "1px #dd3f3f solid");
            } else {
                $(elementParameterToBorderError).css("border", "1px #bbb solid");
            }

            if ($(columnElement).val() == "" || $(parameterElement).val() == "") {
                showAlert("Debe seleccionar una columna y un parámetro para poder agregar otro filtro.", "warning");
                element.value = '?';
            } else {
                $("#FormulaCont").append(FormulatorParameter.GetInitialParameters());
                $('.select2').select2();
                CreateAllTooltips();
            }
        }
    }
    else {
        $(element).parent().nextAll('div').remove();
    }
}

function GetFiltersQuery() {
    var idParameters = "";
    var column = "";
    var comparacion = "";
    var parameter = "";
    var logica = "";
    var cont = 0;
    var lstSelect = $(".condition select");
    var sqlWhere = "";
    for (var index = 0; index < lstSelect.length; index++) {
        var item = lstSelect[index];

        if ($(item).val() != "") {
            if (cont == 0) {
                column = "";
                column = `"${$(item).val()}"`
                cont++;
            }
            else if (cont == 1) {
                if ($(item).val() == "IN" || $(item).val() == "NOT IN") {
                    comparacion = $(item).val() + "("
                } else {
                    comparacion = $(item).val()
                }
                cont++;
            }
            else if (cont == 2) {
                parameter = "";

                if (idParameters == "") {
                    idParameters = $(item).val()
                } else {
                    idParameters = idParameters + "," + $(item).val();
                }

                $("#ReportParameters").val(idParameters);

                if (comparacion == "IN(" || comparacion == "NOT IN(") {
                    parameter = parameters.filter(x => x.Id == $(item).val())[0].ReplaceKey + ")"
                } else {
                    parameter = parameters.filter(x => x.Id == $(item).val())[0].ReplaceKey
                }

                cont++;
            }
            else if (cont == 3 && $($(".condition select")[index + 1]).val() != "") {
                logica = "";
                if ($(item).val() == "Y") {
                    logica = "AND"
                } else if ($(item).val() == "O") {
                    logica = "OR"
                }
                cont++;
            }
            else {
                cont = 1;
                if (comparacion == "IN(" || comparacion == "NOT IN(") {
                    sqlWhere += `(${column} ${comparacion}${parameter}) ${logica} `;
                } else {
                    sqlWhere += `(${column} ${comparacion} ${parameter}) ${logica} `;
                }

                column = "";
                column = `"${$(item).val()}"`
            }

        } else {

            if (sqlWhere.endsWith("AND ")) {
                sqlWhere = sqlWhere.substring(0, sqlWhere.length - 4);
            }
            else if (sqlWhere.endsWith("OR ")) {
                sqlWhere = sqlWhere.substring(0, sqlWhere.length - 4);
            }

            if (sqlWhere.length > 0) {
                sqlWhere = `WHERE ${sqlWhere}`
            }

            return sqlWhere;
        }
    }

    if (index = lstSelect.length - 1) {
        if (comparacion == "IN(" || comparacion == "NOT IN(") {
            sqlWhere += `(${column} ${comparacion}${parameter}) ${logica} `;
        } else {
            sqlWhere += `(${column} ${comparacion} ${parameter}) ${logica} `;
        }

        cont = 0;
    }


    if (sqlWhere.endsWith("AND ")) {
        sqlWhere = sqlWhere.substring(0, sqlWhere.length - 4);
    }
    else if (sqlWhere.endsWith("OR ")) {
        sqlWhere = sqlWhere.substring(0, sqlWhere.length - 4);
    }

    if (sqlWhere.length > 0) {
        sqlWhere = `WHERE ${sqlWhere}`
    }

    return sqlWhere;
}



