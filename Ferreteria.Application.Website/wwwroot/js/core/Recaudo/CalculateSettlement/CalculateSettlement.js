var baseUrl, baseDirectory

var CalculateSettlement = (function () {

    function CalculateSettlement() {

    }

    CalculateSettlement.Init = function () {
        $('#CutoffDate').mask('00/00/0000', { placeholder: "__/__/____" });

        var options = {
            language: 'esp',
            format: "dd/mm/yyyy",
        };
        $('.dp-date input').datepicker(options);

        baseUrl = $('#UrlBase').val();
    };


    CalculateSettlement.VerifyValuesFilters = function () {
        if (formValidation("form")) {
            showLoading("Validando...")
            ExecuteAjax("RecaudoSettlementCalculation/ValidateExistsSettlementCalculation", "GET", {
                cutoffDate: $('#CutoffDate').val()
            }, "CalculateSettlement.ResulValidateExistsSettlementCalculation", null);
        }
    };

    CalculateSettlement.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };

    CalculateSettlement.ResulValidateExistsSettlementCalculation = function (data) {
        hideLoading()
        if (data.Message == null && data.Status) {
            CalculateSettlement.RunCalculation();
        } else {
            if (!data.Status) {
                showAlert(data.Message, 'warning')
            } else {
                ShowModalQuestion('Advertencia', data.Message, function () {
                    HideModalQuestion();
                    CalculateSettlement.RunCalculation();
                }, undefined, undefined, 'Calcular', 'Cancelar');
            }
           
        }
    };

    CalculateSettlement.RunCalculation = function () {

        $.ajax({
            beforeSend: function () { showLoading('Calculando...'); },
            url: baseUrl + 'RunCalculation',
            method: 'POST',
            data: {
                cutoffDate: $('#CutoffDate').val()
            },
            success: function (data) {
                showAlert(`Cálculos ejecutados correctamente`, "success");
            },
            error: function (jqXHR, exception) {
                showAlert("No terminó satisfactoriamente los cálculos.", "warning");
                HideModalQuestion()
            },
            complete: function () {
                hideLoading()
            }
        });
    };


    return CalculateSettlement;
})();