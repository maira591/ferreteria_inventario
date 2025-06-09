var Role = (function () {
    function Role() {

    }

    //Método encargado de abrir la ventana para editar
    Role.OpenEdit = function (urlEdit) {
        showLoading();
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            Role.Init();
            hideLoading();
        });
    };

    //Método encargado de recargar la información
    Role.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    Role.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Role.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };


    //Método encargado de realizar el borrado de un tipo de carga.
    Role.Disabled = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Role.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Role.OpenCreate = function (url) {
        showLoading();
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            Role.Init();
            hideLoading();
        });
    };

    Role.Save = function () {
        var periodicity = getObject("formRolecrud");

        if (formValidation("formRolecrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'Role/Validations',
                type: 'POST',
                data: periodicity,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("Role/CreateOrUpdate", "Post", periodicity, "Role.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del permiso.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    Role.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        Role.Reload();
    };


    Role.GetSelectedPermission = function () {
        var selectedOptions = $('#PermissionId option:selected');
        var ids = "";
        selectedOptions.each(function (index, element) {
            if (ids == "") {
                ids = element.value;
            }
            else {
                ids = ids + "," + element.value;
            }
        });
        $("#Permissions").val(ids);
        return ids;
    };

    Role.ValidatePeriodicitySelection = function () {
        return Role.GetSelectedPermission().length > 0;
    };


    Role.Init = function () {
        setAlphanumericInput();

        $('#PermissionId').multiselect({
            includeSelectAllOption: false,
            maxHeight: 150,
            onChange: function (option, checked) {
                Role.GetSelectedPermission();
            },
            buttonTextAlignment: 'left',
            buttonWidth: '100%'
        });

        if ($("#Permissions").val() != "") {
            Role.SetPeriodicitySelection();
        }
    };

    Role.SetPeriodicitySelection = function () {
        var split = $("#Permissions").val().split(',');

        for (var i = 0; i < split.length; i++) {
            $('#PermissionId option').each(function () {
                if ($(this).val() === split[i]) {
                    $(this).prop('selected', true);
                    return;
                }
            });
        };

        $('#PermissionId').multiselect('refresh');
    };

    return Role;
})();