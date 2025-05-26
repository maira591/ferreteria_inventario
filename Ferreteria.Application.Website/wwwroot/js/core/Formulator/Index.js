var formulatorData;
var contSolidarias;
var idContFormula = "FormulaCont";
//var jsonColumns;
/**
 * Get the operators
 */
$.ajax({
    url: $('#baseUrl').val() + 'Validation/GetOperators?IsEdit=',
    method: 'GET',
    success: function (data) {
        formulatorData = data;
        typeFormatSelect();
        buildTooltips();
        if (document.getElementById("ModelFormula").value !== '') {
            procesarModelo();
        } else if (document.getElementById("Formula").value !== '') {
            $("#sql-tab").click();
        }
        hideLoading();
    }
});

var inputConst = '<input type="text" class="const" placeholder="Ingrese valor"  data-tippy-content="Digite su valor"/>';

var contWhere = '<span><select class="filtro" data-tippy-content="Filtro" onchange="where(this)">'
    + '<option value="" selected>No filtrar</option>'
    + '<option value="f">Filtrar por:</option>'
    + '</select></span>';

function contConst() {
    var html = '<span>' + inputConst + '<select onchange="getConst(this)" class="selConst col-sm-2" data-tippy-content="Seleccione">';
    var constants = JSON.parse(formulatorData.constants);
    $(constants).each(function () {
        html += `<option value="${this.Value}">${this.Text}</option>`;
    });
    html += '</select></span>';
    return html;
}


var si =
    '<select class="condicional" onchange="opSi(this)" data-tippy-content="Condicional">' +
    '<option></option>' +
    '<option value="Si">Si</option>' +
    '</select>';


function opLogica(func, tipo) {
    var html = '<br/><select class="logica" data-tippy-content="Logica" onchange="' + func + '(this,\'' + tipo + '\' )">';
    var logicOperators = JSON.parse(formulatorData.logicOperators);
    $(logicOperators).each(function () {
        html += `<option value="${this.Value}">${this.Text}</option>`;
    });
    html += '</select>';
    return html;
}


function opComparacion(func, tipo) {

    var html = '<select class="comparacion" data-tippy-content="Comparacion"';
    if (tipo === 'w') {
        html += 'onchange="' + func + '(this)"';
    }
    html += '>';

    var comparisionOperators = JSON.parse(formulatorData.comparisionOperators);
    $(comparisionOperators).each(function () {
        if (this.Type === tipo || this.Type === null) {
            html += `<option value="${this.Value}">${this.Text}</option>`;
        }
    });

    html += '</select>';
    return html;
}

function opAritmetica(func, tipo) {
    var html = '<select class="aritmetica" data-tippy-content="Aritmetica" onchange="' + func + '(this,\'' + tipo + '\')">';
    var arithmeticOperators = JSON.parse(formulatorData.arithmeticOperators);
    $(arithmeticOperators).each(function () {
        if (this.Value === "") {
            html += `<option selected value="${this.Value}">${this.Text}</option>`;
        }
        else {
            html += `<option value="${this.Value}">${this.Text}</option>`;
        }        
    });

    html += '</select>';
    return html;
}

function agrupar(clase, tipo) {
    var title = clase === 'agruparVB' && tipo === 'i' ||
        clase === 'agruparVA' && tipo === 'i' ||
        clase === 'agruparWA' && tipo === 'i' ||
        clase === 'agruparWB' && tipo === 'i'
        ? '('
        : clase === 'agruparVA' && tipo === 'f' ||
            clase === 'agruparVB' && tipo === 'f' ||
            clase === 'agruparWA' && tipo === 'f' ||
            clase === 'agruparWB' && tipo === 'f'
            ? ')'
            : '';

    var saltoDeLinea =
        clase === 'agruparVB' && tipo === 'i' ||
            clase === 'agruparVA' && tipo === 'f' ||
            clase === 'agruparWA' && tipo === 'f' ||
            clase === 'agruparWB'
            ? '<br/>'
            : '';

    return saltoDeLinea +
        '<select onchange="agrupador(this, \'' +
        tipo +
        '\')" class="agrupador' + tipo + ' ' +
        clase +
        '" data-tippy-content="Agrupador"><option/><option>' +
        title +
        '</option></select>';
}

function agrupador(el, tipo) {
    if (tipo === 'i')
        el.append('(');
    else
        el.append(')');
}

function opSi(el) {
    if (el.parentNode.nextSibling !== null && el.options[el.selectedIndex].value === '') {
        el.parentNode.parentNode.removeChild(el.parentNode.nextSibling);
    }
    if (el.options[el.selectedIndex].value === 'Si') {
        var newNode = document.createElement('span');
        newNode.innerHTML = '<b>Entonces</b>' + validacionNode().outerHTML;
        el.parentNode.parentNode.insertBefore(newNode, el.parentNode.nextSibling);
    }
}

function formato() {
    var el = document.getElementById("TypeFormats");
    var tipoValidacion = document.getElementById("Type");
    var html = agrupar('agruparVB', 'i') +
        '<span class="Formato"><select class="sFormato" onchange="agregarColumnna(this)" data-tippy-content="Formato" class="col-sm-4"><option value="" Selected>-- Seleccione Formato o Ingrese un valor --></option>';

    var formats = JSON.parse(formulatorData.formats);
    console.log(formats);
    var formatoVal = document.getElementById("Format").value;
    var typeFormat = document.getElementById("TypeFormats").value;

    if (contSolidarias != undefined) {
        formatoVal = contSolidarias;
    }
    $(formats).each(function () {
        if (tipoValidacion.options[tipoValidacion.selectedIndex].value === 'Fila' &&
            formatoVal === this.Value && this.Type == typeFormat) {
            html += `<option value="${this.Value}">${this.Text}</option>`;
        }
        if (tipoValidacion.options[tipoValidacion.selectedIndex].value === 'Formato' && this.Type == typeFormat) {
            html += `<option value="${this.Value}">${this.Text}</option>`;
        }
    });

    html += '</select>' + contConst() + '</span>' + agrupar('agruparVB', 'f');
    return html;
}

function typeFormatSelect() {
    var el = document.getElementById("TypeFormats");
    var valueTypeFormat = el.options[el.selectedIndex].value
    var html = "";
    var formatElementeSelect = document.getElementById("Format");

    var formats = JSON.parse(formulatorData.formats);
    var formatsNews = formats.filter(x => x.Type == valueTypeFormat);

    if (formatsNews.length > 0) {
        $(formatsNews).each(function () {
            if (formatElementeSelect.value == this.Value) {
                html += `<option value="${this.Value}" selected>${this.Text}</option>`;

            } else {
                html += `<option value="${this.Value}">${this.Text}</option>`;
            }
        });
    }

    formatElementeSelect.innerHTML = html;
    contSolidarias = formatElementeSelect.value;
    formula();
    contSolidarias = undefined;
}


function whereNode(el) {
    var col = columnas(el, 'w');
    var newNode = document.createElement('span');
    newNode.className = "separator";
    newNode.innerHTML = agrupar('agruparWA', 'i') + col.outerHTML + "<span>" + opAritmetica('whereAritmetica', 'w') + "</span>" + opComparacion('whereComparacion', 'w') +
        col.outerHTML + "<span>" + opAritmetica('whereAritmetica', 'w') + "</span>" + agrupar('agruparWA', 'f') + opLogica('whereLogica', 'w');
    return newNode;
}

function validacionNode() {
    var newNode = document.createElement('span');
    newNode.className = "separator";
    newNode.innerHTML = agrupar('agruparVA', 'i') + formato() + "<span>" + opAritmetica('whereAritmetica', 'v') + "</span>" + opComparacion('whereComparacion', 'v') +
        formato() + "<span>" + opAritmetica('whereAritmetica', 'v') + "</span>" + agrupar('agruparVA', 'f') + opLogica('whereLogica', 'v');

    return newNode;
}

function validacion() {
    var el = document.getElementById(idContFormula);
    var tipoValidacion = document.getElementById("Type");
    var newNode = document.createElement('span');
    var whereFiNode = document.createElement('span');
    if (tipoValidacion.options[tipoValidacion.selectedIndex].value === 'Fila') {
        whereFiNode.innerHTML = "Donde[" + contWhere + "]";
    }
    newNode.innerHTML = si + validacionNode().outerHTML;
    el.appendChild(newNode);
    el.appendChild(whereFiNode);
    buildTooltips();
}

function agregarColumnna(el) {
    var col = columnas(el);
    if (el.nextSibling !== null) {
        el.parentNode.removeChild(el.nextSibling);
    }
    el.parentNode.insertBefore(col, el.nextSibling);
    buildTooltips();
}

function columnas(el, tipo) {
    var contColumns = document.createElement("span");
    var selectColumns = document.createElement("select");
    var tipoValidacion = document.getElementById("Type");
    var tipoValidacionVal = tipoValidacion.options[tipoValidacion.selectedIndex].value;
    var esFormatoPrimerNivel = false;

    var parent = el;
    while (parent.parentNode !== null) {
        if (typeof parent.parentNode.className !== 'undefined' && parent.parentNode.className === "Formato")
            break;
        parent = parent.parentNode;
    }

    var selectFormato;
    if (tipoValidacionVal === 'Formato' || parent.parentNode !== null) {
        esFormatoPrimerNivel = true;
        selectFormato = parent.parentNode.firstChild;
    }
    else {
        selectFormato = document.getElementById("Format");
    }
    if (tipoValidacionVal === 'Fila') {
        esFormatoPrimerNivel = false;
    }
    var varFormato = selectFormato.options[selectFormato.selectedIndex].value.replace("_", "");
    if (varFormato === '') {
        contColumns.innerHTML = contConst();
    }
    else {
        var esTipo = typeof tipo !== 'undefined' && tipo !== null && tipo !== '';
        if (esTipo) {
            //Opción vacia del where y de los filtros
            var firstItem = document.createElement("option");
            firstItem.textContent = "-- Seleccione Columna o Ingrese valor -->";
            firstItem.value = "";
            selectColumns.appendChild(firstItem);
            selectColumns.setAttribute("onchange", "columnChange(this)");
            selectColumns.setAttribute("class", "sColumna col-sm-4");
        }

        var columns = JSON.parse(formulatorData.columns);
        $(columns).each(function () {
            if (this.Format === varFormato && ((esFormatoPrimerNivel && this.Type === 'NUMBER') || !esFormatoPrimerNivel || esTipo)) {

                var item = document.createElement("option");
                item.textContent = this.FormatColumn;
                item.value = this.Value;
                selectColumns.appendChild(item);
                selectColumns.setAttribute("class", "sColumna col-sm-4");
                selectColumns.setAttribute("data-tippy-content", "Campo");

            }
        });
        if (esTipo) {
            contColumns.innerHTML = agrupar('agruparWB', 'i') + selectColumns.outerHTML + contConst() + agrupar('agruparWB', 'f');
        } else {
            var sum = "";
            var htmlCont = "";
            if (tipoValidacionVal === 'Formato') {
                sum = "&sum;";
                htmlCont = '<br/><span class="separator">[' + contWhere + ']</span><br/>';
            }
            contColumns.innerHTML = sum + selectColumns.outerHTML + htmlCont;
        }
    }

    return contColumns;
}

function columnChange(el) {
    if (el.nextSibling !== null) {
        el.parentNode.removeChild(el.nextSibling);
    }
    if (el.options[el.selectedIndex].value === '') {
        var newNode = document.createElement('span');
        newNode.innerHTML = contConst();
        el.parentNode.insertBefore(newNode, el.nextSibling);
    }

}

function where(el) {
    if (el.options[el.selectedIndex].value === 'f') {
        var col = columnas(el, 'w');
        var newNode = whereNode(el);
        el.parentNode.appendChild(newNode);
    } else {
        while (el.nextSibling !== null) {
            el.parentNode.removeChild(el.nextSibling);
        }
    }
    buildTooltips();
}

function whereAdd(el) {
    var newNode = document.createElement('span');
    newNode.className = "separator";
    newNode.innerHTML = ' , ' + inputConst + '<input type="button" onclick="whereDelete(this)" value="-"/>';
    el.parentNode.insertBefore(newNode, el.parentNode.lastChild);
}

function whereDelete(el) {
    var parent = el.parentNode.parentNode;
    parent.removeChild(el.parentNode);
}

function getConst(el) {
    if (el !== null) {
        var inputConstElement = document.createElement("input");
        inputConstElement.type = "text";
        if (el.options[el.selectedIndex].value === '') {
            el.setAttribute("class", "col-sm-2");
            var parent = el.parentNode;
            var html = inputConst + parent.innerHTML;
            parent.innerHTML = html;
        } else if (el.parentNode !== null) {
            if (el !== el.parentNode.firstChild) {
                el.setAttribute("class", "col-sm-4");
                el.parentNode.removeChild(el.parentNode.firstChild);
            }
        }
    }
    buildTooltips();
}

function whereComparacion(el) {
    var selectedValue = el.options[el.selectedIndex].value;
    if (selectedValue === "sub" || selectedValue === "nsub") {
        el.nextSibling.innerHTML = '(<span>' + inputConst + '<input type="button" onclick="whereAdd(this)" value="+" /></span> )';
        var arithmeticSelect = el.nextSibling.nextSibling.firstChild;
        arithmeticSelect.value = "";
        arithmeticSelect.disabled = true;
        whereAritmetica(arithmeticSelect, 'w');
    } else {
        el.nextSibling.nextSibling.firstChild.disabled = false;
        el.nextSibling.innerHTML = columnas(el, 'w').outerHTML;
    }
}

function whereLogica(el, tipo) {
    var parent = el.parentNode.parentNode;
    var sibling = el.parentNode.nextSibling;
    if (el.options[el.selectedIndex].value !== '?') {
        if (sibling === null) {
            var newNode;
            if (tipo === "w") {
                newNode = whereNode(el);
            }
            else {
                newNode = validacionNode();
            }
            parent.appendChild(newNode);
        }
    } else {
        if (sibling !== null) {
            var selectedIndex = sibling.lastChild.selectedIndex;
            el.selectedIndex = selectedIndex;
            parent.removeChild(sibling);
        }
    }
    buildTooltips();
}

function whereAritmetica(el, tipo) {
    if (el.options[el.selectedIndex].value !== '') {
        var newNode = document.createElement('span');
        var inputHtml = "";
        if (tipo === 'w') {
            var col = columnas(el, tipo);
            inputHtml = col.outerHTML;
        }
        else {
            inputHtml = formato();
        }
        newNode.innerHTML = inputHtml + opAritmetica('whereAritmetica', tipo);
        if (el.nextSibling !== null) {
            el.parentNode.removeChild(el.nextSibling);
        }
        el.parentNode.insertBefore(newNode, el.nextSibling);
    } else {
        if (el.nextSibling !== null) {
            el.parentNode.removeChild(el.nextSibling);
        }
    }
}

function formula() {
    var formulaElement = document.getElementById(idContFormula);
    formulaElement.innerHTML = "";

    validacion();
    var formatSelect = document.getElementsByClassName("slFormato");
    for (var i = 0; i < formatSelect.length; i++) {
        agregarColumnna(formatSelect[i]);
    }
}

function leer(root, node, lastParent, html) {
    if (node === null) {
        if (lastParent !== root) {
            node = lastParent.nextSibling;
            lastParent = lastParent.parentNode;
        }
        if (lastParent === root && node === null) {
            document.getElementById("ModelFormula").value = html;
        } else {
            leer(root, node, lastParent, html);
        }
    } else {
        html += getElementValue(node);
        if (node.hasChildNodes() && !isInputElement(node)) {
            leer(root, node.firstChild, lastParent, html);
        }
        else {
            lastParent = node.parentNode;
            node = node.nextSibling;
            leer(root, node, lastParent, html);
        }
    }

}

function isInputElement(el) {
    if (el.tagName === 'SELECT' || el.tagName === 'INPUT' || el.nodeType === 3) {
        return true;
    }
    return false;
}

function getElementValue(el) {
    var value = ' ';

    if (el.tagName === 'SELECT') {
        value += el.options[el.selectedIndex].value;
    }
    else if (el.tagName === 'INPUT' && el.type !== 'button') {
        if (strSplitContains(el.className, "const")) {
            var re = new RegExp(" ", 'g');
            value += '"' + el.value.replace(re, "|nbsp|") + '"';
        }
        else {
            value += el.value;
        }
    } else if (el.nodeType === 3) {
        value += el.nodeValue;
    }
    value += ' ';
    if (value === '  ') {
        value = '';
    }
    return value;
}

function generarFormula() {
    var formula = document.getElementById(idContFormula);
    var node = formula.firstChild;
    leer(formula, node, null, '');
}
var lastNode = null;
function procesarModelo() {
    var arithmeticOperators = JSON.parse(formulatorData.arithmeticOperators);
    var logicOperators = JSON.parse(formulatorData.logicOperators);
    var comparisonOperators = JSON.parse(formulatorData.comparisionOperators);
    var constants = JSON.parse(formulatorData.constants);

    var root = document.getElementById(idContFormula);
    lastNode = root.firstChild;
    var modelo = document.getElementById("ModelFormula").value;
    var skip = false;
    var res = modelo.split(" ");
    res = res.filter(function (el) {
        return el !== null && typeof el !== 'undefined' && el !== '';
    });
    for (var item of res) {
        var classNode = "";
        if (item === "Si") {
            classNode = "condicional";
        } else if (item === "f") {
            classNode = "filtro";
        } else if (item.substring(0, 1) === '"') {
            item = item.replace(/\|nbsp\|/g, ' ');
            item = item.replace(/"/g, '');
            classNode = "const";
        } else if (item.substring(0, 1) === '|') {
            classNode = "sColumna";
        } else if (item.substring(0, 1) === '_') {
            classNode = "sFormato";
        } else if (operatorContains(arithmeticOperators, item)) {
            classNode = "aritmetica";
        } else if (operatorContains(logicOperators, item)) {
            classNode = "logica";
        } else if (operatorContains(comparisonOperators, item)) {
            if (item === "sub" || item === "nsub" ) {
                skip = true;
            }
            classNode = "comparacion";
        } else if (operatorContains(constants, item)) {
            classNode = "selConst";
        } else if (item === "(") {
            if (!skip) {
                classNode = "agrupadori";
            }
        } else if (item === ")") {
            if (!skip) {
                classNode = "agrupadorf";
            } else {
                skip = false;
            }
        } else if (item === ",") {
            if (strSplitContains(lastNode.parentNode.className, 'separator'))
                whereAdd(lastNode.parentNode);
            else {
                whereAdd(lastNode);
            }
            lastNode = lastNode.nextSibling;
        }

        if (classNode !== "") {
            buscarNodo(root, lastNode, null, classNode, item);
        }
    }
}

function buscarNodo(root, node, lastParent, classNode, value) {
    if (node !== null && typeof node.className !== 'undefined' && strSplitContains(node.className, classNode)) {
        node.value = value;
        var event = new Event('change');
        node.dispatchEvent(event);
        lastNode = node;
    } else {
        if (node === null) {
            if (lastParent !== root) {
                node = lastParent.nextSibling;
                lastParent = lastParent.parentNode;
            }
            if (!(lastParent === root && node === null)) {
                buscarNodo(root, node, lastParent, classNode, value);
            }
        } else {
            if (node.hasChildNodes() && !isInputElement(node)) {
                buscarNodo(root, node.firstChild, lastParent, classNode, value);
            }
            else {
                lastParent = node.parentNode;
                node = node.nextSibling;
                buscarNodo(root, node, lastParent, classNode, value);
            }
        }
    }
}

function operatorContains(arr, value) {
    var contains = arr.filter(function (operator) {
        return operator.Value === value;
    });
    return contains.length > 0;
}

function strSplitContains(str, value) {
    var contains = str.split(" ");
    contains = contains.filter(function (strVal) {
        return strVal === value;
    });
    return contains.length > 0;
}


function buildTooltips() {
    tippy('[data-tippy-content]',
        {
            arrow: true,
            arrowType: 'round',
            animation: 'scale'
        });
}

function SaveInformation(urlSaveInfo) {
    var isOnlySQL = $('#IsOnlySQL').is(":checked");

    if (formValidation("frmValidations")) {

        if (!validationIsOnlySQL()) {
            return false;
        }

        var sql = $("#sql-tab").hasClass('active');
        var msg = '';
        if (!isOnlySQL) {
            if (sql) {
                msg = 'Se guardarán los datos del SQL y los datos de la formula se perderán. ¿Está seguro que desea continuar?';
            }
            else {
                msg = 'Se guardarán los datos de la formula y los datos del SQL se perderán. ¿Está seguro que desea continuar?';
            }
        }
        else {
            msg = 'Se guardarán los datos del SQL y los datos de la formula se perderán. ¿Está seguro que desea continuar?';
        }        

        ShowModalQuestion('Advertencia', msg, function () {
            if (sql) {
                $('#ModelFormula').val('');
            } else {
                $('#Formula').val('');
                generarFormula();
            }
            SaveData(urlSaveInfo);
            HideModalQuestion();
        }, function () {
            HideModalQuestion();
        });
    }   
}

function SaveData(urlSaveInfo) {

    var modelFormula = $('#ModelFormula').val();
    var formula = $('#Formula').data('ace').editor.ace.getValue();

    showLoading('Guardando validación...').then(function () {
        var jsonSender = {};
        jsonSender.Id = $('#Id').val();
        jsonSender.MessageId = $('#MessageId').val();
        jsonSender.Description = $('#Description').val();
        jsonSender.Error = $('#Error').is(":checked");
        jsonSender.ExpirationDate = $('#ExpirationDate').val();
        jsonSender.Format = $("#Format option:selected").val();
        jsonSender.FormatTypeId = $("#TypeFormats").val();
        jsonSender.Type = $("#Type option:selected").val();
        jsonSender.ModelFormula = modelFormula;
        jsonSender.Formula = formula;
        jsonSender.IsOnlySQL = $('#IsOnlySQL').is(":checked");

        $.post(urlSaveInfo, jsonSender, function (response) {
            try {
                if (response.Status === 200) {
                    $('#ModelFormula').val(response.Body.ModelFormula);
                    $('#Id').val(response.Body.Id);
                    var editor = $('#Formula').data('ace').editor.ace;
                    editor.setValue(response.Body.Formula);
                    showAlert(response.Message, 'success');
                    location.href = $('#baseUrl').val() + "Validation";
                } else {
                    showAlert(response.Message, 'danger');
                }
            } catch (e) {
                showAlert(e.message, 'danger');
            }

            hideLoading();
        });
    });
}

//Valida que si se marca la validacion como solo SQL, se debe ingresar la consulta a guardar.
function validationIsOnlySQL() {
    var formula = $('#Formula').data('ace').editor.ace.getValue();
    var isOnlySQL = $('#IsOnlySQL').is(":checked");

    if (isOnlySQL && formula.length == 0) {
        showAlert("No se ha ingresado el texto SQL de la formula.", 'danger');
        return false;
    }
    else {
        return true;
    }

}