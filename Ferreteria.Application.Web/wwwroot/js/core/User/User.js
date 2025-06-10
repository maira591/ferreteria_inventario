var User = (function () {
    function User() {

    }

    //Método encargado de abrir la ventana para editar
    User.OpenEdit = function (urlEdit) {
        showLoading();
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            User.Init();
            hideLoading();
        });
    };

    //Método encargado de recargar la información
    User.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de realizar el borrado.
    User.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    User.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };


    //Método encargado de realizar el borrado de un tipo de carga.
    User.Disabled = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    User.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    User.OpenCreate = function (url) {
        showLoading();
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
            User.Init();
            hideLoading();
        });
    };

    User.Save = function () {
        var obj = getObject("formUsercrud");

        if (formValidation("formUsercrud")) {
            showLoading("Validando...");

            $.ajaxSetup({ cache: false });
            $.ajax({
                ContentType: "application/json",
                url: 'User/Validations',
                type: 'POST',
                data: obj,
                success: function (data) {
                    if (!data.Valid) {
                        hideLoading();
                        showAlert(replaceAll(data.Message, "|", "<br>"), 'danger');
                    }
                    else {
                        showLoading("Guardando...");
                        ExecuteAjax("User/CreateOrUpdate", "Post", obj, "User.ResultSave");
                    }
                },
                error: function (jqXHR, exception) {
                    showAlert("No fue posible realizar la validación del permiso.", 'danger');
                    hideLoading();
                }
            });
        }
    };

    User.ResultSave = function (response) {
        hideLoading();
        if (response.Success) {
            showAlert(response.Message, 'success');
        } else {
            showAlert(response.Message, 'danger');
        }
        hideModalPartial();
        User.Reload();
    };


    User.GetSelectedRole = function () {
        var selectedOptions = $('#RoleId option:selected');
        var ids = "";
        selectedOptions.each(function (index, element) {
            if (ids == "") {
                ids = element.value;
            }
            else {
                ids = ids + "," + element.value;
            }
        });
        $("#Roles").val(ids);
        return ids;
    };

    User.ValidatePeriodicitySelection = function () {
        return User.GetSelectedRole().length > 0;
    };


    User.Init = function () {
        setAlphanumericInput();

        $('#RoleId').multiselect({
            includeSelectAllOption: false,
            maxHeight: 150,
            onChange: function (option, checked) {
                User.GetSelectedRole();
            },
            buttonTextAlignment: 'left',
            buttonWidth: '100%'
        });

        if ($("#Roles").val() != "") {
            User.SetPeriodicitySelection();
        }
    };

    User.SetPeriodicitySelection = function () {
        var split = $("#Roles").val().split(',');

        for (var i = 0; i < split.length; i++) {
            $('#RoleId option').each(function () {
                if ($(this).val() === split[i]) {
                    $(this).prop('selected', true);
                    return;
                }
            });
        };

        $('#RoleId').multiselect('refresh');
    };

    return User;
})();