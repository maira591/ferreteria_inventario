var MonFechaCorte = (function () {
    function MonFechaCorte() {

    }

    //Método encargado de abrir la ventana para editar
    MonFechaCorte.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los formatos
    MonFechaCorte.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };


    MonFechaCorte.Save = function () {

        var format = getObject("formentity");
        if (formValidation("formentity")) {

            $.ajaxSetup({ cache: false });

            showLoading("Guardando...");
            ExecuteAjax("MonitoreoFechaCorte/CreateOrUpdate", "Post", format, "MonFechaCorte.ResultSave");
            hideLoading();
        }
    };

    MonFechaCorte.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            MonFechaCorte.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }
    };

    MonFechaCorte.InitControls = function () {
        $('#FechaInicio').val($('#hdf_FechaInicio').val())
        $('#FechaFin').val($('#hdf_FechaFin').val());
        $('#FechaHoraPlanvisita').val($('#hdf_FechaHoraPlanvisita').val());
        $('#FechaHoraIniMonitoreo').val($('#hdf_FechaHoraIniMonitoreo').val());
        $('#FechaHoraAgregados').val($('#hdf_FechaHoraAgregados').val());
        $('#FechaHoraConInfoCore').val($('#hdf_FechaHoraConInfoCore').val());
        $('#FechaHoraConInfoSiaf').val($('#hdf_FechaHoraConInfoSiaf').val());

        setDateTimePicker();
    };

    return MonFechaCorte;
})();
