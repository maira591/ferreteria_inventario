var baseUrl, baseDirectory

var CalculateArrears = (function () {

    function CalculateArrears() {

    }

    CalculateArrears.Init = function () {
        $('#DateCalculation').mask('00/00/0000', { placeholder: "__/__/____" });
        $('.select2').select2();

        var options = {
            language: 'esp',
            format: "dd/mm/yyyy",
        };
        $('.dp-date input').datepicker(options);

        baseUrl = $('#UrlBase').val();

        $("#EntityType").change(function () {
            CalculateArrears.GetEntities($(this).val());
        });

        $("#CalculationType").change(function () {
            if ($(this).val() == 'true') {
                $(".info-cooperative").show()
                $(".info-control-cooperative").addClass('required')
            } else {
                $(".info-cooperative").hide()
                $(".info-control-cooperative").removeClass('required')
            }
        });
    };

    CalculateArrears.GetEntities = function (entityTypeId) {
        if (entityTypeId != '') {
            ExecuteAjax("RecaudoCalculateArrears/GetEntityList", "GET", { entityType: entityTypeId }, "CalculateArrears.LoadEntities", null);
        } else {
            $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');
        }
    };

    CalculateArrears.VerifyValuesFilters = function () {
        if (formValidation("form")) {
            showLoading("Validando...")
            ExecuteAjax("RecaudoCalculateArrears/ValidateArrearsCalculation", "GET", {
                entityType: $('#EntityType').val(),
                entityCode: $('#EntityCode').val(),
                dateCalculation: $('#DateCalculation').val(),
                dateNowCalculation: $('#DateNowCalculation').val(),
                isCalculateAllCooperatives: $('#CalculationType').val()
            }, "CalculateArrears.ResulValidationArrearsCalculation", null);
        }
    };

    CalculateArrears.LoadEntities = function (data) {
        //Se remueven todos los items, se deja opcion seleccione. 
        $('#EntityCode').find('option').remove().end().append('<option value="">Seleccione</option>').val('');

        //Se agregan los items que vienen en la variable data.
        var listitems = "";
        $.each(data, function (key, value) {
            listitems += '<option value=' + value.Value + '>' + value.Text + '</option>';
        });
        $('#EntityCode').append(listitems);
    };

    CalculateArrears.ResulValidationArrearsCalculation = function (data) {
        hideLoading()
        if (data.Message == null && data.Status) {
            CalculateArrears.RunCalculation();
        } else {
            if (!data.Status) {
                showAlert(data.Message, 'warning')
            } else {
                ShowModalQuestion('Advertencia', data.Message, function () {
                    HideModalQuestion();
                    CalculateArrears.RunCalculation();
                }, undefined, undefined, 'Calcular');
            }
           
        }
    };

    CalculateArrears.RunCalculation = function () {

        $.ajax({
            beforeSend: function () { showLoading('Calculando...'); },
            url: baseUrl + 'RunCalculation',
            method: 'POST',
            data: {
                entityType: $('#EntityType').val(),
                entityCode: $('#EntityCode').val(),
                dateCalculation: $('#DateCalculation').val(),
                dateNowCalculation: $('#DateNowCalculation').val(),
                isCalculateAllCooperatives: $('#CalculationType').val()
            },
            success: function (data) {
                showAlert(`Cálculos  de mora ejecutados correctamente`, "success");
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


    return CalculateArrears;
})();